#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.LocalDataStore
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