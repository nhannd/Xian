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

		public LocalDataStoreServiceClient(string endpointConfigurationName)
			:
				base(endpointConfigurationName)
		{
		}

		public LocalDataStoreServiceClient(string endpointConfigurationName, string remoteAddress)
			:
				base(endpointConfigurationName, remoteAddress)
		{
		}

		public LocalDataStoreServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
			:
				base(endpointConfigurationName, remoteAddress)
		{
		}

		public LocalDataStoreServiceClient(Binding binding, EndpointAddress remoteAddress)
			:
				base(binding, remoteAddress)
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