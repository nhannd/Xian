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
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreService
	{
		private class ImportProcessor : ImportProcessorBase
		{
			private LocalDataStoreService _parent;

			private object _syncLock = new object();
			private ServiceState _parentState;
			private List<FileImportJobInformation> _importJobs;
			
			public ImportProcessor(LocalDataStoreService parent)
			{
				_importJobs = new List<FileImportJobInformation>();

				_parent = parent;
				_parentState = _parent._state;

				_parent.PurgeEvent += new EventHandler(OnPurge);
				_parent.CancelEvent += new EventHandler<ItemEventArgs<CancelProgressItemInformation>>(OnCancel);
				_parent.RepublishEvent += new EventHandler(OnRepublish);
				_parent.ClearInactiveEvent += new EventHandler(OnClearInactive);
				_parent.StateActivated += new EventHandler<ItemEventArgs<ServiceState>>(OnImportServiceStateChanged);
			}

			protected new void ProcessFileImportResults(DicomFileImporter.FileImportInformation results)
			{
				FileImportInformation information = (FileImportInformation)results;
				base.ProcessFileImportResults(results);

				lock (_syncLock)
				{
					if (_parentState == ServiceState.Importing)
						return;

					lock (information.FileImportJobInformation.SyncRoot)
					{
						if (!information.FileImportJobInformation.ProgressItem.IsImportComplete() && !information.FileImportJobInformation.ProgressItem.Cancelled)
						{
							if (_parentState == ServiceState.Reindexing)
								information.FileImportJobInformation.ProgressItem.StatusMessage = SR.MessageImportPausedForReindex;
							else //when neither import queue is active, it's a delete operation.
								information.FileImportJobInformation.ProgressItem.StatusMessage = SR.MessageImportPausedForDelete;

							UpdateProgress(information.FileImportJobInformation.ProgressItem);
						}
					}
				}
			}

			private void OnImportServiceStateChanged(object sender, ItemEventArgs<ServiceState> args)
			{
				lock (_syncLock)
				{
					_parentState = args.Item;
					if (_parentState == ServiceState.Importing)
						return;

					foreach (FileImportJobInformation jobInformation in _importJobs)
					{
						lock (jobInformation.SyncRoot)
						{
							if (!jobInformation.ProgressItem.IsImportComplete() && !jobInformation.ProgressItem.Cancelled)
							{
								if (_parentState == ServiceState.Reindexing)
									jobInformation.ProgressItem.StatusMessage = SR.MessageImportPausedForReindex;
								else //when neither import queue is active, it's a delete operation.
									jobInformation.ProgressItem.StatusMessage = SR.MessageImportPausedForDelete;

								UpdateProgress(jobInformation.ProgressItem);
							}
						}
					}
				}
			}

			private void OnPurge(object sender, EventArgs e)
			{
				//TODO (Time Review): Change this back to use Platform.Time once we've resolved
				//the exception throwing issue.
				DateTime now = DateTime.Now;
				TimeSpan timeLimit = TimeSpan.FromMinutes(LocalDataStoreServiceSettings.Instance.PurgeTimeMinutes);
				
				List<FileImportJobInformation> clearJobs = new List<FileImportJobInformation>();
				lock (_syncLock)
				{
					foreach (FileImportJobInformation jobInformation in _importJobs)
					{
						lock (jobInformation.SyncRoot)
						{
							bool isOld = now.Subtract(jobInformation.ProgressItem.LastActive) > timeLimit;
							bool hasErrors = jobInformation.ProgressItem.TotalDataStoreCommitFailures > 0;

							if (isOld && !hasErrors)
								clearJobs.Add(jobInformation);
						}
					}
				}

				foreach (FileImportJobInformation jobInformation in clearJobs)
					ClearJob(jobInformation);
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
						lock (jobInformation.SyncRoot)
						{
							var republishItem = jobInformation.ProgressItem.Clone();
							republishItem.MessageType = MessageType.Republish;
							UpdateProgress(republishItem);
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

			protected override void OnNoFilesToImport(FileImportJobInformation jobInformation)
			{
				lock (jobInformation.SyncRoot)
				{
					if (!jobInformation.ProgressItem.Cancelled)
					{
						jobInformation.ProgressItem.StatusMessage = SR.MessageNoFilesToImport;
						jobInformation.ProgressItem.AllowedCancellationOperations = CancellationFlags.Clear;
						jobInformation.DisposeClientUserContext();

						UpdateProgress(jobInformation.ProgressItem);
					}
				}
			}

			protected override void UpdateProgress(ImportProgressItem progressItem)
			{
				LocalDataStoreActivityPublisher.Instance.ImportProgressChanged(progressItem.Clone());
			}

			protected override void AddToImportQueue(ImportProcessorBase.FileImportInformation fileImportInformation)
			{
				_parent.Importer.Enqueue(fileImportInformation, this.ProcessFileImportResults, DicomFileImporter.DedicatedImportQueue.Import);
			}

			protected override void ClearJob(FileImportJobInformation jobInformation)
			{
				base.ClearJob(jobInformation);

				lock (jobInformation.SyncRoot)
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
				progressItem.StatusMessage = SR.MessagePending;
				progressItem.IsBackground = request.IsBackground;

				FileImportJobInformation jobInformation = new FileImportJobInformation(progressItem, request.FileImportBehaviour, request.BadFileBehaviour, UserIdentityContext.CreateFromCurrentThreadPrincipal());

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