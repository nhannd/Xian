using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Services;

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
								LocalDataStoreActivityPublisher.Instance.SendProgressChanged(item.Clone());
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
				DicomFile dicomFile = new DicomFile(sopInstanceFilename);
				try
				{
					dicomFile.Load(DicomTags.PixelData, DicomReadOptions.Default);
				}
				catch (Exception e)
				{
					throw new Exception(String.Format(SR.FormatUnableToParseFile, sopInstanceFilename), e);
				}

				StudyInformationFieldExchanger information = new StudyInformationFieldExchanger();
				dicomFile.DataSet.LoadDicomFields(information);
				return information;
			}

			private SendProgressItem GetProgressItem(string toAETitle, StudyInformation studyInformation)
			{
				//no synclock required, since only the single thread pool thread is accessing the items.
				SendProgressItem progressItem = _sendProgressItems.Find(delegate(SendProgressItem testItem)
					{
						return testItem.ToAETitle == toAETitle &&
							testItem.StudyInformation.StudyInstanceUid == studyInformation.StudyInstanceUid;
					});

				if (progressItem == null)
				{
					progressItem = new SendProgressItem();
					progressItem.Identifier = Guid.NewGuid();
					progressItem.StartTime = Platform.Time;
					progressItem.LastActive = progressItem.StartTime;
					progressItem.ToAETitle = toAETitle;
					progressItem.StudyInformation = studyInformation;
					progressItem.NumberOfFilesExported = 0;
					progressItem.AllowedCancellationOperations = CancellationFlags.Clear;

					_sendProgressItems.Add(progressItem);
				}

				return progressItem;
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
								SendProgressItem progressItem = GetProgressItem(sentFileInformation.ToAETitle, studyInformation);
								progressItem.StudyInformation = studyInformation;

								if (progressItem.StatusMessage == SR.MessagePending)
									progressItem.StatusMessage = "";

								progressItem.LastActive = Platform.Time;
								++progressItem.NumberOfFilesExported;
								LocalDataStoreActivityPublisher.Instance.SendProgressChanged(progressItem.Clone());
							}
							catch (Exception e)
							{
								Platform.Log(LogLevel.Error, e); //this only happens if we can't parse the file, so not much we can do.
							}
						}
					);
			}

			internal void SendStarted(SendStudyInformation information)
			{
				//always leave the main thread free to accept incoming requests.
				_sentFileInformationProcessor.Enqueue
					(
						delegate()
						{
							SendProgressItem progressItem = GetProgressItem(information.ToAETitle, information.StudyInformation);
							progressItem.StudyInformation = information.StudyInformation;
							progressItem.LastActive = Platform.Time; 
							progressItem.StatusMessage = SR.MessagePending;
							LocalDataStoreActivityPublisher.Instance.SendProgressChanged(progressItem.Clone());
						}
					);
			}
			
			public void SendError(SendErrorInformation errorInformation)
			{
				//always leave the main thread free to accept incoming requests.
				_sentFileInformationProcessor.Enqueue
					(
						delegate()
						{
							SendProgressItem progressItem = GetProgressItem(errorInformation.ToAETitle, errorInformation.StudyInformation);
							progressItem.StudyInformation = errorInformation.StudyInformation;
							progressItem.LastActive = Platform.Time;
							progressItem.StatusMessage = errorInformation.ErrorMessage;
							LocalDataStoreActivityPublisher.Instance.SendProgressChanged(progressItem.Clone());
						}
					);
			}
		}
	}
}