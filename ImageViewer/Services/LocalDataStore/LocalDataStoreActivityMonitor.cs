#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	/// <summary>
	/// This is a singleton class that manages one connection to the LocalDataStoreActivityMonitorService
	/// for the entire Application Domain.
	/// </summary>
	/// <remarks>
	/// This class is thread safe.
	/// </remarks>
	public sealed class LocalDataStoreActivityMonitor
	{
		[CallbackBehavior(UseSynchronizationContext = false)]
		private class LocalDataStoreActivityMonitorServiceCallback : ILocalDataStoreActivityMonitorServiceCallback
		{
			private readonly LocalDataStoreActivityMonitor _parent;

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

			public void LocalDataStoreCleared()
			{
				_parent.OnLocalDataStoreCleared();
			}

			#endregion
		}

		#region Private Fields

		private static LocalDataStoreActivityMonitor _instance;

		private readonly object _connectionThreadLock = new object();
		private bool _active;
		private bool _stopThread;
		private volatile bool _isConnected;

		private LocalDataStoreActivityMonitorServiceCallback _callback;
		private LocalDataStoreActivityMonitorServiceClient _serviceClient;
		private Thread _connectionThread;

		private readonly object _subscriptionLock = new object();
		private event EventHandler<ItemEventArgs<ReceiveProgressItem>> _receiveProgressUpdate;
		private event EventHandler<ItemEventArgs<SendProgressItem>> _sendProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportProgressItem>> _importProgressUpdate;
		private event EventHandler<ItemEventArgs<ReindexProgressItem>> _reindexProgressUpdate;
		private event EventHandler<ItemEventArgs<ImportedSopInstanceInformation>> _sopInstanceImported;
		private event EventHandler<ItemEventArgs<DeletedInstanceInformation>> _instanceDeleted;
		private event EventHandler _localDataStoreCleared;
		private event EventHandler _lostConnection;
		private event EventHandler _connected;
		private bool _refreshRequired;
				
		#endregion

		private LocalDataStoreActivityMonitor()
		{
		}

		#region Singleton Instance

		internal static LocalDataStoreActivityMonitor Instance
		{
			get 
			{
				try
				{
					if (_instance == null)
						_instance = new LocalDataStoreActivityMonitor();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
					_instance = null;
				}

				return _instance; 
			}
		}

		#endregion

		#region Internal Events

		internal event EventHandler<ItemEventArgs<SendProgressItem>> SendProgressUpdate
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

		internal event EventHandler<ItemEventArgs<ReceiveProgressItem>> ReceiveProgressUpdate
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

		internal event EventHandler<ItemEventArgs<ImportProgressItem>> ImportProgressUpdate
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

		internal event EventHandler<ItemEventArgs<ReindexProgressItem>> ReindexProgressUpdate
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

		internal event EventHandler<ItemEventArgs<ImportedSopInstanceInformation>> SopInstanceImported
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

		internal event EventHandler<ItemEventArgs<DeletedInstanceInformation>> InstanceDeleted
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

		internal event EventHandler LocalDataStoreCleared
		{
			add
			{
				lock (_subscriptionLock)
				{
					_localDataStoreCleared += value;
				}

				Startup();
			}
			remove
			{
				lock (_subscriptionLock)
				{
					_localDataStoreCleared -= value;
				}

				if (!this.AnySubscribers)
					ShutDown();
			}
		}

		internal event EventHandler LostConnection
		{
			add
			{
				lock (_subscriptionLock)
				{
					_lostConnection += value;
				}

				Startup();
			}
			remove
			{
				lock (_subscriptionLock)
				{
					_lostConnection -= value;
				}

				if (!this.AnySubscribers)
					ShutDown();
			}
		}

		internal event EventHandler Connected
		{
			add
			{
				lock (_subscriptionLock)
				{
					_connected += value;
				}

				Startup();
			}
			remove
			{
				lock (_subscriptionLock)
				{
					_connected -= value;
				}

				if (!this.AnySubscribers)
					ShutDown();
			}
		}

		#endregion

		#region Public Properties

		public static bool IsConnected
		{
			get { return Instance._isConnected; }
		}

		#endregion

		#region Private Properties

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
						_instanceDeleted != null ||
						_lostConnection != null ||
						_connected != null);
				}
			}
		}
		
		#endregion

		#region Private Methods

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

				_callback = null;

				_active = false;
			}
		}

		private void OnReceiveProgressChanged(ReceiveProgressItem progressItem)
		{
			lock (_subscriptionLock)
			{
				if (!this.AnySubscribers)
					return;

				EventsHelper.Fire(_receiveProgressUpdate, this, new ItemEventArgs<ReceiveProgressItem>(progressItem));
			}
		}

		private void OnSendProgressChanged(SendProgressItem progressItem)
		{
			lock (_subscriptionLock)
			{
				if (!this.AnySubscribers)
					return;
				EventsHelper.Fire(_sendProgressUpdate, this, new ItemEventArgs<SendProgressItem>(progressItem));
			}
		}

		private void OnImportProgressChanged(ImportProgressItem progressItem)
		{
			lock (_subscriptionLock)
			{
				if (!this.AnySubscribers)
					return;

				EventsHelper.Fire(_importProgressUpdate, this, new ItemEventArgs<ImportProgressItem>(progressItem));
			}
		}

		private void OnReindexProgressChanged(ReindexProgressItem progressItem)
		{
			lock (_subscriptionLock)
			{
				if (!this.AnySubscribers)
					return;

				EventsHelper.Fire(_reindexProgressUpdate, this, new ItemEventArgs<ReindexProgressItem>(progressItem));
			}
		}

		private void OnSopInstanceImported(ImportedSopInstanceInformation information)
		{
			lock (_subscriptionLock)
			{
				if (!this.AnySubscribers)
					return;

				EventsHelper.Fire(_sopInstanceImported, this, new ItemEventArgs<ImportedSopInstanceInformation>(information));
			}
		}

		private void OnInstanceDeleted(DeletedInstanceInformation information)
		{
			lock (_subscriptionLock)
			{
				if (!this.AnySubscribers)
					return;

				EventsHelper.Fire(_instanceDeleted, this, new ItemEventArgs<DeletedInstanceInformation>(information));
			}
		}

		private void OnLocalDataStoreCleared()
		{
			lock (_subscriptionLock)
			{
				if (!this.AnySubscribers)
					return;

				EventsHelper.Fire(_localDataStoreCleared, this, EventArgs.Empty);
			}
		}

		private void OnLostConnection()
		{
			lock (_connectionThreadLock)
			{
				CloseConnection();

				if (this.AnySubscribers)
					_refreshRequired = true;

				//retry the connection one time (if needed) before firing 'lost connection'.
				Monitor.Pulse(_connectionThreadLock);
				Monitor.Wait(_connectionThreadLock);

				lock (_subscriptionLock)
				{
					if (!_isConnected)
						EventsHelper.Fire(_lostConnection, this, EventArgs.Empty);
				}
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

					lock(_subscriptionLock)
					{
						EventsHelper.Fire(_connected, this, EventArgs.Empty);
					}
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

					Platform.Log(LogLevel.Error, e);
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
					Platform.Log(LogLevel.Error, e);
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
						Platform.Log(LogLevel.Error, e);
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
		#endregion

		#region Client Functions

		public static void Cancel(CancelProgressItemInformation information)
		{
			LocalDataStoreActivityMonitorServiceCallback dummy = new LocalDataStoreActivityMonitorServiceCallback(Instance);
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

		public static void ClearInactive()
		{
			LocalDataStoreActivityMonitorServiceCallback dummy = new LocalDataStoreActivityMonitorServiceCallback(Instance);
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

		public static void Refresh()
		{
			LocalDataStoreActivityMonitorServiceCallback dummy = new LocalDataStoreActivityMonitorServiceCallback(Instance);
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

		#region Public Factory Methods

		public static ILocalDataStoreEventBroker CreatEventBroker()
		{
			return CreatEventBroker(true);
		}

		public static ILocalDataStoreEventBroker CreatEventBroker(bool useSynchronizationContext)
		{
			SynchronizationContext synchronizationContext = null;
			if (useSynchronizationContext)
				synchronizationContext = SynchronizationContext.Current;

			return CreatEventBroker(synchronizationContext);
		}

		public static ILocalDataStoreEventBroker CreatEventBroker(SynchronizationContext synchronizationContext)
		{
			return new LocalDataStoreEventBroker(synchronizationContext);
		}

		#endregion
	}
}
