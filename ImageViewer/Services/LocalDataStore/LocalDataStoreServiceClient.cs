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

		public void FileReceived(StoreScpReceivedFileInformation receivedFileInformation)
		{
			base.Channel.FileReceived(receivedFileInformation);
		}

		public void FileSent(StoreScuSentFileInformation sentFileInformation)
		{
			base.Channel.FileSent(sentFileInformation);
		}

		public Guid Import(FileImportRequest request)
		{
			return base.Channel.Import(request);
		}

		public void Reindex()
		{
			base.Channel.Reindex();
		}
	}
}