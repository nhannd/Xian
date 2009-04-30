#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

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
						_activeJobInformation.ProgressItem.StartTime = Platform.Time;
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