using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using System.ServiceModel;
using System.ComponentModel;
using ClearCanvas.Common;
using System.Threading;

namespace ClearCanvas.ImageViewer.Services
{
	public sealed class LocalDataStoreActivityMonitor
	{
		[CallbackBehavior(UseSynchronizationContext = false)]
		private class LocalDataStoreActivityMonitorServiceCallback : ILocalDataStoreActivityMonitorServiceCallback
		{
			private object _syncLock = new object();
			private LocalDataStoreActivityMonitor _parent;

			public LocalDataStoreActivityMonitorServiceCallback(LocalDataStoreActivityMonitor parent)
			{
				_parent = parent;
			}

			#region ILocalDataStoreActivityMonitorServiceCallback Members

			public void ReceiveProgressChanged(ReceiveProgressItem progressItem)
			{
				lock(_syncLock)
				{
					_parent._marshaler.QueueInvoke
					(delegate()
						{
							EventsHelper.Fire(_parent._receiveProgressUpdate, _parent, new ItemEventArgs<ReceiveProgressItem>(progressItem));
						});
				}
			}

			public void SendProgressChanged(SendProgressItem progressItem)
			{
				lock (_syncLock)
				{
					_parent._marshaler.QueueInvoke
					(delegate()
						{
							EventsHelper.Fire(_parent._sendProgressUpdate, _parent, new ItemEventArgs<SendProgressItem>(progressItem));
						});
				}
			}

			public void ImportProgressChanged(ImportProgressItem progressItem)
			{
				lock (_syncLock)
				{
					_parent._marshaler.QueueInvoke
					(delegate()
						{
							EventsHelper.Fire(_parent._importProgressUpdate, _parent, new ItemEventArgs<ImportProgressItem>(progressItem));
						});
				}
			}

			public void ReindexProgressChanged(ReindexProgressItem progressItem)
			{
				lock (_syncLock)
				{
					_parent._marshaler.QueueInvoke
					(delegate()
						{
							EventsHelper.Fire(_parent._reindexProgressUpdate, _parent, new ItemEventArgs<ReindexProgressItem>(progressItem));
						});
				}
			}

			public void SopInstanceImported(ImportedSopInstanceInformation information)
			{
				lock (_syncLock)
				{
					_parent._marshaler.QueueInvoke
					(delegate()
					{
						EventsHelper.Fire(_parent._sopInstanceImported, _parent, new ItemEventArgs<ImportedSopInstanceInformation>(information));
					});
				}
			}

			#endregion
		}

		private static LocalDataStoreActivityMonitor _instance;

		private event EventHandler<ItemEventArgs<ReceiveProgressItem>> _receiveProgressUpdate;
		private event EventHandler<ItemEventArgs<SendProgressItem>> _sendProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportProgressItem>> _importProgressUpdate;
		private event EventHandler<ItemEventArgs<ReindexProgressItem>> _reindexProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportedSopInstanceInformation>> _sopInstanceImported;

		private event EventHandler _lostConnection;
		private event EventHandler _connected;

		private InterthreadMarshaler _marshaler;

		private object _connectionThreadLock = new object();
		private bool _stopThread;
		private volatile bool _isConnected;

		private object _subscriptionLock = new object();
		private bool _refreshRequired;
				
		private LocalDataStoreActivityMonitorServiceCallback _callback;
		private LocalDataStoreActivityMonitorServiceClient _serviceClient;
		private Thread _connectionThread;

		public LocalDataStoreActivityMonitor()
		{
			_marshaler = new InterthreadMarshaler();
			_callback = new LocalDataStoreActivityMonitorServiceCallback(this);

			_isConnected = false;
			_stopThread = false;
			_refreshRequired = false;

			ThreadStart threadStart = new ThreadStart(this.RunThread);
			_connectionThread = new Thread(threadStart);
			_connectionThread.IsBackground = true;
			_connectionThread.Priority = ThreadPriority.Lowest;
			_connectionThread.Start();
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

		public bool IsConnected
		{
			get 
			{
				return _isConnected;
			}
		}
		
		public event EventHandler<ItemEventArgs<SendProgressItem>> SendProgressUpdate
		{
			add 
			{
				lock (_subscriptionLock)
				{
					_sendProgressUpdate += value;
					_refreshRequired = true;
				}

				WaitConnect();
			}
			remove 
			{
				lock (_subscriptionLock)
				{
					_sendProgressUpdate -= value;
				}
			}
		}

		public event EventHandler<ItemEventArgs<ReceiveProgressItem>> ReceiveProgressUpdate
		{
			add
			{
				lock (_subscriptionLock)
				{
					_receiveProgressUpdate += value;
					_refreshRequired = true;
				}

				WaitConnect();
			}
			remove 
			{
				lock (_subscriptionLock)
				{
					_receiveProgressUpdate -= value;
				}
			}
		}

		public event EventHandler<ItemEventArgs<ImportProgressItem>> ImportProgressUpdate
		{
			add 
			{
				lock (_subscriptionLock)
				{
					_importProgressUpdate += value;
					_refreshRequired = true;
				}

				WaitConnect();
			}
			remove
			{
				lock (_subscriptionLock)
				{
					_importProgressUpdate -= value;
				}
			}
		}

		public event EventHandler<ItemEventArgs<ReindexProgressItem>> ReindexProgressUpdate
		{
			add 
			{
				lock (_subscriptionLock)
				{
					_reindexProgressUpdate += value;
					_refreshRequired = true;
				}

				WaitConnect();
			}
			remove
			{
				lock (_subscriptionLock)
				{
					_reindexProgressUpdate -= value;
				}
			}
		}

		public event EventHandler<ItemEventArgs<ImportedSopInstanceInformation>> SopInstanceImported
		{
			add
			{
				lock (_subscriptionLock)
				{
					_sopInstanceImported += value;
				}
			}
			remove
			{
				lock (_subscriptionLock)
				{
					_sopInstanceImported -= value;
				}
			}
		}

		public event EventHandler LostConnection
		{
			add
			{
				_lostConnection += value;
			}
			remove
			{
				_lostConnection -= value;
			}
		}

		public event EventHandler Connected
		{
			add
			{
				_connected += value;
			}
			remove
			{
				_connected -= value;
			}
		}

		private bool AnySubscribers
		{
			get
			{
				lock (_subscriptionLock)
				{
					return (_importProgressUpdate != null ||
						_receiveProgressUpdate != null ||
						_reindexProgressUpdate != null ||
						_sendProgressUpdate != null ||
						_sopInstanceImported != null);
				}
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

		private void WaitConnect()
		{
			lock (_connectionThreadLock)
			{
				Monitor.Pulse(_connectionThreadLock);
				Monitor.Wait(_connectionThreadLock);
			}
		}

		private void OnLostConnection()
		{
			lock (_connectionThreadLock)
			{
				CloseConnection();

				lock (_subscriptionLock)
				{
					if (this.AnySubscribers)
						_refreshRequired = true;
				}

				//retry the connection one time (if needed) before firing 'lost connection'.
				Monitor.Pulse(_connectionThreadLock);
				Monitor.Wait(_connectionThreadLock);

				if (!_isConnected)
					_marshaler.QueueInvoke(delegate() { EventsHelper.Fire(_lostConnection, this, EventArgs.Empty); });
			}
		}

		#region Worker Thread Functions

		private void OpenConnection()
		{
			if (_serviceClient == null)
			{
				_serviceClient = new LocalDataStoreActivityMonitorServiceClient(new InstanceContext(_callback));

				try
				{
					_serviceClient.Open();

					//we could actually attempt to manage subscriptions to individual events, but it's not really worth it right now.
					_serviceClient.Subscribe("");
					_serviceClient.InnerChannel.Faulted += new EventHandler(OnChannelFaulted);
					_serviceClient.InnerChannel.Closed += new EventHandler(OnChannelClosed);

					_isConnected = true;
					_marshaler.QueueInvoke(delegate() { EventsHelper.Fire(_connected, this, EventArgs.Empty); });
				}
				catch (Exception e)
				{
					_serviceClient.Abort();
					_serviceClient = null;

					_isConnected = false;

					Platform.Log(e);
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
					_serviceClient = null;
					_isConnected = false;
				}
			}
		}

		private void OpenAndRefresh()
		{
			OpenConnection();

			lock (_subscriptionLock)
			{
				if (_refreshRequired)
				{
					try
					{
						_serviceClient.Refresh();
						_refreshRequired = false;
					}
					catch (Exception e)
					{
						Platform.Log(e);
						CloseConnection();
					}
				}
			}
		}

		private void RunThread()
		{
			lock (_connectionThreadLock)
			{
				while (true)
				{
					Monitor.Wait(_connectionThreadLock, 5000);

					if (_stopThread)
					{
						CloseConnection();
						break;
					}

					if (!_isConnected)
					{
						CloseConnection();
						if (this.AnySubscribers)
							OpenAndRefresh();
					}
					else
					{
						if (!this.AnySubscribers)
							CloseConnection();
						else
							OpenAndRefresh();
					}
				
					Monitor.Pulse(_connectionThreadLock);
				}
			}
		}

		#endregion

		#region Client Functions

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

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			lock (_connectionThreadLock)
			{
				_stopThread = true;
				Monitor.Pulse(_connectionThreadLock);
			}

			_connectionThread.Join();

			if (_marshaler != null)
			{
				_marshaler.Dispose();
				_marshaler = null;
			}

			_callback = null;
		}

		#endregion
	}
}
