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
				string message = String.Format(SR.ExceptionErrorProcessingReceivedFile, filesReceivedInformation.FileName);
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreFaultException(exceptionMessage);
			}
		}

		public void FileSent(StoreScuSentFileInformation sentFileInformation)
		{
			try
			{
				LocalDataStoreService.Instance.FileSent(sentFileInformation);
			}
			catch (Exception e)
			{
				string message = String.Format(SR.ExceptionErrorProcessingSentFile, sentFileInformation.FileName);
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreFaultException(exceptionMessage);
			}
		}

		public Guid Import(FileImportRequest request)
		{
			try
			{
				return LocalDataStoreService.Instance.Import(request);
			}
			catch (Exception e)
			{
				string message = SR.ExceptionErrorProcessingImportRequest;
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreFaultException(exceptionMessage);
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
				string message = SR.ExceptionErrorProcessingReindexRequest;
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreFaultException(exceptionMessage);
			}
		}

		#endregion
	}
}
