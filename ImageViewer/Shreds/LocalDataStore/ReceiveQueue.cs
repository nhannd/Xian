using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using System.Threading;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal class ReceiveQueue
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

		private object _receiveProgressItemsLock = new object();
		private List<ReceiveProgressItem> _receiveProgressItems;

		public ReceiveQueue()
		{
			_receiveProgressItems = new List<ReceiveProgressItem>();
		}

		private void OnImportThreadPoolStartStop(object sender, ItemEventArgs<SimpleThreadPool.StartStopState> e)
		{
			//stopping? Pause all the imports (status messages).
			//starting? Resume all the imports (status messages).
		}

		private ReceiveProgressItem GetReceiveProgressItem(ReceivedFileImportInformation receivedFileImportInformation, out bool exists)
		{
			ReceiveProgressItem progressItem = null;
			lock (_receiveProgressItemsLock)
			{
				progressItem = _receiveProgressItems.Find(
					delegate(ReceiveProgressItem testItem)
					{
						return testItem.FromAETitle == receivedFileImportInformation.SourceAETitle &&
							testItem.StudyInformation.StudyInstanceUid == receivedFileImportInformation.StudyInstanceUid;
					});

				exists = (progressItem != null);
				if (!exists)
				{
					progressItem = new ReceiveProgressItem();
					progressItem.Identifier = Guid.NewGuid();
					progressItem.AllowedCancellationOperations = CancellationFlags.Clear;
					progressItem.StartTime = DateTime.Now;
					progressItem.LastActive = progressItem.StartTime;

					progressItem.FromAETitle = receivedFileImportInformation.SourceAETitle;
					progressItem.NumberOfFilesReceived = 0;
					progressItem.NumberOfFilesImported = 0;
					progressItem.TotalFilesToImport = 0;
					progressItem.NumberOfFailedImports = 0; //not applicable, really, since we won't always know if it's not parseable.
					progressItem.StudyInformation = new StudyInformation();
					progressItem.StudyInformation.StudyInstanceUid = receivedFileImportInformation.StudyInstanceUid;
					progressItem.StudyInformation.PatientId = receivedFileImportInformation.PatientId;
					progressItem.StudyInformation.PatientsName = receivedFileImportInformation.PatientsName;
					progressItem.StudyInformation.StudyDescription = receivedFileImportInformation.StudyDescription;
					DateTime studyDate;
					DateParser.Parse(receivedFileImportInformation.StudyDate, out studyDate);
					progressItem.StudyInformation.StudyDate = studyDate;

					_receiveProgressItems.Add(progressItem);
				}
			}

			return progressItem;
		}

		private void ProcessFileImportResults(DicomFileImporter.FileImportInformation results)
		{
			ReceivedFileImportInformation receivedFileImportInformation = results as ReceivedFileImportInformation;

			if (results.CompletedStage == DicomFileImporter.ImportStage.NotStarted)
			{
				if (results.Failed)
				{ 
					//!!report generic error back to subscribers as there is no study information!!
					Platform.Log(results.Error);
				}

				return;
			}

			bool exists;
			bool updateProgress = false;

			ReceiveProgressItem progressItem = GetReceiveProgressItem(receivedFileImportInformation, out exists);
			if (!exists)
				updateProgress = true;

			lock(progressItem)
			{
				ImportedSopInstanceInformation importedSopInformation = null;

				progressItem.LastActive = DateTime.Now;

				if (receivedFileImportInformation.CompletedStage == DicomFileImporter.ImportStage.FileParsed)
				{
					if (results.Failed)
					{
						++progressItem.NumberOfFailedImports;
						Platform.Log(results.Error);
					}
					else
					{
						++progressItem.NumberOfFilesReceived;
						++progressItem.NumberOfFilesImported;
					}

					updateProgress = true;
				}
				else if (receivedFileImportInformation.CompletedStage == DicomFileImporter.ImportStage.FileMoved)
				{
					if (results.Failed)
					{
						++progressItem.NumberOfFailedImports;
						Platform.Log(results.Error);

						updateProgress = true;
					}
					else
					{
						//nothing to do.
					}
				}
				else
				{
					if (results.Failed)
					{
						++progressItem.NumberOfFailedImports;
						Platform.Log(results.Error);
					}
					else
					{
						if (!exists)
						{
							++progressItem.NumberOfFilesReceived;
							++progressItem.NumberOfFilesImported;
						}

						++progressItem.NumberOfFilesCommittedToDataStore;

						importedSopInformation = new ImportedSopInstanceInformation();
						importedSopInformation.StudyInstanceUid = receivedFileImportInformation.StudyInstanceUid;
						importedSopInformation.SeriesInstanceUid = receivedFileImportInformation.SeriesInstanceUid;
						importedSopInformation.SopInstanceUid = receivedFileImportInformation.SopInstanceUid;
						importedSopInformation.SopInstanceFileName = receivedFileImportInformation.StoredFile;
					}

					updateProgress = true;
				}

				if (importedSopInformation != null)
					LocalDataStoreActivityPublisher.Instance.SopInstanceImported(importedSopInformation);

				if (updateProgress)
					LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(progressItem.Clone());
			}
		}

		public void Start()
		{ 
			//nothing to do.
		}

		public void Stop()
		{
			//nothing to do.
		}

		internal void ProcessReceivedFileInformation(StoreScpReceivedFileInformation receivedFileInformation)
		{
			LocalDataStoreService.Instance.DicomFileImporter.Enqueue(new ReceivedFileImportInformation(receivedFileInformation.FileName, receivedFileInformation.AETitle), this.ProcessFileImportResults);
		}

		public void RepublishAll()
		{
			lock (_receiveProgressItemsLock)
			{
				foreach (ReceiveProgressItem item in _receiveProgressItems)
					LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(item.Clone());
			}
		}

		internal void Cancel(CancelProgressItemInformation information)
		{
			if (information.ProgressItemIdentifiers == null)
				return;

			if (information.CancellationFlags != CancellationFlags.Clear)
				return;

			lock (_receiveProgressItemsLock)
			{
				foreach(Guid identifier in information.ProgressItemIdentifiers)
				{
					ReceiveProgressItem item = _receiveProgressItems.Find(delegate(ReceiveProgressItem test) { return test.Identifier.Equals(identifier); });
					if (item != null)
					{
						item.Removed = true;
						LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(item.Clone());
					}
				}
			}
		}

		internal void ClearInactive()
		{
			//not supported.
		}
	}
}
