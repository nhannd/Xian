using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Common.Utilities;
using System.ServiceModel;
using System.ComponentModel;
using ClearCanvas.Common;
using System.Threading;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	/// <summary>
	/// This is a singleton class that manages one connection to the LocalDataStoreActivityMonitorService for all application components.
	/// ApplicationComponents can subscribe to the various events, but it is VERY important that all components unsubscribe when finished.
	/// Otherwise, the connection to the service may remain open and the resources used may not get freed up.  
	/// </summary>
	/// <remarks>
	/// This class is not guaranteed to be thread safe and is intended only to be used from the main UI thread.
	/// </remarks>
	public sealed class LocalDataStoreActivityMonitor
	{
		[CallbackBehavior(UseSynchronizationContext = false)]
		private class LocalDataStoreActivityMonitorServiceCallback : ILocalDataStoreActivityMonitorServiceCallback
		{
			private LocalDataStoreActivityMonitor _parent;

			public LocalDataStoreActivityMonitorServiceCallback(LocalDataStoreActivityMonitor parent)
			{
				_parent = parent;
			}

			#region ILocalDataStoreActivityMonitorServiceCallback Members

			public void ReceiveProgressChanged(ReceiveProgressItem progressItem)
			{
				_parent.OnReceiveProgressChanged(progressItem);
			}

			public void SendProgressChanged(SendProgressItem progressItem)
			{
				_parent.OnSendProgressChanged(progressItem);
			}

			public void ImportProgressChanged(ImportProgressItem progressItem)
			{
				_parent.OnImportProgressChanged(progressItem);
			}

			public void ReindexProgressChanged(ReindexProgressItem progressItem)
			{
				_parent.OnReindexProgressChanged(progressItem);
			}

			public void SopInstanceImported(ImportedSopInstanceInformation information)
			{
				_parent.OnSopInstanceImported(information);
			}

			public void InstanceDeleted(DeletedInstanceInformation information)
			{
				_parent.OnInstanceDeleted(information);
			}

			#endregion
		}

		private static LocalDataStoreActivityMonitor _instance;

		private event EventHandler<ItemEventArgs<ReceiveProgressItem>> _receiveProgressUpdate;
		private event EventHandler<ItemEventArgs<SendProgressItem>> _sendProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportProgressItem>> _importProgressUpdate;
		private event EventHandler<ItemEventArgs<ReindexProgressItem>> _reindexProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportedSopInstanceInformation>> _sopInstanceImported;
		private event EventHandler<ItemEventArgs<DeletedInstanceInformation>> _instanceDeleted;

		private event EventHandler _lostConnection;
		private event EventHandler _connected;

		private InterthreadMarshaler _marshaler;

		private object _connectionThreadLock = new object();
		private bool _active;
		private bool _stopThread;
		private volatile bool _isConnected;

		private object _subscriptionLock = new object();
		private bool _refreshRequired;
				
		private LocalDataStoreActivityMonitorServiceCallback _callback;
		private LocalDataStoreActivityMonitorServiceClient _serviceClient;
		private Thread _connectionThread;

		private LocalDataStoreActivityMonitor()
		{
		}

		/// <summary>
		/// Try to make sure things get cleaned up during Finalization just in case a component didn't unsubscribe.
		/// </summary>
		~LocalDataStoreActivityMonitor()
		{
			try
			{
				ShutDown();
			}
			catch (Exception e)
			{
				Platform.Log(e);
			}
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

				Startup();
			}
			remove 
			{
				lock (_subscriptionLock)
				{
					_sendProgressUpdate -= value;
				}

				if (!this.AnySubscribers)
					ShutDown();
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

				Startup();
			}
			remove 
			{
				lock (_subscriptionLock)
				{
					_receiveProgressUpdate -= value;
				}

				if (!this.AnySubscribers)
					ShutDown();
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

				Startup();
			}
			remove
			{
				lock (_subscriptionLock)
				{
					_importProgressUpdate -= value;
				}

				if (!this.AnySubscribers)
					ShutDown();
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

				Startup();
			}
			remove
			{
				lock (_subscriptionLock)
				{
					_reindexProgressUpdate -= value;
				}

				if (!this.AnySubscribers)
					ShutDown();
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
				
				Startup();
			}
			remove
			{
				lock (_subscriptionLock)
				{
					_sopInstanceImported -= value;
				}

				if (!this.AnySubscribers)
					ShutDown();
			}
		}

		public event EventHandler<ItemEventArgs<DeletedInstanceInformation>> InstanceDeleted
		{
			add
			{
				lock (_subscriptionLock)
				{
					_instanceDeleted += value;
				}

				Startup();
			}
			remove
			{
				lock (_subscriptionLock)
				{
					_instanceDeleted -= value;
				}

				if (!this.AnySubscribers)
					ShutDown();
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
						_sopInstanceImported != null ||
						_instanceDeleted != null);
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

		private void Startup()
		{
			lock (_connectionThreadLock)
			{
				if (!_active)
				{
					_active = true;

					_marshaler = new InterthreadMarshaler();
					_callback = new LocalDataStoreActivityMonitorServiceCallback(this);

					_isConnected = false;
					_stopThread = false;
					_refreshRequired = false;
					_serviceClient = null;

					ThreadStart threadStart = new ThreadStart(this.RunThread);
					_connectionThread = new Thread(threadStart);
					_connectionThread.IsBackground = true;
					_connectionThread.Priority = ThreadPriority.Lowest;

					_connectionThread.Start();
					Monitor.Wait(_connectionThreadLock); //wait for the thread to signal it has started.
				}

				//release the running thread to attempt to connect/refresh if necessary.
				Monitor.Pulse(_connectionThreadLock);
				//wait up to one second for a connection/refresh to occur.
				Monitor.Wait(_connectionThreadLock, 1000);
			}
		}

		private void ShutDown()
		{
			lock (_connectionThreadLock)
			{
				if (!_active)
					return;

				_stopThread = true;
				//release the thread and wait for it to signal it has stopped running.
				Monitor.Pulse(_connectionThreadLock);
				Monitor.Wait(_connectionThreadLock);

				_connectionThread.Join();
				_connectionThread = null;

				if (_marshaler != null)
				{
					_marshaler.Dispose();
					_marshaler = null;
				}

				_callback = null;

				_active = false;
			}
		}

		private void OnReceiveProgressChanged(ReceiveProgressItem progressItem)
		{
			if (!this.AnySubscribers || _marshaler == null)
				return;

			_marshaler.QueueInvoke(delegate()
			{
					EventsHelper.Fire(_receiveProgressUpdate, this, new ItemEventArgs<ReceiveProgressItem>(progressItem));
			});
		}

		private void OnSendProgressChanged(SendProgressItem progressItem)
		{
			if (!this.AnySubscribers || _marshaler == null)
				return;

			_marshaler.QueueInvoke(delegate()
			{
				EventsHelper.Fire(_sendProgressUpdate, this, new ItemEventArgs<SendProgressItem>(progressItem));
			});
		}

		private void OnImportProgressChanged(ImportProgressItem progressItem)
		{
			if (!this.AnySubscribers || _marshaler == null)
				return;

			_marshaler.QueueInvoke(delegate()
			{
				EventsHelper.Fire(_importProgressUpdate, this, new ItemEventArgs<ImportProgressItem>(progressItem));
			});
		}

		private void OnReindexProgressChanged(ReindexProgressItem progressItem)
		{
			if (!this.AnySubscribers || _marshaler == null)
				return;

			_marshaler.QueueInvoke(delegate()
			{
				EventsHelper.Fire(_reindexProgressUpdate, this, new ItemEventArgs<ReindexProgressItem>(progressItem));
			});
		}

		private void OnSopInstanceImported(ImportedSopInstanceInformation information)
		{
			if (!this.AnySubscribers || _marshaler == null)
				return;

			_marshaler.QueueInvoke(delegate()
			{
				EventsHelper.Fire(_sopInstanceImported, this, new ItemEventArgs<ImportedSopInstanceInformation>(information));
			});
		}

		internal void OnInstanceDeleted(DeletedInstanceInformation information)
		{
			if (!this.AnySubscribers || _marshaler == null)
				return;

			_marshaler.QueueInvoke(delegate()
			{
				EventsHelper.Fire(_instanceDeleted, this, new ItemEventArgs<DeletedInstanceInformation>(information));
			});

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
				catch (EndpointNotFoundException)
				{ 
					//the service isn't running, don't log the exception.
					_serviceClient.Abort();
					_serviceClient = null;

					_isConnected = false;
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
				if (_refreshRequired && _serviceClient != null)
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
				//signal the thread has started up.
				Monitor.Pulse(_connectionThreadLock);

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
				
				//the ShutDown method is waiting for a final pulse before joining the thread.
				Monitor.Pulse(_connectionThreadLock);
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
				//because of the WCF buffer size, we need to break this request down into reasonable batches.
				List<Guid> progressIdentifiers = new List<Guid>(information.ProgressItemIdentifiers);
				List<Guid> batchList = new List<Guid>();
				
				int batchSize = 500;

				while (progressIdentifiers.Count > 0)
				{
					batchList.Add(progressIdentifiers[0]);
					progressIdentifiers.RemoveAt(0);

					if (batchList.Count >= batchSize || progressIdentifiers.Count == 0)
					{
						CancelProgressItemInformation batchCancellation = new CancelProgressItemInformation();
						batchCancellation.CancellationFlags = information.CancellationFlags;
						batchCancellation.ProgressItemIdentifiers = batchList;

						client.Cancel(batchCancellation);

						batchList.Clear();
					}
				}

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
	}
}
