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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreService
	{
		internal class ReceivedFileProcessor
		{
			private class ReceivedFileImportInformation : DicomFileImporter.FileImportInformation
			{
				private string _sourceAETitle;

				public ReceivedFileImportInformation(string sourceFile, string sourceAETitle)
					: base(sourceFile, FileImportBehaviour.Move, BadFileBehaviour.Move)
				{
					_sourceAETitle = sourceAETitle;
				}

				public string SourceAETitle
				{
					get { return _sourceAETitle; }
				}
			}

			private class InternalReceiveProgressItem : ReceiveProgressItem
			{
				private string _terminalErrorMessage;
				private bool _pending;

				public InternalReceiveProgressItem()
				{
					_terminalErrorMessage = null;
					_pending = true;
				}

				public string TerminalErrorMessage
				{
					get { return _terminalErrorMessage; }
					set { _terminalErrorMessage = value; }
				}

				public bool Pending
				{
					get { return _pending; }
					set { _pending = value; }
				}
			}

			private LocalDataStoreService _parent;
			private object _syncLock = new object();
			private List<InternalReceiveProgressItem> _receiveProgressItems;
			private System.Threading.Timer _noActivityTimer;

			public ReceivedFileProcessor(LocalDataStoreService parent)
			{
				_receiveProgressItems = new List<InternalReceiveProgressItem>();
				
				_parent = parent;

				_parent.PurgeEvent += new EventHandler(OnPurge);
				_parent.StartEvent += new EventHandler(OnStart);
				_parent.StopEvent += new EventHandler(OnStop);
				_parent.CancelEvent += new EventHandler<ItemEventArgs<CancelProgressItemInformation>>(OnCancel);
				_parent.RepublishEvent += new EventHandler(OnRepublish);
			}

			void OnStart(object sender, EventArgs e)
			{
				lock (_syncLock)
				{
					_noActivityTimer = new System.Threading.Timer(OnTimer, null, 15000, 15000);
				}
			}

			void OnStop(object sender, EventArgs e)
			{
				lock (_syncLock)
				{
					_noActivityTimer.Dispose();
					_noActivityTimer = null;
				}
			}

			void OnPurge(object sender, EventArgs e)
			{
				DateTime now = Platform.Time;
				TimeSpan timeLimit = TimeSpan.FromMinutes(LocalDataStoreServiceSettings.Instance.PurgeTimeMinutes);
				
				lock (_syncLock)
				{
					List<InternalReceiveProgressItem> clearItems = new List<InternalReceiveProgressItem>();
					foreach (InternalReceiveProgressItem item in _receiveProgressItems)
					{
						lock (item)
						{
							bool isOld = now.Subtract(item.LastActive) > timeLimit;
							bool hasErrors = item.TotalDataStoreCommitFailures > 0 || 
												!String.IsNullOrEmpty(item.StatusMessage);

							if (isOld && !hasErrors)
								clearItems.Add(item);
						}
					}

					foreach (InternalReceiveProgressItem item in clearItems)
					{
						_receiveProgressItems.Remove(item);
						item.Removed = true;
						LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(item.Clone());
					}
				}
			}

			private void OnRepublish(object sender, EventArgs e)
			{
				lock (_syncLock)
				{
					foreach (InternalReceiveProgressItem item in _receiveProgressItems)
					{
						lock (item)
						{
							LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(item.Clone());
						}
					}
				}
			}

			private void OnCancel(object sender, ItemEventArgs<CancelProgressItemInformation> e)
			{
				CancelProgressItemInformation information = e.Item;

				if (information.ProgressItemIdentifiers == null)
					return;

				if (information.CancellationFlags != CancellationFlags.Clear)
					return;

				lock (_syncLock)
				{
					foreach (Guid identifier in information.ProgressItemIdentifiers)
					{
						InternalReceiveProgressItem item = _receiveProgressItems.Find(delegate(InternalReceiveProgressItem test) { return test.Identifier.Equals(identifier); });
						if (item != null)
						{
							_receiveProgressItems.Remove(item);
							item.Removed = true;
							LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(item.Clone());
						}
					}
				}
			}

			private void OnTimer(object nothing)
			{
				lock (_syncLock)
				{
					if (_noActivityTimer == null)
						return;

					TimeSpan oneMinute = TimeSpan.FromMinutes(1);

					foreach (InternalReceiveProgressItem item in _receiveProgressItems)
					{
						lock(item)
						{
							if (item.Pending && Platform.Time.Subtract(item.LastActive) >= oneMinute)
							{
								if (item.StatusMessage != SR.MessageRetrieveLikelyFailed)
								{
									item.StatusMessage = SR.MessageRetrieveLikelyFailed;
									LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(item.Clone());
								}
							}
						}
					}
				}
			}

			private InternalReceiveProgressItem GetReceiveProgressItem(ReceivedFileImportInformation information, out bool exists)
			{
				StudyInformation studyInformation = new StudyInformation();
				studyInformation.PatientId = information.PatientId;
				studyInformation.PatientsName = information.PatientsName;
				
				DateTime date;
				DateParser.Parse(information.StudyDate, out date);
				studyInformation.StudyDate = date;

				studyInformation.StudyDescription = information.StudyDescription;
				studyInformation.StudyInstanceUid = information.StudyInstanceUid;

				return GetReceiveProgressItem(information.SourceAETitle, studyInformation, out exists);
			}

			private InternalReceiveProgressItem GetReceiveProgressItem(string fromAETitle, StudyInformation studyInformation, out bool exists)
			{
				InternalReceiveProgressItem progressItem = null;
				lock (_syncLock)
				{
					progressItem = _receiveProgressItems.Find(
						delegate(InternalReceiveProgressItem testItem)
						{
							return testItem.FromAETitle == fromAETitle &&
								testItem.StudyInformation.StudyInstanceUid == studyInformation.StudyInstanceUid;
						});

					exists = (progressItem != null);
					if (!exists)
					{
						progressItem = new InternalReceiveProgressItem();
						progressItem.Identifier = Guid.NewGuid();
						progressItem.AllowedCancellationOperations = CancellationFlags.Clear;
						progressItem.StartTime = Platform.Time;
						progressItem.LastActive = progressItem.StartTime;

						progressItem.FromAETitle = fromAETitle;
						progressItem.StudyInformation = studyInformation;

						_receiveProgressItems.Add(progressItem);
					}
				}

				return progressItem;
			}

			private void FormatErrorMessage(InternalReceiveProgressItem progressItem, Exception error)
			{
				string message = progressItem.TerminalErrorMessage != null ? progressItem.TerminalErrorMessage : error.Message;
				int errors = progressItem.TotalDataStoreCommitFailures;
				if (progressItem.TerminalErrorMessage != null)
					++errors;

				progressItem.StatusMessage = String.Format(SR.FormatReceiveErrorSummary, errors, message);
			}

			private void ProcessFileImportResults(DicomFileImporter.FileImportInformation results)
			{
				ReceivedFileImportInformation receivedFileImportInformation = results as ReceivedFileImportInformation;

				InternalReceiveProgressItem progressItem;
				bool exists;

				if (results.CompletedStage == DicomFileImporter.ImportStage.NotStarted)
				{
					if (results.Failed)
					{
						//This is a 'special' progress item for things that failed to parse.
						progressItem = GetReceiveProgressItem(receivedFileImportInformation.SourceAETitle, new StudyInformation(), out exists);
						lock (progressItem)
						{
							progressItem.Pending = false;
							++progressItem.NumberOfParseFailures;
							this.FormatErrorMessage(progressItem, receivedFileImportInformation.Error);
							LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(progressItem.Clone());
						}

						Platform.Log(LogLevel.Error, results.Error);
					}

					return;
				}

				progressItem = GetReceiveProgressItem(receivedFileImportInformation, out exists);
				lock (progressItem)
				{
					if (progressItem.Pending)
					{
						progressItem.Pending = false;
						progressItem.StatusMessage = "";
					}

					ImportedSopInstanceInformation importedSopInformation = null;

					progressItem.LastActive = Platform.Time;

					if (receivedFileImportInformation.Failed)
					{
						//When a failure is reported, it is because the 'next' stage failed, hence the stage 
						//that was attempted did not get completed.  This is why the logic below is the way it is.
						switch (results.CompletedStage)
						{
							case DicomFileImporter.ImportStage.FileParsed:
								++progressItem.NumberOfImportFailures;
								break;
							case DicomFileImporter.ImportStage.FileMoved:
								++progressItem.NumberOfDataStoreCommitFailures;
								break;
						}

						this.FormatErrorMessage(progressItem, results.Error);
						Platform.Log(LogLevel.Error, results.Error);
					}
					else if (receivedFileImportInformation.CompletedStage == DicomFileImporter.ImportStage.FileParsed)
					{
						++progressItem.NumberOfFilesParsed;
					}
					else if (receivedFileImportInformation.CompletedStage == DicomFileImporter.ImportStage.FileMoved)
					{
						if (!exists)
						{
							++progressItem.NumberOfFilesParsed;
						}

						++progressItem.NumberOfFilesImported;
					}
					else
					{
						if (!exists)
						{
							++progressItem.NumberOfFilesParsed;
							++progressItem.NumberOfFilesImported;
						}

						++progressItem.NumberOfFilesCommittedToDataStore;

						importedSopInformation = new ImportedSopInstanceInformation();
						importedSopInformation.StudyInstanceUid = receivedFileImportInformation.StudyInstanceUid;
						importedSopInformation.SeriesInstanceUid = receivedFileImportInformation.SeriesInstanceUid;
						importedSopInformation.SopInstanceUid = receivedFileImportInformation.SopInstanceUid;
						importedSopInformation.SopInstanceFileName = receivedFileImportInformation.FileName;
					}

					if (importedSopInformation != null)
						LocalDataStoreActivityPublisher.Instance.SopInstanceImported(importedSopInformation);

					LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(progressItem.Clone());
				}
			}

			public void ProcessReceivedFileInformation(StoreScpReceivedFileInformation receivedFileInformation)
			{
				_parent.Importer.Enqueue(new ReceivedFileImportInformation(receivedFileInformation.FileName, receivedFileInformation.AETitle), this.ProcessFileImportResults, DicomFileImporter.DedicatedImportQueue.Import);
			}

			public void RetrieveStarted(RetrieveStudyInformation information)
			{
				lock (_syncLock)
				{
					bool exists;
					InternalReceiveProgressItem progressItem = GetReceiveProgressItem(information.FromAETitle, information.StudyInformation, out exists);
					lock (progressItem)
					{
						progressItem.StudyInformation = information.StudyInformation;
						progressItem.LastActive = Platform.Time;
						progressItem.TerminalErrorMessage = null;
						progressItem.Pending = true;
						progressItem.StatusMessage = SR.MessagePending;
						LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(progressItem.Clone());
					}	
				}
			}

			public void ReceiveError(ReceiveErrorInformation errorInformation)
			{
				lock (_syncLock)
				{
					bool exists;
					InternalReceiveProgressItem progressItem = GetReceiveProgressItem(errorInformation.FromAETitle, errorInformation.StudyInformation, out exists);
					lock (progressItem)
					{
						progressItem.StudyInformation = errorInformation.StudyInformation;
						progressItem.LastActive = Platform.Time;
						progressItem.TerminalErrorMessage = errorInformation.ErrorMessage;
						progressItem.Pending = false;
						this.FormatErrorMessage(progressItem, new Exception(errorInformation.ErrorMessage));
						LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(progressItem.Clone());
					}
				}
			}
		}
	}
}
