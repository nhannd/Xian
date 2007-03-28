using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using System.ServiceModel;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[CallbackBehavior(UseSynchronizationContext = false)]
	public sealed class LocalDataStoreActivityMonitor : ILocalDataStoreActivityMonitorServiceCallback
	{
		private LocalDataStoreActivityMonitorServiceClient _serviceClient;
		private event EventHandler<ItemEventArgs<ReceiveProgressItem>> _receiveProgressUpdate;
		private event EventHandler<ItemEventArgs<SendProgressItem>> _sendProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportProgressItem>> _importProgressUpdate;
		private event EventHandler<ItemEventArgs<ReindexProgressItem>> _reindexProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportedSopInstanceInformation>> _sopInstanceImported;
		private event EventHandler _serviceStopped;
		private InterthreadMarshaler _marshaler;

		public LocalDataStoreActivityMonitor()
		{
		}

		public void Start()
		{
			InstanceContext context = new InstanceContext(this);
			_serviceClient = new LocalDataStoreActivityMonitorServiceClient(context);
			try
			{
				_marshaler = new InterthreadMarshaler();

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
			finally
			{
				_marshaler.Dispose();
				_marshaler = null;
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
			_marshaler.QueueInvoke
			(delegate()
				{
					EventsHelper.Fire(_receiveProgressUpdate, this, new ItemEventArgs<ReceiveProgressItem>(progressItem));
				});
		}

		public void SendProgressChanged(SendProgressItem progressItem)
		{
			_marshaler.QueueInvoke
			(delegate()
				{
					EventsHelper.Fire(_sendProgressUpdate, this, new ItemEventArgs<SendProgressItem>(progressItem));
				});
		}

		public void ImportProgressChanged(ImportProgressItem progressItem)
		{
			_marshaler.QueueInvoke
			(delegate()
				{
					EventsHelper.Fire(_importProgressUpdate, this, new ItemEventArgs<ImportProgressItem>(progressItem));
				});
		}

		public void ReindexProgressChanged(ReindexProgressItem progressItem)
		{
			_marshaler.QueueInvoke
			(delegate()
				{
					EventsHelper.Fire(_reindexProgressUpdate, this, new ItemEventArgs<ReindexProgressItem>(progressItem));
				});
		}

		public void SopInstanceImported(ImportedSopInstanceInformation information)
		{
			_marshaler.QueueInvoke
			(delegate()
			{
				EventsHelper.Fire(_sopInstanceImported, this, new ItemEventArgs<ImportedSopInstanceInformation>(information));
			});
		}

		public void OnServiceStopped()
		{
			_marshaler.QueueInvoke
			(delegate()
			{
				EventsHelper.Fire(_serviceStopped, this, EventArgs.Empty);
			});
		}

		#endregion
    }
}
