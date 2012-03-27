#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Security.Principal;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
	/// <summary>
	/// This is a singleton class that manages one connection to the LocalDataStoreActivityMonitorService
	/// for the entire Application Domain.
	/// </summary>
	/// <remarks>
	/// This class is thread safe.
	/// </remarks>
	public sealed class WorkItemMonitor
	{
		[CallbackBehavior(UseSynchronizationContext = false)]
		private class WorkActivityMonitorServiceCallback : IWorkItemActivityCallback
		{
			private readonly WorkItemMonitor _parent;

			public WorkActivityMonitorServiceCallback(WorkItemMonitor parent)
			{
				_parent = parent;
            }

            #region IWorkItemActivityCallback Members        

            public void WorkItemChanged(WorkItemData progressItem)
		    {
		        _parent.OnReceiveProgressChanged(progressItem);
            }

            #endregion
        }

		#region Private Fields

		private static WorkItemMonitor _instance;

		private readonly object _connectionThreadLock = new object();
		private bool _active;
		private bool _stopThread;
		private volatile bool _isConnected;

		private WorkActivityMonitorServiceCallback _callback;
		private WorkItemServiceClient _serviceClient;
		private Thread _connectionThread;

		private readonly object _subscriptionLock = new object();
		private event EventHandler<ItemEventArgs<WorkItemData>> _receiveProgressUpdate;
		private event EventHandler _lostConnection;
		private event EventHandler _connected;
		private bool _refreshRequired;
				
		#endregion

		private WorkItemMonitor()
		{
		}

		#region Singleton Instance

		internal static WorkItemMonitor Instance
		{
			get 
			{
				try
				{
					if (_instance == null)
						_instance = new WorkItemMonitor();
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

	

		internal event EventHandler<ItemEventArgs<WorkItemData>> ReceiveProgressUpdate
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
					return (
						_receiveProgressUpdate != null ||
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

					_callback = new WorkActivityMonitorServiceCallback(this);

					_isConnected = false;
					_stopThread = false;
					_refreshRequired = false;
					_serviceClient = null;

					ThreadStart threadStart = RunThread;
				    _connectionThread = new Thread(threadStart)
				                            {
				                                IsBackground = true, 
                                                Priority = ThreadPriority.Lowest
				                            };

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

		private void OnReceiveProgressChanged(WorkItemData progressItem)
		{
			lock (_subscriptionLock)
			{
				if (!AnySubscribers)
					return;

				EventsHelper.Fire(_receiveProgressUpdate, this, new ItemEventArgs<WorkItemData>(progressItem));
			}
		}


		private void OnLostConnection()
		{
			lock (_connectionThreadLock)
			{
				CloseConnection();

				if (AnySubscribers)
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
				_serviceClient = new WorkItemServiceClient(new InstanceContext(_callback));

				try
				{
					_serviceClient.Open();

					//we could actually attempt to manage subscriptions to individual events, but it's not really worth it right now.
					_serviceClient.Subscribe(new WorkItemSubscribeRequest {Culture = CultureInfo.CurrentCulture });
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
					_serviceClient.Unsubscribe(new WorkItemUnsubscribeRequest());
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
						//_serviceClient.Refresh();
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

		public static WorkItemData Cancel(WorkItemData item)
		{
            var dummy = new WorkActivityMonitorServiceCallback(Instance);
            var client = new WorkItemServiceClient(new InstanceContext(dummy));

			try
			{
				client.Open();
                WorkItemUpdateRequest rq = new WorkItemUpdateRequest();
			    rq.Cancel = true;
			    rq.Identifier = item.Identifier;
			    var rsp = client.Update(rq);
                client.Close();
			    return rsp.Item;				
			}
			catch
			{
				client.Abort();
				throw;
			}
		}

        public static WorkItemInsertResponse Insert(WorkItemInsertRequest rq)
        {
            var dummy = new WorkActivityMonitorServiceCallback(Instance);
            var client = new WorkItemServiceClient(new InstanceContext(dummy));

			try
			{
				client.Open();
                var rsp = client.Insert(rq);
                client.Close();
			    return rsp;				
			}
			catch
			{
				client.Abort();
				throw;
			}
        }

		public static void Refresh()
		{
            var dummy = new WorkActivityMonitorServiceCallback(Instance);
            WorkItemServiceClient client = new WorkItemServiceClient(new InstanceContext(dummy));

			try
			{
				client.Open();
				//client.Refresh();
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



		#endregion
	}
}

