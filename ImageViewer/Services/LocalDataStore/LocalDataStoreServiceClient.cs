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

		public void FilesReceived(StoreScpReceivedFilesInformation receivedFilesInformation)
		{
			base.Channel.FilesReceived(receivedFilesInformation);
		}

		public void FilesSent(StoreScuSentFilesInformation sentFilesInformation)
		{
			base.Channel.FilesSent(sentFilesInformation);
		}

		public void Import(FileImportRequest request)
		{
			base.Channel.Import(request);
		}

		public void Reindex()
		{
			base.Channel.Reindex();
		}
	}
}