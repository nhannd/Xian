using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using System.Threading;

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

					lock (_activeJobInformation)
					{
						_activeJobInformation.ProgressItem.Identifier = Guid.NewGuid();
						_activeJobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Cancel;
						_activeJobInformation.ProgressItem.StartTime = DateTime.Now;
						_activeJobInformation.ProgressItem.LastActive = _activeJobInformation.ProgressItem.StartTime;
						_activeJobInformation.ProgressItem.Cancelled = false;
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

			private void OnRepublish(object sender, EventArgs e)
			{
				lock (_syncLock)
				{
					lock (_activeJobInformation)
					{
						this.UpdateProgress(_activeJobInformation.ProgressItem);
					}
				}
			}

			private void OnCancel(object sender, ItemEventArgs<CancelProgressItemInformation> e)
			{
				CancelProgressItemInformation information = e.Item;

				lock (_syncLock)
				{
					foreach (Guid guid in information.ProgressItemIdentifiers)
					{
						if (guid.Equals(_activeJobInformation.ProgressItem.Identifier))
							CancelJob(_activeJobInformation);
					}
				}

				CheckResumeImports();
			}

			private void CheckResumeImports()
			{
				lock (_syncLock)
				{
					if (_resumingImports)
						return;

					lock (_activeJobInformation)
					{
						bool resumeImports = _active && (_activeJobInformation.ProgressItem.Cancelled || _activeJobInformation.ProgressItem.IsImportComplete());

						if (resumeImports)
						{
							_resumingImports = true;

							//do this on a separate thread, because what we are essentially doing is stopping this thread, so if we didn't do
							//the stop operation on another thread, we would cause a deadlock.
							WaitCallback resumeImportsDelegate = delegate(object reindexProcessor)
							{
								ReindexProcessor processor = (ReindexProcessor)reindexProcessor;

								lock (processor._syncLock)
								{
									processor._parent.Importer.ActivateImportQueue(DicomFileImporter.DedicatedImportQueue.Default);
									processor._active = false;
									processor._resumingImports = false;
								}
							};

							ThreadPool.QueueUserWorkItem(resumeImportsDelegate, this);
						};
					}
				}
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
						return; //don't throw an exception, just return.

					_active = true;
					NewJobInformation(false);

					WaitCallback reindexDelegate = delegate(object reindexProcessor)
					{
						_parent.Importer.ActivateImportQueue(DicomFileImporter.DedicatedImportQueue.Reindex);

						List<string> filePaths = new List<string>();
						filePaths.Add(LocalDataStoreService.Instance.StorageFolder);

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
}