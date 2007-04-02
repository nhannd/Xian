using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Common;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal class SendQueue
	{
		private SimpleThreadPool _sentFileInformationProcessor;
		private List<SendProgressItem> _sendProgressItems;

		public SendQueue()
		{
			_sentFileInformationProcessor = new SimpleThreadPool(1);
			_sendProgressItems = new List<SendProgressItem>();
		}

		public void ProcessSentFileInformation(StoreScuSentFileInformation sentFileInformation)
		{
			//always leave the main thread free to accept incoming requests.
			_sentFileInformationProcessor.AddEnd
				(
					delegate()
					{
						try
						{
							StudyInformation studyInformation = GetStudyInformation(sentFileInformation.FileName);

							//no synclock required, since only the single thread pool thread is accessing the items.
							SendProgressItem progressItem = _sendProgressItems.Find(delegate(SendProgressItem testItem)
								{
									return testItem.ToAETitle == sentFileInformation.ToAETitle &&
										testItem.StudyInformation.StudyInstanceUid == studyInformation.StudyInstanceUid;
								});

							if (progressItem == null)
							{
								progressItem = new SendProgressItem();
								progressItem.Identifier = Guid.NewGuid();
								progressItem.StartTime = DateTime.Now;
								progressItem.LastActive = progressItem.StartTime;
								progressItem.ToAETitle = sentFileInformation.ToAETitle;
								progressItem.StudyInformation = studyInformation;
								progressItem.NumberOfFilesExported = 0;
								progressItem.AllowedCancellationOperations = CancellationFlags.Clear;

								_sendProgressItems.Add(progressItem);
							}

							progressItem.LastActive = DateTime.Now;
							++progressItem.NumberOfFilesExported;

							LocalDataStoreActivityPublisher.Instance.SendProgressChanged(progressItem);
						}
						catch (Exception e)
						{
							//!!Report generic error to the subscribers.!!
							Platform.Log(e);
						}
					}
				);
		}

		private StudyInformation GetStudyInformation(string sopInstanceFilename)
		{
			using (DcmFileFormat file = new DcmFileFormat())
			{
				OFCondition condition = file.loadFile(sopInstanceFilename);

				if (!condition.good())
					throw new Exception(String.Format(SR.FormatUnableToParseFile, sopInstanceFilename));

				DcmMetaInfo metaInfo = file.getMetaInfo();
				DcmDataset dataset = file.getDataset();

				StudyInformation information = new StudyInformation();
				StringBuilder parser = new StringBuilder(1024);

				dataset.findAndGetOFString(Dcm.PatientId, parser);
				information.PatientId = parser.ToString();

				dataset.findAndGetOFString(Dcm.PatientsName, parser);
				information.PatientsName = parser.ToString();

				dataset.findAndGetOFString(Dcm.StudyDate, parser);
				DateTime studyDate;
				DateParser.Parse(parser.ToString(), out studyDate);
				information.StudyDate = studyDate;

				dataset.findAndGetOFString(Dcm.StudyDescription, parser);
				information.StudyDescription = parser.ToString();

				dataset.findAndGetOFString(Dcm.StudyInstanceUID, parser);
				information.StudyInstanceUid = parser.ToString();

				return information;
			}
		}

		public void Start()
		{
			_sentFileInformationProcessor.Start();
		}

		public void Stop()
		{
			_sentFileInformationProcessor.Stop();
		}

		public void RepublishAll()
		{
			//use the same thread for everything, then no synclock is required on the progress items.
			_sentFileInformationProcessor.AddFront
				(
					delegate()
					{
						foreach (SendProgressItem item in _sendProgressItems)
							LocalDataStoreActivityPublisher.Instance.SendProgressChanged(item);
					}
				);
		}

		internal void Cancel(CancelProgressItemInformation information)
		{
			if (information.ProgressItemIdentifiers == null)
				return;
			
			if (information.CancellationFlags != CancellationFlags.Clear)
				return;

			_sentFileInformationProcessor.AddFront
				(
					delegate()
					{
						foreach (Guid identifier in information.ProgressItemIdentifiers)
						{
							SendProgressItem item = _sendProgressItems.Find(delegate(SendProgressItem test) { return test.Identifier.Equals(identifier); });
							if (item != null)
							{
								item.Removed = true;
								LocalDataStoreActivityPublisher.Instance.SendProgressChanged(item.Clone());
							}
						}
					}
				);
		}

		internal void ClearInactive()
		{
			//not supported.
		}
	}
}
