using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    internal class RealWorkItemActivityMonitor : WorkItemActivityMonitor
    {
        [CallbackBehavior(UseSynchronizationContext = false)]
        private class Callback : WorkItemActivityCallback
        {
            private readonly RealWorkItemActivityMonitor _realActivityMonitor;

            internal Callback(RealWorkItemActivityMonitor real)
            {
                _realActivityMonitor = real;
            }

            public override void WorkItemChanged(WorkItemData workItemData)
            {
                _realActivityMonitor.OnWorkItemChanged(workItemData);
            }

            public override void StudiesCleared()
            {
                _realActivityMonitor.OnStudiesCleared();
            }
        }

        internal static TimeSpan ConnectionRetryInterval = TimeSpan.FromSeconds(5);

        private readonly object _syncLock = new object();

        private Thread _connectionThread;
        private bool _disposed;

        private bool _subscribedToService;
        
        private event EventHandler _isConnectedChanged;

        private IList<WorkItemTypeEnum> _workItemTypeFilters;
        private IList<long> _workItemIdFilters;
        private event EventHandler<WorkItemChangedEventArgs> _workItemChanged;
        private event EventHandler _studiesCleared;

        private volatile IWorkItemActivityMonitorService _client;

        internal RealWorkItemActivityMonitor()
        {
            _connectionThread = new Thread(MonitorConnection);
            lock (_syncLock)
            {
                _connectionThread.Start();
                Monitor.Wait(_syncLock); //Wait for the thread to start up.
            }
        }

        public override bool IsConnected
        {
            get { return _client != null; }
        }

        public override event EventHandler IsConnectedChanged
        {
            add
            {
                lock (_syncLock)
                {
                    _isConnectedChanged += value;
                    Monitor.Pulse(_syncLock);
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _isConnectedChanged -= value;
                    Monitor.Pulse(_syncLock);
                }
            }
        }

        public override WorkItemTypeEnum[] WorkItemTypeFilters
        {
            get
            {
                lock(_syncLock)
                {
                    return _workItemTypeFilters == null ? null : _workItemTypeFilters.ToArray();
                }
            }
            set
            {
                lock (_syncLock)
                {
                    if (value == null || value.Length == 0)
                        _workItemTypeFilters = null;
                    else
                        _workItemTypeFilters = value.ToList();
                }
            }
        }

        public override long[] WorkItemIdFilters
        {
            get
            {
                lock (_syncLock)
                {
                    return _workItemIdFilters == null ? null : _workItemIdFilters.ToArray();
                }
            }
            set
            {
                lock (_syncLock)
                {
                    if (value == null || value.Length == 0)
                        _workItemIdFilters = null;
                    else
                        _workItemIdFilters = value.ToList();
                }
            }
        }

        public override event EventHandler StudiesCleared
        {
            add
            {
                lock (_syncLock)
                {
                    _studiesCleared += value;
                    Monitor.Pulse(_syncLock);
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _studiesCleared -= value;
                    Monitor.Pulse(_syncLock);
                }
            }
        }


        public override event EventHandler<WorkItemChangedEventArgs> WorkItemChanged
        {
            add
            {
                lock (_syncLock)
                {
                    _workItemChanged += value;
                    Monitor.Pulse(_syncLock);
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _workItemChanged -= value;
                    Monitor.Pulse(_syncLock);
                }
            }
        }

        private void MonitorConnection(object ignore)
        {
            lock (_syncLock)
            {
                //Try to connect first.
                Connect();
                ManageSubscription();

                //startup pulse.
                Monitor.Pulse(_syncLock);
            }

            while (true)
            {
                lock (_syncLock)
                {
                    //Check disposed before and after the wait because it could have changed.
                    if (_disposed) break;

                    Monitor.Wait(_syncLock, ConnectionRetryInterval);
                    if (_disposed) break;
                }

                Connect();
                ManageSubscription();
            }

            DropConnection();
            ManageSubscription();
        }

        private void ManageSubscription()
        {
            if (_client == null)
            {
                //No need to sync because this variable's only used on one thread.
                _subscribedToService = false;
                return;
            }

            bool subscribe;
            lock (_syncLock)
            {
                var hasListeners = _workItemChanged != null;
                bool needsSubscriptionChange = _subscribedToService != hasListeners;
                if (!needsSubscriptionChange)
                    return;

                subscribe = hasListeners;
            }

            try
            {
                if (subscribe)
                {
                    _client.Subscribe(new WorkItemSubscribeRequest());
                    _subscribedToService = true;
                }
                else
                {
                    _client.Unsubscribe(new WorkItemUnsubscribeRequest());
                    _subscribedToService = false;
                }
            }
            catch(Exception e)
            {
                _subscribedToService = false;
                Platform.Log(LogLevel.Warn, e, "Failed to subscribe/unsubscribe from ActivityMonitorService.");
            }
        }

        private void Connect()
        {
            if (_client != null)
                return;

            Platform.Log(LogLevel.Debug, "Attempting to connect to ActivityMonitorService.");

            try
            {
                var callback = new Callback(this);
                var client = Platform.GetDuplexService<IWorkItemActivityMonitorService, IWorkItemActivityCallback>(callback);
                var communication = (ICommunicationObject)client;
                if (communication.State == CommunicationState.Created)
                    communication.Open();

                communication.Closed += OnConnectionClosed;
                communication.Faulted += OnConnectionFaulted;

                Platform.Log(LogLevel.Debug, "Successfully connected to ActivityMonitorService.");

                _client = client;
                FireIsConnectedChanged();
            }
            catch (EndpointNotFoundException)
            {
                Platform.Log(LogLevel.Debug, "ActivityMonitorService is not running.");
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e, "Unexpected error trying to connect to ActivityMonitorService.");
            }
        }

        void OnConnectionFaulted(object sender, EventArgs e)
        {
            DropConnection();
            ManageSubscription();
        }

        void OnConnectionClosed(object sender, EventArgs e)
        {
            DropConnection();
            ManageSubscription();
        }

        private void DropConnection()
        {
            if (_client == null)
                return;

            Platform.Log(LogLevel.Debug, "Attempting to disconnect from ActivityMonitorService.");

            try
            {
                var communication = (ICommunicationObject)_client;
                communication.Closed -= OnConnectionClosed;
                communication.Faulted -= OnConnectionFaulted;

                if (communication.State == CommunicationState.Opened)
                    communication.Close();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e, "Unexpected error disconnecting from ActivityMonitorService.");
            }

            _client = null;
            FireIsConnectedChanged();
        }

        private void FireIsConnectedChanged()
        {
            Delegate[] delegates;
            lock (_syncLock)
            {
                if (_disposed)
                    return;

                delegates = _isConnectedChanged != null ? _isConnectedChanged.GetInvocationList() : new Delegate[0];
            }

            if (delegates.Length > 0)
            {
                //ThreadPool.QueueUserWorkItem(ignore => CallDelegates(delegates, EventArgs.Empty));
                CallDelegates(delegates, EventArgs.Empty);
            }
        }

        private void OnWorkItemChanged(WorkItemData workItemData)
        {
            IList<Delegate> delegates;
            lock (_syncLock)
            {
                if (_disposed || !this.WorkItemMatchesFilters(workItemData))
                    return;

                delegates = _workItemChanged != null ? _workItemChanged.GetInvocationList() : new Delegate[0];
            }

            if (delegates.Count <= 0)
                return;

            var args = new WorkItemChangedEventArgs(workItemData);
            //ThreadPool.QueueUserWorkItem(ignore => CallDelegates(delegates, args));
            CallDelegates(delegates, args);
        }

        private void OnStudiesCleared()
        {
            IList<Delegate> delegates;
            lock (_syncLock)
            {
                delegates = _studiesCleared != null ? _studiesCleared.GetInvocationList() : new Delegate[0];
            }

            if (delegates.Count > 0)
                CallDelegates(delegates, EventArgs.Empty);
        }

        private void CallDelegates(IEnumerable<Delegate> delegates, EventArgs e)
        {
            foreach (var @delegate in delegates)
            {
                try
                {
                    @delegate.DynamicInvoke(this, e);
                }
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Error encountered while firing event.");
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            lock (_syncLock)
            {
                if (_disposed)
                    return;

                _disposed = true; //Setting disposed causes the thread to stop.
                Monitor.Pulse(_syncLock);

                //Don't bother joining - no point.
                _connectionThread = null;
            }
        }
    }
}