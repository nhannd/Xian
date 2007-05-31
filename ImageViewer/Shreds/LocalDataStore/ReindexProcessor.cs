using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using System.Threading;
using NHibernate.Cfg;
using NHibernate;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreService
	{
		private class ReindexProcessor : ImportProcessorBase
		{
			private static class DatabaseCleaner
			{
				static Configuration _configuration;
				static ISessionFactory _sessionFactory;

				static DatabaseCleaner()
				{
					_configuration = new Configuration();
					_configuration.Configure("ClearCanvas.Dicom.DataStore.cfg.xml");
					_configuration.AddAssembly("ClearCanvas.Dicom.DataStore");
					_sessionFactory = _configuration.BuildSessionFactory();
				}

				public static void ClearAllStudies()
				{
					ITransaction transaction = null;
					ISession session = null;

					try
					{
						session = _sessionFactory.OpenSession();
						transaction = session.BeginTransaction();
						session.Delete("from Study");
						transaction.Commit();
					}
					catch
					{
						if (transaction != null)
							transaction.Rollback();

						throw;
					}
					finally
					{
						if (session != null)
							session.Close();
					}
				}
			}

			private LocalDataStoreService _parent;

			private object _syncLock = new object();
			private FileImportJobInformation _activeJobInformation;
			private bool _active;
			private bool _resumingImports;
			private bool _cancelRequested;

			public ReindexProcessor(LocalDataStoreService parent)
			{
				_parent = parent;
				_parent.CancelEvent += new EventHandler<ItemEventArgs<CancelProgressItemInformation>>(OnCancel);
				_parent.RepublishEvent += new EventHandler(OnRepublish);

				NewJobInformation(true);

				_resumingImports = false;
				_active = false;
				_cancelRequested = false;
			}

			private void NewJobInformation(bool inactive)
			{
				lock (_syncLock)
				{
					_activeJobInformation = new FileImportJobInformation(new ReindexProgressItem(), FileImportBehaviour.Move, BadFileBehaviour.Move);

					lock (_activeJobInformation)
					{
						_activeJobInformation.ProgressItem.Identifier = Guid.NewGuid();
						_activeJobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Cancel;
						_activeJobInformation.ProgressItem.StartTime = Platform.Time;
						_activeJobInformation.ProgressItem.LastActive = _activeJobInformation.ProgressItem.StartTime;
						_activeJobInformation.ProgressItem.Cancelled = false;
						_activeJobInformation.ProgressItem.Removed = false;
						_activeJobInformation.ProgressItem.NumberOfFailedImports = 0;
						_activeJobInformation.ProgressItem.NumberOfFilesImported = 0;
						_activeJobInformation.ProgressItem.TotalFilesToImport = 0;
						_activeJobInformation.ProgressItem.NumberOfFilesCommittedToDataStore = 0;
						
						if (!inactive)
							_activeJobInformation.ProgressItem.StatusMessage = SR.MessagePending;
						else
							_activeJobInformation.ProgressItem.StatusMessage = SR.MessageInactive;
					}
				}
			}

			private void UpdateProgress()
			{
				lock (_syncLock)
				{
					lock (_activeJobInformation)
					{
						this.UpdateProgress(_activeJobInformation.ProgressItem);
					}
				}
			}

			private void OnRepublish(object sender, EventArgs e)
			{
				UpdateProgress();
			}

			private void OnCancel(object sender, ItemEventArgs<CancelProgressItemInformation> e)
			{
				CancelProgressItemInformation information = e.Item;

				lock (_syncLock)
				{
					foreach (Guid guid in information.ProgressItemIdentifiers)
					{
						if (guid.Equals(_activeJobInformation.ProgressItem.Identifier))
						{
							if (_active)
							{
								lock (_activeJobInformation)
								{
									if (base.CanCancelJob(_activeJobInformation))
									{
										_cancelRequested = true;
										_activeJobInformation.ProgressItem.StatusMessage = SR.MessageCancelling;
										UpdateProgress();
									}
								}
							}

							break;
						}
					}
				}
			}
			
			private void CheckResumeImports()
			{
				lock (_syncLock)
				{
					if (_resumingImports)
						return;

					lock (_activeJobInformation)
					{
						if (_cancelRequested)
							base.CancelJob(_activeJobInformation);

						bool resumeImports = _active && (_activeJobInformation.ProgressItem.Cancelled || _activeJobInformation.ProgressItem.IsImportComplete());
						if (resumeImports)
						{
							_resumingImports = true;

							//do this on a separate thread, because what we are essentially doing is stopping this thread, so if we didn't do
							//the stop operation on another thread, we would cause a deadlock.
							WaitCallback resumeImportsDelegate = delegate(object nothing)
							{
								//the reindex database updates will have to complete before returning from this function.
								_parent.Importer.ActivateImportQueue(DicomFileImporter.DedicatedImportQueue.Default);

								lock (_syncLock)
								{
									_active = false;
									_resumingImports = false;
									_cancelRequested = false;
								}
							};

							ThreadPool.QueueUserWorkItem(resumeImportsDelegate, this);
						};
					}
				}
			}

			protected override void NotifyNoFilesToImport(FileImportJobInformation jobInformation)
			{
				jobInformation.ProgressItem.StatusMessage = SR.MessageNoFilesToReindex;
				jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;

				CheckResumeImports();
			}

			protected override void UpdateProgress(ImportProgressItem progressItem)
			{
				LocalDataStoreActivityPublisher.Instance.ReindexProgressChanged(((ReindexProgressItem)progressItem).Clone());
			}

			protected override void AddToImportQueue(ImportProcessorBase.FileImportInformation fileImportInformation)
			{
				_parent.Importer.Enqueue(fileImportInformation, ProcessReindexResults, DicomFileImporter.DedicatedImportQueue.Reindex);
			}

			protected void ProcessReindexResults(DicomFileImporter.FileImportInformation results)
			{
				lock (_syncLock)
				{
					FileImportInformation information = (FileImportInformation)results;
					if (information.FileImportJobInformation != _activeJobInformation)
						return; //not the current reindex item, just return.

					base.ProcessFileImportResults(results);
				}
				
				CheckResumeImports();
			}

			public void Reindex()
			{
				lock (_syncLock)
				{
					if (_active)
						return; //don't throw an exception, just ignore it.

					_active = true;
					NewJobInformation(false);
				}

				WaitCallback reindexDelegate = delegate(object reindexProcessor)
				{
					lock (_syncLock)
					{
						if (_cancelRequested)
						{
							CheckResumeImports();
							return;
						}

						lock (_activeJobInformation)
						{
							_activeJobInformation.ProgressItem.StatusMessage = SR.MessagePausingActiveImportJobs;
							UpdateProgress();
						}
					}

					_parent.Importer.ActivateImportQueue(DicomFileImporter.DedicatedImportQueue.Reindex);

					bool clearFailed = false;

					try
					{
						lock (_syncLock)
						{
							if (_cancelRequested)
							{
								CheckResumeImports();
								return;
							}

							lock (_activeJobInformation)
							{
								_activeJobInformation.ProgressItem.StatusMessage = SR.MessageClearingDatabase;
								UpdateProgress();
							}
						}

						DatabaseCleaner.ClearAllStudies();
					}
					catch (Exception e)
					{
						clearFailed = true;
						Platform.Log(e);

						lock (_syncLock)
						{
							lock (_activeJobInformation)
							{
								_activeJobInformation.ProgressItem.StatusMessage = SR.MessageFailedToClearDatabase;
								_activeJobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
								UpdateProgress();
							}
						}
					}

					lock (_syncLock)
					{
						if (clearFailed || _cancelRequested)
						{
							CheckResumeImports();
							return;
						}
					}

					List<string> filePaths = new List<string>();
					filePaths.Add(LocalDataStoreService.Instance.StorageDirectory);

					lock (_syncLock)
					{
						((ReindexProcessor)reindexProcessor).Import(_activeJobInformation, filePaths, new List<string>(), true);
					}
				};

				ThreadPool.QueueUserWorkItem(reindexDelegate, this);
			}
		}
	}
}