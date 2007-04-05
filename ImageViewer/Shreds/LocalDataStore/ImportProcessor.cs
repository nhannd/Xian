using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using System.IO;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreService
	{
		private class ImportProcessor : ImportProcessorBase
		{
			private LocalDataStoreService _parent;

			private object _syncLock = new object();
			private bool _paused;
			private List<FileImportJobInformation> _importJobs;
			
			public ImportProcessor(LocalDataStoreService parent)
			{
				_importJobs = new List<FileImportJobInformation>();
				_parent = parent;
				_paused = false;
				_parent.CancelEvent += new EventHandler<ItemEventArgs<CancelProgressItemInformation>>(OnCancel);
				_parent.RepublishEvent += new EventHandler(OnRepublish);
				_parent.ClearInactiveEvent += new EventHandler(OnClearInactive);
				_parent.Importer.ImportThreadPoolSwitched += new EventHandler<ItemEventArgs<DicomFileImporter.DedicatedImportQueue>>(OnImportThreadPoolSwitched);
			}

			protected new void ProcessFileImportResults(DicomFileImporter.FileImportInformation results)
			{
				FileImportInformation information = (FileImportInformation)results;

				base.ProcessFileImportResults(results);

				lock (_syncLock)
				{
					lock (information.FileImportJobInformation)
					{
						if (_paused && !information.FileImportJobInformation.ProgressItem.IsImportComplete() && !information.FileImportJobInformation.ProgressItem.Cancelled)
						{
							information.FileImportJobInformation.ProgressItem.StatusMessage = SR.MessageImportPausedForReindex;
							UpdateProgress(information.FileImportJobInformation.ProgressItem);
						}
					}
				}
			}

			private void OnImportThreadPoolSwitched(object sender, ItemEventArgs<DicomFileImporter.DedicatedImportQueue> args)
			{
				lock (_syncLock)
				{
					if (args.Item != DicomFileImporter.DedicatedImportQueue.Reindex)
					{
						_paused = false;
					}
					else
					{
						_paused = true;

						foreach (FileImportJobInformation jobInformation in _importJobs)
						{
							lock (jobInformation)
							{
								if (!jobInformation.ProgressItem.IsImportComplete() && !jobInformation.ProgressItem.Cancelled)
								{
									jobInformation.ProgressItem.StatusMessage = SR.MessageImportPausedForReindex;
									UpdateProgress(jobInformation.ProgressItem);
								}
							}
						}
					}
				}
			}

			private void OnClearInactive(object sender, EventArgs e)
			{
				List<FileImportJobInformation> clearJobs = new List<FileImportJobInformation>();
				lock (_syncLock)
				{
					clearJobs.AddRange(_importJobs);
				}

				foreach (FileImportJobInformation jobInformation in clearJobs)
				{
					ClearJob(jobInformation);
				}
			}

			private void OnRepublish(object sender, EventArgs e)
			{
				lock (_syncLock)
				{
					foreach (FileImportJobInformation jobInformation in _importJobs)
					{
						lock (jobInformation)
						{
							UpdateProgress(jobInformation.ProgressItem);
						}
					}
				}
			}

			private void OnCancel(object sender, ItemEventArgs<CancelProgressItemInformation> e)
			{
				CancelProgressItemInformation information = e.Item;

				if (information.ProgressItemIdentifiers == null)
					return;

				if ((information.CancellationFlags & CancellationFlags.Cancel) == CancellationFlags.Cancel)
				{
					foreach (Guid guid in information.ProgressItemIdentifiers)
					{
						FileImportJobInformation jobInformation = null;
						lock (_syncLock)
						{
							jobInformation = _importJobs.Find(delegate(FileImportJobInformation test) { return test.ProgressItem.Identifier.Equals(guid); });
						}

						if (jobInformation != null)
						{
							CancelJob(jobInformation);
						}
					}
				}

				if ((information.CancellationFlags & CancellationFlags.Clear) == CancellationFlags.Clear)
				{
					foreach (Guid guid in information.ProgressItemIdentifiers)
					{
						FileImportJobInformation jobInformation = null;
						lock (_syncLock)
						{
							jobInformation = _importJobs.Find(delegate(FileImportJobInformation test) { return test.ProgressItem.Identifier.Equals(guid); });
						}

						if (jobInformation != null)
						{
							ClearJob(jobInformation);
						}
					}
				}
			}

			private void ValidateRequest(FileImportRequest request)
			{
				if (request.FilePaths == null)
					throw new ArgumentNullException(SR.ExceptionNoFilesHaveBeenSpecifiedToImport);

				int paths = 0;
				int badPaths = 0;

				foreach (string path in request.FilePaths)
				{
					if (Directory.Exists(path) || File.Exists(path))
						++paths;
					else
						++badPaths;
				}

				if (paths == 0)
					throw new ArgumentNullException(SR.ExceptionNoValidFilesHaveBeenSpecifiedToImport);
			}

			protected override void UpdateProgress(ImportProgressItem progressItem)
			{
				LocalDataStoreActivityPublisher.Instance.ImportProgressChanged(progressItem.Clone());
			}

			protected override void AddToImportQueue(ImportProcessorBase.FileImportInformation fileImportInformation)
			{
				_parent.Importer.Enqueue(fileImportInformation, this.ProcessFileImportResults, DicomFileImporter.DedicatedImportQueue.Default);
			}

			protected override void ClearJob(FileImportJobInformation jobInformation)
			{
				base.ClearJob(jobInformation);

				lock (jobInformation)
				{
					if (jobInformation.ProgressItem.Removed)
					{
						lock (_syncLock)
						{
							if (_importJobs.Contains(jobInformation))
								_importJobs.Remove(jobInformation);
						}
					}
				}
			}

			public Guid Import(FileImportRequest request)
			{
				ValidateRequest(request);

				ImportProgressItem progressItem = new ImportProgressItem();

				progressItem.Identifier = Guid.NewGuid();
				progressItem.AllowedCancellationOperations = CancellationFlags.Cancel;
				progressItem.StartTime = DateTime.Now;
				progressItem.LastActive = progressItem.StartTime;

				progressItem.NumberOfFailedImports = 0;
				progressItem.NumberOfFilesImported = 0;
				progressItem.TotalFilesToImport = 0;
				progressItem.NumberOfFilesCommittedToDataStore = 0;

				progressItem.StatusMessage = SR.MessagePending;

				FileImportJobInformation jobInformation = new FileImportJobInformation(progressItem, request.FileImportBehaviour, request.BadFileBehaviour);

				List<string> fileExtensions = new List<string>();
				if (request.FileExtensions != null)
					fileExtensions.AddRange(request.FileExtensions);

				List<string> filePaths = new List<string>(request.FilePaths);

				if (filePaths.Count > 1)
				{
					jobInformation.ProgressItem.Description = String.Format(SR.FormatMultipleFilesDescription, filePaths[0]);
				}
				else
				{
					jobInformation.ProgressItem.Description = filePaths[0];
				}

				lock (_syncLock)
				{
					_importJobs.Add(jobInformation);
				}

				base.Import(jobInformation, filePaths, fileExtensions, request.Recursive);

				return progressItem.Identifier;
			}
		}
	}
}