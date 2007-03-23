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
	internal class ReceiveImportQueue
	{
		private class ReceivedFileImportInformation : DicomFileImporter.FileImportInformation
		{
			private string _sourceAETitle;

			public ReceivedFileImportInformation(string sourceFile, string sourceAETitle)
				: base(sourceFile, BadFileBehaviour.Move)
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

		public ReceiveImportQueue()
		{
			_receiveProgressItems = new List<ReceiveProgressItem>();
		}

		private void OnImportThreadPoolStartStop(object sender, StartStopEventArgs e)
		{
			//stopping? Pause all the imports (status messages).
			//starting? Resume all the imports (status messages).
		}

		private void ProcessFileImportResults(DicomFileImporter.FileImportInformation results)
		{
			ReceivedFileImportInformation receivedFileImportInformation = results as ReceivedFileImportInformation;

			if (results.Failed)
			{
				//!!report back to subscribers!!
				Platform.Log(results.Error);
			}
			else
			{
				if (results.CompletedStage == DicomFileImporter.ImportStage.NotStarted)
					return;

				lock (_receiveProgressItemsLock)
				{
					ReceiveProgressItem progressItem = _receiveProgressItems.Find(
						delegate(ReceiveProgressItem testItem)
						{
							return testItem.FromAETitle == receivedFileImportInformation.SourceAETitle &&
								testItem.StudyInformation.StudyInstanceUid == receivedFileImportInformation.StudyInstanceUid;
						});

					bool exists = (progressItem != null);
					if (!exists)
					{
						progressItem = new ReceiveProgressItem();
						progressItem.Identifier = Guid.NewGuid();
						progressItem.AllowedCancellationOperations = CancellationFlags.None;
						progressItem.StartTime = DateTime.Now;
						progressItem.LastActive = progressItem.StartTime;
						progressItem.State = FileOperationProgressItemState.InProgress;

						progressItem.FromAETitle = receivedFileImportInformation.SourceAETitle;
						progressItem.NumberOfFilesReceived = 0;
						progressItem.NumberOfFilesImported = 0;
						progressItem.TotalFilesToImport = 0;
						progressItem.NumberOfFailedImports = 0; //not applicable, really, since we won't always know if it's not parseable.
						progressItem.StudyInformation = new StudyInformation();
						progressItem.StudyInformation.StudyInstanceUid = results.StudyInstanceUid;
						progressItem.StudyInformation.PatientId = results.PatientId;
						progressItem.StudyInformation.PatientsName = results.PatientsName;
						progressItem.StudyInformation.StudyDescription = results.StudyDescription;
						DateTime studyDate;
						DateParser.Parse(results.StudyDate, out studyDate);
						progressItem.StudyInformation.StudyDate = studyDate;

						LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(progressItem);
						_receiveProgressItems.Add(progressItem);

					}

					progressItem.LastActive = DateTime.Now;

					if (receivedFileImportInformation.CompletedStage == DicomFileImporter.ImportStage.FileParsed)
					{
						++progressItem.NumberOfFilesReceived;
						LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(progressItem);
					}
					else if (receivedFileImportInformation.CompletedStage == DicomFileImporter.ImportStage.FileMoved)
					{
						//do nothing.
					}
					else
					{
						if (!exists)
							++progressItem.NumberOfFilesReceived;

						++progressItem.NumberOfFilesImported;
						LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(progressItem);
					}
				}
			}
		}

		public void RepublishAll()
		{
			lock (_receiveProgressItemsLock)
			{
				//remember to clone!
				foreach (ReceiveProgressItem item in _receiveProgressItems)
					LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(item);
			}
		}

		internal void ProcessReceivedFileInformation(StoreScpReceivedFileInformation receivedFileInformation)
		{
			LocalDataStoreService.Instance.DicomFileImporter.Enqueue(new ReceivedFileImportInformation(receivedFileInformation.FileName, receivedFileInformation.AETitle), this.ProcessFileImportResults);
		}
	}
}
