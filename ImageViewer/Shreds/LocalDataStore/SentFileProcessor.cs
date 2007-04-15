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
	internal sealed partial class LocalDataStoreService
	{
		private class SentFileProcessor
		{
			private LocalDataStoreService _parent;
			private SimpleBlockingThreadPool _sentFileInformationProcessor;
			private List<SendProgressItem> _sendProgressItems;

			public SentFileProcessor(LocalDataStoreService parent)
			{
				_sentFileInformationProcessor = new SimpleBlockingThreadPool(1);
				_sendProgressItems = new List<SendProgressItem>();

				_parent = parent;
				_parent.StartEvent += new EventHandler(OnStart);
				_parent.StopEvent += new EventHandler(OnStop);
				_parent.CancelEvent += new EventHandler<ItemEventArgs<CancelProgressItemInformation>>(OnCancel);
				_parent.RepublishEvent += new EventHandler(OnRepublish);
			}

			private void OnStart(object sender, EventArgs e)
			{
				_sentFileInformationProcessor.Start();
			}
			private void OnStop(object sender, EventArgs e)
			{
				_sentFileInformationProcessor.Stop();
			}

			private void OnRepublish(object sender, EventArgs e)
			{
				//use the same thread for everything, then no synclock is required on the progress items.
				_sentFileInformationProcessor.Enqueue
					(
						delegate()
						{
							foreach (SendProgressItem item in _sendProgressItems)
								LocalDataStoreActivityPublisher.Instance.SendProgressChanged(item);
						}
					);
			}

			private void OnCancel(object sender, ItemEventArgs<CancelProgressItemInformation> e)
			{
				CancelProgressItemInformation information = e.Item;

				if (information.ProgressItemIdentifiers == null)
					return;

				if (information.CancellationFlags != CancellationFlags.Clear)
					return;

				_sentFileInformationProcessor.Enqueue
					(
						delegate()
						{
							foreach (Guid identifier in information.ProgressItemIdentifiers)
							{
								SendProgressItem item = _sendProgressItems.Find(delegate(SendProgressItem test) { return test.Identifier.Equals(identifier); });
								if (item != null)
								{
									_sendProgressItems.Remove(item);

									item.Removed = true;
									LocalDataStoreActivityPublisher.Instance.SendProgressChanged(item.Clone());
								}
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

					condition = dataset.findAndGetOFString(Dcm.PatientId, parser);
					if (condition.good())
						information.PatientId = parser.ToString();

					condition = dataset.findAndGetOFString(Dcm.PatientsName, parser);
					if (condition.good()) 
						information.PatientsName = parser.ToString();

					condition = dataset.findAndGetOFString(Dcm.StudyDate, parser);
					if (condition.good())
					{
						DateTime studyDate;
						DateParser.Parse(parser.ToString(), out studyDate);
						information.StudyDate = studyDate;
					}

					condition = dataset.findAndGetOFString(Dcm.StudyDescription, parser);
					if (condition.good())
						information.StudyDescription = parser.ToString();

					condition = dataset.findAndGetOFString(Dcm.StudyInstanceUID, parser);
					if (condition.good()) 
						information.StudyInstanceUid = parser.ToString();

					return information;
				}
			}

			public void ProcessSentFileInformation(StoreScuSentFileInformation sentFileInformation)
			{
				//always leave the main thread free to accept incoming requests.
				_sentFileInformationProcessor.Enqueue
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
		}
	}
}