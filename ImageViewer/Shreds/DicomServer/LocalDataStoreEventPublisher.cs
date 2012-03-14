#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.LocalDataStore;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal class LocalDataStoreEventPublisher
	{
		private class PublishThreadPool : BlockingThreadPool<object>
		{
			public PublishThreadPool()
				: base(1, false)
			{
			}

			private static void Publish(object item)
			{
				LocalDataStoreServiceClient proxy = new LocalDataStoreServiceClient();

				try
				{
					proxy.Open();

					if (item is StoreScpReceivedFileInformation)
					{
						proxy.FileReceived(item as StoreScpReceivedFileInformation);
					}
					else if (item is StoreScuSentFileInformation)
					{
						proxy.FileSent(item as StoreScuSentFileInformation);
					}
					if (item is RetrieveStudyInformation)
					{
						proxy.RetrieveStarted(item as RetrieveStudyInformation);
					}
					else if (item is SendStudyInformation)
					{
						proxy.SendStarted(item as SendStudyInformation);
					}
					else if (item is ReceiveErrorInformation)
					{
						proxy.ReceiveError(item as ReceiveErrorInformation);
					}
					else if (item is SendErrorInformation)
					{
						proxy.SendError(item as SendErrorInformation);
					}

					proxy.Close();
				}
				catch (Exception e)
				{
					proxy.Abort();
					LogPublishError(item, e);
				}
			}

			private static void LogPublishError(object item, Exception e)
			{
				if (item is RetrieveStudyInformation)
				{
					Platform.Log(LogLevel.Warn, e,
						"Failed to notify the local data store service that a retrieve operation has begun.");
				}
				else if (item is SendStudyInformation)
				{
					Platform.Log(LogLevel.Warn, e,
						"Failed to notify the local data store service that a send operation has begun.");
				}
				else if (item is StoreScpReceivedFileInformation)
				{
					StoreScpReceivedFileInformation receivedFileInformation = (StoreScpReceivedFileInformation)item;
					String message = String.Format(
						"Failed to notify local data store service that a file has been received ({0})." +
						"  The file must be imported manually.", receivedFileInformation.FileName);

					Platform.Log(LogLevel.Warn, e, message);
				}
				else if (item is StoreScuSentFileInformation)
				{
					StoreScuSentFileInformation sentFileInformation = (StoreScuSentFileInformation)item;
					string message =
						String.Format("Failed to notify local data store service that a file has been sent ({0}).",
						sentFileInformation.FileName);

					Platform.Log(LogLevel.Warn, e, message);
				}
				else if (item is SendErrorInformation)
				{
					Platform.Log(LogLevel.Warn, e,
						"Failed to notify local data store service that a send error occurred.");
				}
				else if (item is ReceiveErrorInformation)
				{
					Platform.Log(LogLevel.Warn, e,
						"Failed to notify local data store service that a receive error occurred.");
				}
			}

			protected override void ProcessItem(object item)
			{
				Publish(item);
			}
		}

		public static readonly LocalDataStoreEventPublisher Instance = new LocalDataStoreEventPublisher();

		private readonly PublishThreadPool _pool;

		private LocalDataStoreEventPublisher()
		{
			_pool = new PublishThreadPool();
		}

		private void AddToQueue(object item)
		{
			_pool.Enqueue(item);
		}

		public void Start()
		{
			_pool.Start();
		}

		public void Stop()
		{
			_pool.Stop();
		}

		public void FileSent(StoreScuSentFileInformation info)
		{
			AddToQueue(info);
		}

		public void FileReceived(StoreScpReceivedFileInformation info)
		{
			AddToQueue(info);
		}

		public void SendStarted(SendStudyInformation info)
		{
			AddToQueue(info);
		}

		public void RetrieveStarted(RetrieveStudyInformation info)
		{
			AddToQueue(info);
		}

		public void SendError(SendErrorInformation info)
		{
			AddToQueue(info);
		}

		public void ReceiveError(ReceiveErrorInformation info)
		{
			AddToQueue(info);
		}
	}
}
