using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class LocalDataStoreServiceType : ILocalDataStoreService
	{
		public LocalDataStoreServiceType()
		{
			Platform.Log("[" + AppDomain.CurrentDomain.FriendlyName + "]: LocalDataStoreServiceType Constructor");
		}


		#region ILocalDataStoreService Members

		public void FileReceived(StoreScpReceivedFileInformation filesReceivedInformation)
		{
			try
			{
				LocalDataStoreService.Instance.FileReceived(filesReceivedInformation);
			}
			catch (Exception e)
			{
				string message = String.Format("An error has occurred while attempting to process a received file ({0})", filesReceivedInformation.FileName);
				throw new LocalDataStoreFaultException(message, e);
			}
		}

		public void FileSent(StoreScuSentFileInformation filesSentInformation)
		{
			try
			{
				LocalDataStoreService.Instance.FileSent(filesSentInformation);
			}
			catch (Exception e)
			{
				string message = "An error has occurred while attempting to process the sent file information.";
				throw new LocalDataStoreFaultException(message, e);
			}
		}

		public void Import(FileImportRequest request)
		{
			try
			{
				LocalDataStoreService.Instance.Import(request);
			}
			catch (Exception e)
			{
				string message = "An error has occurred while attempting to process the file import request.";
				throw new LocalDataStoreFaultException(message, e);
			}
		}

		public void Reindex()
		{
			try
			{
				LocalDataStoreService.Instance.Reindex();
			}
			catch (Exception e)
			{
				string message = "An error has occurred while attempting to process the file import request.";
				throw new LocalDataStoreFaultException(message, e);
			}
		}

		#endregion
	}
}
