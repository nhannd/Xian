using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed class LocalDataStoreService : ILocalDataStoreService
	{
		private static LocalDataStoreService _instance;

		private LocalDataStoreService()
		{
		}

		public static LocalDataStoreService Instance
		{
			get
			{
				if (_instance == null)
					_instance = new LocalDataStoreService();

				return _instance;
			}
		}

		#region ILocalDataStoreService Members

		public void FilesReceived(StoreScpReceivedFilesInformation receivedFilesInformation)
		{
			
		}

		public void FilesSent(StoreScuSentFilesInformation sentFilesInformation)
		{
			throw new LocalDataStoreFaultException("The method or operation is not implemented.");
		}

		public void Import(FileImportRequest request)
		{
			throw new LocalDataStoreFaultException("The method or operation is not implemented.");
		}

		public void Reindex()
		{
			throw new LocalDataStoreFaultException("The method or operation is not implemented.");
		}

		#endregion
	}
}
