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
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.LocalDataStore;
using ClearCanvas.ImageViewer.Services;
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
				_parent.PurgeEvent += OnPurge;
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

			private void OnPurge(object sender, EventArgs e)
			{
				//use the same thread for everything, then no synclock is required on the progress items.
				_sentFileInformationProcessor.Enqueue(delegate { Purge(); });
			}

			private void Purge()
			{
				DateTime now = DateTime.Now;
				TimeSpan timeLimit = TimeSpan.FromMinutes(LocalDataStoreServiceSettings.Instance.PurgeTimeMinutes);

				List<SendProgressItem> clearItems = new List<SendProgressItem>();
				foreach (SendProgressItem item in _sendProgressItems)
				{
					bool isOld = now.Subtract(item.LastActive) > timeLimit;
					bool hasErrors = !String.IsNullOrEmpty(item.StatusMessage);

					if (isOld && !hasErrors)
						clearItems.Add(item);
				}

				foreach (SendProgressItem item in clearItems)
				{
					_sendProgressItems.Remove(item);
					item.Removed = true;
					LocalDataStoreActivityPublisher.Instance.SendProgressChanged(item.Clone());
				}
			}

			private void OnRepublish(object sender, EventArgs e)
			{
				//use the same thread for everything, then no synclock is required on the progress items.
				_sentFileInformationProcessor.Enqueue
					(
						delegate()
						{
							foreach (SendProgressItem item in _sendProgressItems)
							{
								var republishItem = item.Clone();
								republishItem.MessageType = MessageType.Republish;
								LocalDataStoreActivityPublisher.Instance.SendProgressChanged(republishItem);
							}
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

			private StudyInformation GetStudyInformation(StoreScuSentFileInformation information)
			{
				//If the study information is already specified, then don't parse the file.
				if (information.StudyInformation != null)
					return information.StudyInformation;

				DicomFile dicomFile = new DicomFile(information.FileName);
				try
				{
					dicomFile.Load(DicomTags.PixelData, DicomReadOptions.Default);
				}
				catch (Exception e)
				{
					throw new Exception(String.Format(SR.FormatUnableToParseFile, information.FileName), e);
				}

				StudyInformationFieldExchanger studyInformation = new StudyInformationFieldExchanger();
				dicomFile.DataSet.LoadDicomFields(studyInformation);
				return studyInformation;
			}

			private SendProgressItem GetProgressItem(SendOperationReference reference, string toAETitle, StudyInformation studyInformation)
			{
				//no synclock required, since only the single thread pool thread is accessing the items.
				SendProgressItem progressItem = _sendProgressItems.Find(delegate(SendProgressItem testItem)
					{
						if (reference == null)
						{
							// should never get here, but we'll create one progress item per 
							// ae title/study uid pair.
							return testItem.SendOperationReference == null && testItem.ToAETitle == toAETitle &&
							testItem.StudyInformation.StudyInstanceUid == studyInformation.StudyInstanceUid;
						}
						else
						{
							//we want one progress item per send operation/study uid pair.
							return testItem.SendOperationReference == reference
							       && testItem.StudyInformation.StudyInstanceUid == studyInformation.StudyInstanceUid;
						}
					});

				if (progressItem == null)
				{
					progressItem = new SendProgressItem();
					progressItem.SendOperationReference = reference;
					if (reference != null)
						progressItem.IsBackground = reference.IsBackground;

					progressItem.Identifier = Guid.NewGuid();

					//TODO (Time Review): Change this back to use Platform.Time once we've resolved
					//the exception throwing issue.
					progressItem.StartTime = DateTime.Now;
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
								StudyInformation studyInformation = GetStudyInformation(sentFileInformation);
								SendProgressItem progressItem = GetProgressItem(sentFileInformation.SendOperationReference, sentFileInformation.ToAETitle, studyInformation);
								progressItem.SendOperationReference = sentFileInformation.SendOperationReference;
								progressItem.StudyInformation = studyInformation;

								if (progressItem.StatusMessage == SR.MessagePending)
									progressItem.StatusMessage = "";

								progressItem.LastActive = DateTime.Now;
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
							SendProgressItem progressItem = GetProgressItem(information.SendOperationReference, information.ToAETitle, information.StudyInformation);
							progressItem.StudyInformation = information.StudyInformation;
							progressItem.LastActive = DateTime.Now; 
							progressItem.StatusMessage = SR.MessagePending;
							progressItem.SendOperationReference = information.SendOperationReference;
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
							SendProgressItem progressItem = GetProgressItem(errorInformation.SendOperationReference, errorInformation.ToAETitle, errorInformation.StudyInformation);
							progressItem.StudyInformation = errorInformation.StudyInformation;
							progressItem.LastActive = DateTime.Now;
							progressItem.StatusMessage = errorInformation.ErrorMessage;
							progressItem.SendOperationReference = errorInformation.SendOperationReference;
							progressItem.HasErrors = true;
							LocalDataStoreActivityPublisher.Instance.SendProgressChanged(progressItem.Clone());
						}
					);
			}
		}
	}
}