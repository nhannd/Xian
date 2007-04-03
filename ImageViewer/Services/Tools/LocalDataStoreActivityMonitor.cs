using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using System.ServiceModel;
using System.ComponentModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	public sealed class LocalDataStoreActivityMonitor : IDisposable
	{
		[CallbackBehavior(UseSynchronizationContext = false)]
		private class LocalDataStoreActivityMonitorServiceCallback : ILocalDataStoreActivityMonitorServiceCallback, IDisposable
		{
			private LocalDataStoreActivityMonitor _parent;
			private InterthreadMarshaler _marshaler;

			public LocalDataStoreActivityMonitorServiceCallback(LocalDataStoreActivityMonitor parent)
			{
				_parent = parent;
				_marshaler = new InterthreadMarshaler();
			}

			#region ILocalDataStoreActivityMonitorServiceCallback Members

			public void ReceiveProgressChanged(ReceiveProgressItem progressItem)
			{
				_marshaler.QueueInvoke
				(delegate()
					{
						EventsHelper.Fire(_parent._receiveProgressUpdate, _parent, new ItemEventArgs<ReceiveProgressItem>(progressItem));
					});
			}

			public void SendProgressChanged(SendProgressItem progressItem)
			{
				_marshaler.QueueInvoke
				(delegate()
					{
						EventsHelper.Fire(_parent._sendProgressUpdate, _parent, new ItemEventArgs<SendProgressItem>(progressItem));
					});
			}

			public void ImportProgressChanged(ImportProgressItem progressItem)
			{
				_marshaler.QueueInvoke
				(delegate()
					{
						EventsHelper.Fire(_parent._importProgressUpdate, _parent, new ItemEventArgs<ImportProgressItem>(progressItem));
					});
			}

			public void ReindexProgressChanged(ReindexProgressItem progressItem)
			{
				_marshaler.QueueInvoke
				(delegate()
					{
						EventsHelper.Fire(_parent._reindexProgressUpdate, _parent, new ItemEventArgs<ReindexProgressItem>(progressItem));
					});
			}

			public void SopInstanceImported(ImportedSopInstanceInformation information)
			{
				_marshaler.QueueInvoke
				(delegate()
				{
					EventsHelper.Fire(_parent._sopInstanceImported, _parent, new ItemEventArgs<ImportedSopInstanceInformation>(information));
				});
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				_marshaler.Dispose();
				_marshaler = null;
			}

			#endregion
		}

		private static LocalDataStoreActivityMonitor _instance;

		private LocalDataStoreActivityMonitorServiceCallback _callback;
		private LocalDataStoreActivityMonitorServiceClient _serviceClient;

		private event EventHandler<ItemEventArgs<ReceiveProgressItem>> _receiveProgressUpdate;
		private event EventHandler<ItemEventArgs<SendProgressItem>> _sendProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportProgressItem>> _importProgressUpdate;
		private event EventHandler<ItemEventArgs<ReindexProgressItem>> _reindexProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportedSopInstanceInformation>> _sopInstanceImported;
		private event EventHandler _lostConnection;
		private event EventHandler _connected;

		public LocalDataStoreActivityMonitor()
		{
		}

		public static LocalDataStoreActivityMonitor Instance
		{
			get 
			{
				if (_instance == null)
					_instance = new LocalDataStoreActivityMonitor();

				return _instance; 
			}	
		}

		public event EventHandler<ItemEventArgs<SendProgressItem>> SendProgressUpdate
		{
			add 
			{
				HandleConnection(true);
				_sendProgressUpdate += value;
				_serviceClient.Refresh();
			}
			remove 
			{ 
				_sendProgressUpdate -= value;
				if (!this.AnySubscribers)
					HandleConnection(false);
			}
		}

		public event EventHandler<ItemEventArgs<ReceiveProgressItem>> ReceiveProgressUpdate
		{
			add
			{
				HandleConnection(true); 
				_receiveProgressUpdate += value;
				_serviceClient.Refresh();
			}
			remove 
			{ 
				_receiveProgressUpdate -= value;
				if (!this.AnySubscribers)
					HandleConnection(false);
			}
		}

		public event EventHandler<ItemEventArgs<ImportProgressItem>> ImportProgressUpdate
		{
			add 
			{
				HandleConnection(true);
				_importProgressUpdate += value;
				_serviceClient.Refresh();
			}
			remove
			{ 
				_importProgressUpdate -= value;
				if (!this.AnySubscribers)
					HandleConnection(false);
			}
		}

		public event EventHandler<ItemEventArgs<ReindexProgressItem>> ReindexProgressUpdate
		{
			add 
			{
				HandleConnection(true);
				_reindexProgressUpdate += value;
				_serviceClient.Refresh();
			}
			remove
			{
				_reindexProgressUpdate -= value;
				if (!this.AnySubscribers)
					HandleConnection(false);
			}
		}

		public event EventHandler LostConnection
		{
			add { _lostConnection += value; }
			remove { _lostConnection -= value; }
		}

		public event EventHandler Connected
		{
			add { _connected += value; }
			remove { _connected -= value; }
		}

		private bool AnySubscribers
		{
			get
			{
				return (_importProgressUpdate != null ||
					_receiveProgressUpdate != null ||
					_reindexProgressUpdate != null ||
					_sendProgressUpdate != null ||
					_sopInstanceImported != null);
			}
		}

		private void OnChannelClosed(object sender, EventArgs e)
		{
			OnLostConnection();
		}

		private void OnChannelFaulted(object sender, EventArgs e)
		{
			OnLostConnection();
		}

		private void OnLostConnection()
		{
			EventsHelper.Fire(_lostConnection, this, EventArgs.Empty);
			CloseConnection();
		}

		private void OpenConnection()
		{
			if (_serviceClient == null)
			{
				_callback = new LocalDataStoreActivityMonitorServiceCallback(this);
				_serviceClient = new LocalDataStoreActivityMonitorServiceClient(new InstanceContext(_callback));

				try
				{
					_serviceClient.Open();
					
					//we could actually attempt to manage subscriptions to individual events, but it's not really worth it.
					_serviceClient.Subscribe(""); 
					_serviceClient.InnerChannel.Faulted += new EventHandler(OnChannelFaulted);
					_serviceClient.InnerChannel.Closed += new EventHandler(OnChannelClosed);

					EventsHelper.Fire(_connected, this, EventArgs.Empty);
				}
				catch
				{
					_serviceClient = null;
					throw;
				}
			}
		}

		private void CloseConnection()
		{
			if (_serviceClient != null)
			{
				try
				{
					_serviceClient.InnerChannel.Faulted -= new EventHandler(OnChannelFaulted);
					_serviceClient.InnerChannel.Closed -= new EventHandler(OnChannelClosed);
					_serviceClient.Unsubscribe("");
					_serviceClient.Close();
				}
				catch (Exception e)
				{
					_serviceClient.Abort();
					Platform.Log(e);
				}
				finally
				{
					_callback.Dispose();
					_callback = null;
					_serviceClient = null;
				}
			}
		}

		private void HandleConnection(bool anySubscribers)
		{
			if (!anySubscribers)
			{
				CloseConnection();
			}
			else
			{
				OpenConnection();
			}
		}

		public void Cancel(CancelProgressItemInformation information)
		{
			LocalDataStoreActivityMonitorServiceCallback dummy = new LocalDataStoreActivityMonitorServiceCallback(this);
			LocalDataStoreActivityMonitorServiceClient client = new LocalDataStoreActivityMonitorServiceClient(new InstanceContext(dummy));

			try
			{
				client.Open();
				client.Cancel(information);
				client.Close();
			}
			catch
			{
				client.Abort();
				throw;
			}
		}

		public void ClearInactive()
		{
			LocalDataStoreActivityMonitorServiceCallback dummy = new LocalDataStoreActivityMonitorServiceCallback(this);
			LocalDataStoreActivityMonitorServiceClient client = new LocalDataStoreActivityMonitorServiceClient(new InstanceContext(dummy));

			try
			{
				client.Open();
				client.ClearInactive();
				client.Close();
			}
			catch
			{
				client.Abort();
				throw;
			}
		}

		public void Refresh()
		{
			LocalDataStoreActivityMonitorServiceCallback dummy = new LocalDataStoreActivityMonitorServiceCallback(this);
			LocalDataStoreActivityMonitorServiceClient client = new LocalDataStoreActivityMonitorServiceClient(new InstanceContext(dummy));

			try
			{
				client.Open();
				client.Refresh();
				client.Close();
			}
			catch
			{
				client.Abort();
				throw;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			CloseConnection();
		}

		#endregion
	}
}
