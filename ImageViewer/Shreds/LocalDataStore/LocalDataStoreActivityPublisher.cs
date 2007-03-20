using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ServiceModelEx;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal class LocalDataStoreActivityPublisher : TransientPublishService<ILocalDataStoreActivityMonitorServiceCallback>, ILocalDataStoreActivityMonitorServiceCallback
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
			FireEvent(progressItem);
		}

		public void SendProgressChanged(SendProgressItem progressItem)
		{
			FireEvent(progressItem);
		}

		public void ImportProgressChanged(ImportProgressItem progressItem)
		{
			FireEvent(progressItem);
		}

		public void ReindexProgressChanged(ReindexProgressItem progressItem)
		{
			FireEvent(progressItem);
		}

		public void SopInstancesImported(ImportedSopInstanceInformation information)
		{
			FireEvent(information);
		}

		public void ServiceStopped()
		{
			FireEvent(null);
		}

		#endregion
	}
}
