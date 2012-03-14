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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.LocalDataStore;
using ClearCanvas.ImageViewer.Services;

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
				//TODO (Time Review): Change this back to use Platform.Time once we've resolved
				//the exception throwing issue.
				DateTime now = DateTime.Now;
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
							var republishItem = item.Clone();
							republishItem.MessageType = MessageType.Republish;
							LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(republishItem);
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
							if (item.Pending && DateTime.Now.Subtract(item.LastActive) >= oneMinute)
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
						progressItem.StartTime = DateTime.Now;
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

					progressItem.LastActive = DateTime.Now;

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
						progressItem.LastActive = DateTime.Now;
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
						progressItem.LastActive = DateTime.Now;
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
