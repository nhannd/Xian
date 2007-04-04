using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ServiceModelEx;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed class LocalDataStoreActivityPublisher : TransientPublishService<ILocalDataStoreActivityMonitorServiceCallback>, ILocalDataStoreActivityMonitorServiceCallback
	{
		private static LocalDataStoreActivityPublisher _instance;

		private LocalDataStoreActivityPublisher()
		{
		}

		public static LocalDataStoreActivityPublisher Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LocalDataStoreActivityPublisher();
				}

				return _instance;
			}
		}

		#region ILocalDataStoreActivityMonitorServiceCallback Members

		public void ReceiveProgressChanged(ReceiveProgressItem progressItem)
		{
			FireEvent("ReceiveProgressChanged", progressItem);
		}

		public void SendProgressChanged(SendProgressItem progressItem)
		{
			FireEvent("SendProgressChanged", progressItem);
		}

		public void ImportProgressChanged(ImportProgressItem progressItem)
		{
			FireEvent("ImportProgressChanged", progressItem);
		}

		public void ReindexProgressChanged(ReindexProgressItem progressItem)
		{
			FireEvent("ReindexProgressChanged", progressItem);
		}

		public void SopInstanceImported(ImportedSopInstanceInformation information)
		{
			FireEvent("SopInstanceImported", information);
		}

		#endregion
	}
}
