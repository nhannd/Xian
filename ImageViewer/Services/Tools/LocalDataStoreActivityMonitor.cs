using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using System.ServiceModel;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	public sealed class LocalDataStoreActivityMonitor : ILocalDataStoreActivityMonitorServiceCallback
	{
		private LocalDataStoreActivityMonitorServiceClient _serviceClient;
		private event EventHandler<ItemEventArgs<ReceiveProgressItem>> _receiveProgressUpdate;
		private event EventHandler<ItemEventArgs<SendProgressItem>> _sendProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportProgressItem>> _importProgressUpdate;
		private event EventHandler<ItemEventArgs<ReindexProgressItem>> _reindexProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportedSopInstanceInformation>> _sopInstanceImported;
		private event EventHandler _serviceStopped;

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

		public event EventHandler<ItemEventArgs<SendProgressItem>> SendProgressUpdate
		{
			add { _sendProgressUpdate += value; }
			remove { _sendProgressUpdate -= value; }
		}

		public event EventHandler<ItemEventArgs<ReceiveProgressItem>> ReceiveProgressUpdate
		{
			add { _receiveProgressUpdate += value; }
			remove { _receiveProgressUpdate -= value; }
		}

		public event EventHandler<ItemEventArgs<ImportProgressItem>> ImportProgressUpdate
		{
			add { _importProgressUpdate += value; }
			remove { _importProgressUpdate -= value; }
		}

		public event EventHandler<ItemEventArgs<ReindexProgressItem>> ReindexProgressUpdate
		{
			add { _reindexProgressUpdate += value; }
			remove { _reindexProgressUpdate -= value; }
		}

		public event EventHandler ServiceStopped
		{
			add { _serviceStopped += value; }
			remove { _serviceStopped -= value; }
		}

		#region ILocalDataStoreActivityMonitorServiceCallback Members

		public void ReceiveProgressChanged(ReceiveProgressItem progressItem)
		{
			EventsHelper.Fire(_receiveProgressUpdate,this, new ItemEventArgs<ReceiveProgressItem>(progressItem));
		}

		public void SendProgressChanged(SendProgressItem progressItem)
		{
			EventsHelper.Fire(_sendProgressUpdate, this, new ItemEventArgs<SendProgressItem>(progressItem));
		}

		public void ImportProgressChanged(ImportProgressItem progressItem)
		{
			EventsHelper.Fire(_importProgressUpdate, this, new ItemEventArgs<ImportProgressItem>(progressItem));
		}

		public void ReindexProgressChanged(ReindexProgressItem progressItem)
		{
			EventsHelper.Fire(_reindexProgressUpdate, this, new ItemEventArgs<ReindexProgressItem>(progressItem));
		}

		public void SopInstanceImported(ImportedSopInstanceInformation information)
		{
			EventsHelper.Fire(_sopInstanceImported, this, new ItemEventArgs<ImportedSopInstanceInformation>(information));
		}

		public void OnServiceStopped()
		{
			EventsHelper.Fire(_serviceStopped, this, EventArgs.Empty);
		}

		#endregion
	}
}
