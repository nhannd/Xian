using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	public class LocalDataStoreServiceClient : ClientBase<ILocalDataStoreService>, ILocalDataStoreService
	{
		public LocalDataStoreServiceClient()
		{
		}

		#region ILocalDataStoreService Members

		public void RetrieveStarted(RetrieveStudyInformation information)
		{
			base.Channel.RetrieveStarted(information);
		}

		public void FileReceived(StoreScpReceivedFileInformation receivedFileInformation)
		{
			base.Channel.FileReceived(receivedFileInformation);
		}

		public void ReceiveError(ReceiveErrorInformation errorInformation)
		{
			base.Channel.ReceiveError(errorInformation);
		}

		public void SendStarted(SendStudyInformation information)
		{
			base.Channel.SendStarted(information);
		}

		public void FileSent(StoreScuSentFileInformation sentFileInformation)
		{
			base.Channel.FileSent(sentFileInformation);
		}

		public void SendError(SendErrorInformation errorInformation)
		{
			base.Channel.SendError(errorInformation);
		}

		public LocalDataStoreServiceConfiguration GetConfiguration()
		{
			return base.Channel.GetConfiguration();
		}

		public void DeleteInstances(DeleteInstancesRequest request)
		{
			base.Channel.DeleteInstances(request);
		}

		public Guid Import(FileImportRequest request)
		{
			return base.Channel.Import(request);
		}

		public void Reindex()
		{
			base.Channel.Reindex();
		}

		#endregion
	}
}