#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Common.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed class LocalDataStoreActivityPublisher : ILocalDataStoreActivityMonitorServiceCallback
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
			PublishManager<ILocalDataStoreActivityMonitorServiceCallback>.Publish("ReceiveProgressChanged", progressItem);
		}

		public void SendProgressChanged(SendProgressItem progressItem)
		{
			PublishManager<ILocalDataStoreActivityMonitorServiceCallback>.Publish("SendProgressChanged", progressItem);
		}

		public void ImportProgressChanged(ImportProgressItem progressItem)
		{
			PublishManager<ILocalDataStoreActivityMonitorServiceCallback>.Publish("ImportProgressChanged", progressItem);
		}

		public void ReindexProgressChanged(ReindexProgressItem progressItem)
		{
			PublishManager<ILocalDataStoreActivityMonitorServiceCallback>.Publish("ReindexProgressChanged", progressItem);
		}

		public void SopInstanceImported(ImportedSopInstanceInformation information)
		{
			PublishManager<ILocalDataStoreActivityMonitorServiceCallback>.Publish("SopInstanceImported", information);
		}

		public void InstanceDeleted(DeletedInstanceInformation information)
		{
			PublishManager<ILocalDataStoreActivityMonitorServiceCallback>.Publish("InstanceDeleted", information);
		}

		public void LocalDataStoreCleared()
		{
			PublishManager<ILocalDataStoreActivityMonitorServiceCallback>.Publish("LocalDataStoreCleared");
		}

		#endregion
	}
}
