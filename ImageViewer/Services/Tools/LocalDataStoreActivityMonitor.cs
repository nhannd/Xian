using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	public class ReceiveProgressUpdateEventArgs : EventArgs
	{
		private ReceiveProgressItem  _item;

		public ReceiveProgressUpdateEventArgs(ReceiveProgressItem item)
		{
			_item = item;
		}

		public ReceiveProgressItem  Item
		{
			get { return _item;}
		}
	}

	public sealed class LocalDataStoreActivityMonitor : ILocalDataStoreActivityMonitorServiceCallback
	{
		private LocalDataStoreActivityMonitorServiceClient _serviceClient;
		private event EventHandler<ReceiveProgressUpdateEventArgs> _receiveProgressUpdate;

		public LocalDataStoreActivityMonitor()
		{ 
		}

		public void Start()
		{
			InstanceContext context = new InstanceContext(this);
			_serviceClient = new LocalDataStoreActivityMonitorServiceClient(context);
			try
			{
				_serviceClient.Open();
				_serviceClient.Subscribe("");
			}
			catch
			{
				_serviceClient.Abort();
				throw;
			}
		}

		public void Stop()
		{ 
			try
			{
				_serviceClient.Unsubscribe("");
				_serviceClient.Close();
			}
			catch
			{
				_serviceClient.Abort();
				throw;
			}
		}

		public event EventHandler<ReceiveProgressUpdateEventArgs> ReceiveProgressUpdate
		{
			add { _receiveProgressUpdate += value; }
			remove { _receiveProgressUpdate -= value; }
		}

		#region ILocalDataStoreActivityMonitorServiceCallback Members

		public void ReceiveProgressChanged(ReceiveProgressItem progressItem)
		{
			EventsHelper.Fire(_receiveProgressUpdate,this, new ReceiveProgressUpdateEventArgs(progressItem));
		}

		public void SendProgressChanged(SendProgressItem progressItem)
		{
		}

		public void ImportProgressChanged(ImportProgressItem progressItem)
		{
		}

		public void ReindexProgressChanged(ReindexProgressItem progressItem)
		{
		}

		public void SopInstancesImported(ImportedSopInstanceInformation information)
		{
		}

		public void ServiceStopped()
		{
		}

		#endregion
	}
}
