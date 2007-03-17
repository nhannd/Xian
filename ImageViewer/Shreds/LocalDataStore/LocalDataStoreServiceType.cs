using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
	public class LocalDataStoreServiceType : ILocalDataStoreService
	{
		public LocalDataStoreServiceType()
		{
			Platform.Log("[" + AppDomain.CurrentDomain.FriendlyName + "]: LocalDataStoreServiceType Constructor");
		}


		#region ILocalDataStoreService Members

		public void FilesReceived(StoreScpReceivedFilesInformation filesReceivedInformation)
		{
			try
			{
				LocalDataStoreService.Instance.FilesReceived(filesReceivedInformation);
			}
			catch (Exception e)
			{ 
			
			}
		}

		public void FilesSent(StoreScuSentFilesInformation filesSentInformation)
		{
			LocalDataStoreService.Instance.FilesSent(filesSentInformation);
		}

		public void Import(FileImportRequest request)
		{
			LocalDataStoreService.Instance.Import(request);
		}

		public void Reindex()
		{
			LocalDataStoreService.Instance.Reindex();
		}

		#endregion
	}
}
