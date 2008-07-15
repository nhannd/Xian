using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal class LocalDataStorePublishHelper
	{
		public static readonly LocalDataStorePublishHelper Instance = new LocalDataStorePublishHelper();

		private delegate void PublishCallback(ILocalDataStoreService client);
		private readonly SimpleBlockingThreadPool _pool;

		private LocalDataStorePublishHelper()
		{
			_pool = new SimpleBlockingThreadPool(Environment.ProcessorCount * 2);
		}

		public void Start()
		{
			_pool.Start();
		}

		public void Stop()
		{
			_pool.Stop();
		}

		private static void Publish(PublishCallback callback, string failureMessage)
		{
			LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();
			try
			{
				callback(client);
				client.Close();
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e, failureMessage);
				client.Abort();
			}
		}

		public void RetrieveStarted(RetrieveStudyInformation information)
		{
			_pool.Enqueue(delegate
					{
						Publish(delegate(ILocalDataStoreService client)
								{
									client.RetrieveStarted(information);
								}, "Failed to notify the local data store service that a retrieve operation has begun.");
					});
		}

		public void FileReceived(StoreScpReceivedFileInformation receivedFileInformation)
		{
			_pool.Enqueue(delegate
					{
						String failureMessage = String.Format(
							"Failed to notify local data store service that a file has been received ({0})." + 
							"  The file must be imported manually.", receivedFileInformation.FileName);

						Publish(delegate(ILocalDataStoreService client)
								{
									client.FileReceived(receivedFileInformation);
								}, failureMessage);
					});
		}

		public void ReceiveError(ReceiveErrorInformation errorInformation)
		{
			_pool.Enqueue(delegate
					{
						Publish(delegate(ILocalDataStoreService client)
								{
									client.ReceiveError(errorInformation);
								}, "Failed to notify the local data store service of a retrieve error.");
					});
		}

		public void SendStarted(SendStudyInformation information)
		{
			_pool.Enqueue(delegate
					{
						Publish(delegate(ILocalDataStoreService client)
								{
									client.SendStarted(information);
								}, "Failed to notify the local data store service that a send operation has begun.");
					});
		}

		public void FileSent(StoreScuSentFileInformation sentFileInformation)
		{
			_pool.Enqueue(delegate
					{
						string message = String.Format("Failed to notify local data store service that a file has been sent ({0}).  " +
						             "The file will need to be manually imported.", sentFileInformation.FileName);

						Publish(delegate(ILocalDataStoreService client)
								{
									client.FileSent(sentFileInformation);
								}, message);
					});
		}

		public void SendError(SendErrorInformation errorInformation)
		{
			_pool.Enqueue(delegate
					{
						Publish(delegate(ILocalDataStoreService client)
								{
									client.SendError(errorInformation);
								}, "Failed to notify local data store service that a send error occurred.");
					});
		}
	}
}
