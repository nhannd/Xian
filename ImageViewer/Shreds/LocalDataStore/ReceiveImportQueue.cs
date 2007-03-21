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
		private delegate void FileImportResultDelegate(FileImportInformation results);

		private class FileImportJob : IJob
		{
			private string _sourceAETitle;
			private string _file;
			private FileImportResultDelegate _resultsCallbackDelegate;

			public FileImportJob(string file, string sourceAETitle, FileImportResultDelegate resultsCallbackDelegate)
			{
				_file = file;
				_sourceAETitle = sourceAETitle;
				_resultsCallbackDelegate = resultsCallbackDelegate;
			}

			#region IJob Members

			public void Execute()
			{
				ReceivedFileImportInformation results = new ReceivedFileImportInformation(_file, _sourceAETitle);

				LocalDataStoreService.Instance.DicomFileImporter.InsertSopInstance(results);

				_resultsCallbackDelegate(results);
			}

			#endregion
		}

		private class ReceivedFileImportInformation : FileImportInformation
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
		private SimpleThreadPool<IJob> _importThreadPool;

		public ReceiveImportQueue()
		{
			_receiveProgressItems = new List<ReceiveProgressItem>();
			_importThreadPool = new SimpleThreadPool<IJob>(LocalDataStoreService.Instance.SendReceiveImportConcurrency);
		}

		private SimpleThreadPool<IJob> ImportThreadPool
		{
			get { return _importThreadPool; }
		}

		private void OnImportThreadPoolStartStop(object sender, StartStopEventArgs e)
		{
			//stopping? Pause all the imports (status messages).
			//starting? Resume all the imports (status messages).
		}

		private void ProcessFileImportResults(FileImportInformation results)
		{
			ReceivedFileImportInformation receivedFileImportInformation = results as ReceivedFileImportInformation;

			if (results.Failed)
			{
				Platform.Log(results.Error);
			}
			else
			{
				lock (_receiveProgressItemsLock)
				{
					ReceiveProgressItem progressItem = _receiveProgressItems.Find(
						delegate(ReceiveProgressItem testItem)
						{
							return testItem.FromAETitle == receivedFileImportInformation.SourceAETitle &&
								testItem.StudyInformation.StudyInstanceUid == receivedFileImportInformation.StudyInstanceUid;
						});

					if (progressItem == null)
					{
						progressItem = new ReceiveProgressItem();
						progressItem.Identifier = Guid.NewGuid();
						progressItem.AllowedCancellationOperations = CancellationFlags.None;
						progressItem.StartTime = DateTime.Now;
						progressItem.LastActive = progressItem.StartTime;
						progressItem.State = FileOperationProgressItemState.InProgress;

						progressItem.FromAETitle = receivedFileImportInformation.SourceAETitle;
						progressItem.NumberOfFilesImported = 1;
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
					else
					{
						progressItem.LastActive = DateTime.Now;
						++progressItem.NumberOfFilesImported;
						LocalDataStoreActivityPublisher.Instance.ReceiveProgressChanged(progressItem);
					}
				}
			}
		}

		public void Start()
		{
			_importThreadPool.Start();
			_importThreadPool.StartStopEvent += new EventHandler<StartStopEventArgs>(OnImportThreadPoolStartStop);
		}

		public void Stop()
		{
			_importThreadPool.StartStopEvent -= new EventHandler<StartStopEventArgs>(OnImportThreadPoolStartStop);
			_importThreadPool.Stop();
		}

		public void ProcessReceivedFileInformation(StoreScpReceivedFileInformation receivedFileInformation)
		{
			_importThreadPool.Push(new FileImportJob(receivedFileInformation.FileName, receivedFileInformation.AETitle, this.ProcessFileImportResults));
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
	}
}
