#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.ImageViewer.Common.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreService
	{
		private class ReindexProcessor : ImportProcessorBase
		{
			private LocalDataStoreService _parent;

			private object _syncLock = new object();
			private FileImportJobInformation _activeJobInformation;
			private bool _active;
			private bool _resumingImports;

			public ReindexProcessor(LocalDataStoreService parent)
			{
				_parent = parent;
				_parent.CancelEvent += new EventHandler<ItemEventArgs<CancelProgressItemInformation>>(OnCancel);
				_parent.RepublishEvent += new EventHandler(OnRepublish);

				NewJobInformation(true);

				_resumingImports = false;
				_active = false;
			}

			private void NewJobInformation(bool inactive)
			{
				lock (_syncLock)
				{
					_activeJobInformation = new FileImportJobInformation(new ReindexProgressItem(), FileImportBehaviour.Move, BadFileBehaviour.Move);

					lock (_activeJobInformation.SyncRoot)
					{
						_activeJobInformation.ProgressItem.Identifier = Guid.NewGuid();
						_activeJobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.None;
						//TODO (Time Review): Change this back to use Platform.Time once we've resolved
						//the exception throwing issue.
						_activeJobInformation.ProgressItem.StartTime = DateTime.Now;
						_activeJobInformation.ProgressItem.LastActive = _activeJobInformation.ProgressItem.StartTime;
						_activeJobInformation.ProgressItem.Cancelled = false;
						_activeJobInformation.ProgressItem.Removed = false;
						
						if (!inactive)
						{
							_activeJobInformation.ProgressItem.StatusMessage = SR.MessagePending;
						}
						else
						{
							_activeJobInformation.ProgressItem.StatusMessage = SR.MessageInactive;
							_activeJobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
						}
					}
				}
			}

			private void UpdateProgress()
			{
				lock (_syncLock)
				{
					lock (_activeJobInformation.SyncRoot)
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
					if (!_active)
						return;

					foreach (Guid guid in information.ProgressItemIdentifiers)
					{
						if (guid.Equals(_activeJobInformation.ProgressItem.Identifier))
						{
							base.CancelJob(_activeJobInformation);
							CheckResumeImports();
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

					lock (_activeJobInformation.SyncRoot)
					{
						bool resumeImports = _active && (_activeJobInformation.ProgressItem.Cancelled || _activeJobInformation.ProgressItem.IsImportComplete());
						if (resumeImports)
						{
							_resumingImports = true;

							//do this on a separate thread, because what we are essentially doing is stopping this thread, so if we didn't do
							//the stop operation on another thread, we would cause a deadlock.
							WaitCallback resumeImportsDelegate = delegate
							{
								_parent.ReserveState(ServiceState.Importing);
								_parent.ActivateState();
								
								lock (_syncLock)
								{
									lock (_activeJobInformation.SyncRoot)
									{
										_activeJobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
										UpdateProgress();
									}

									_active = false;
									_resumingImports = false;
								}
							};

							ThreadPool.QueueUserWorkItem(resumeImportsDelegate, this);
						}
					}
				}
			}

			protected override void OnNoFilesToImport(FileImportJobInformation jobInformation)
			{
				lock (_syncLock)
				{
					lock (_activeJobInformation.SyncRoot)
					{
						if (!jobInformation.ProgressItem.Cancelled)
						{
							_activeJobInformation.ProgressItem.StatusMessage = SR.MessageNoFilesToReindex;
							_activeJobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;

							UpdateProgress(_activeJobInformation.ProgressItem);
						}
					}
				}

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

					// We don't want to actually stop the running imports from this thread because the 
					// WCF connection could time out.  Instead, let's just reserve the 'Reindexing' state and activate it
					// on the processing thread.  This will throw an exception if we cannot reserve the state.
					_parent.ReserveState(ServiceState.Reindexing);
					_active = true;
					NewJobInformation(false);
				}

				WaitCallback reindexDelegate = delegate(object reindexProcessor)
				{
					lock (_syncLock)
					{
						lock (_activeJobInformation.SyncRoot)
						{
							if (_activeJobInformation.ProgressItem.Cancelled)
								return;

							_activeJobInformation.ProgressItem.StatusMessage = SR.MessagePausingActiveImportJobs;
							UpdateProgress();
						}
					}

					//the 'reindex' state has already been reserved, let's activate it.
					_parent.ActivateState();

					bool clearFailed = false;

					try
					{
						lock (_syncLock)
						{
							lock (_activeJobInformation.SyncRoot)
							{
								if (_activeJobInformation.ProgressItem.Cancelled)
									return;

								_activeJobInformation.ProgressItem.StatusMessage = SR.MessageClearingDatabase;
								UpdateProgress();
							}
						}

						using (IDataStoreStudyRemover studyRemover = DataAccessLayer.GetIDataStoreStudyRemover())
						{
							studyRemover.ClearAllStudies();
						}

						LocalDataStoreActivityPublisher.Instance.LocalDataStoreCleared();
					}
					catch (Exception e)
					{
						clearFailed = true;
						Platform.Log(LogLevel.Error, e);

						lock (_syncLock)
						{
							lock (_activeJobInformation.SyncRoot)
							{
								_activeJobInformation.ProgressItem.StatusMessage = SR.MessageFailedToClearDatabase;
								_activeJobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
								UpdateProgress();
							}
						}
					}

					lock (_syncLock)
					{
						lock (_activeJobInformation.SyncRoot)
						{
							if (clearFailed)
							{
								CheckResumeImports();
								return;
							}

							if (_activeJobInformation.ProgressItem.Cancelled)
								return;

							//don't allow cancellation until after the data store has been cleared, otherwise you might get
							//some strange behaviour because the imports will be resumed while the data store is being cleared.
							_activeJobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Cancel;
						}
					}

					List<string> filePaths = new List<string>();
					filePaths.Add(LocalDataStoreService.Instance.StorageDirectory);

					lock (_syncLock)
					{
						// there are now xml files in the datastore, and everything in there should have a dcm extension, 
						// so just look for that.  Otherwise, the study xml files would be treated as 'bad' files.
						List<string> extensions = new List<string>();
						extensions.Add(".dcm");
						((ReindexProcessor)reindexProcessor).Import(_activeJobInformation, filePaths, extensions, true);
					}
				};

				ThreadPool.QueueUserWorkItem(reindexDelegate, this);
			}
		}
	}
}