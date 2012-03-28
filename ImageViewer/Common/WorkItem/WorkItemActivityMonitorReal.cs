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
        }

        internal static TimeSpan ConnectionRetryInterval = TimeSpan.FromSeconds(5);

        private readonly object _syncLock = new object();

        private Thread _connectionThread;
        private bool _disposed;
        private bool _updateSubscriptions;
        private event EventHandler _isConnectedChanged;

        private readonly WorkItemChangedEventWrappers _workItemChangedEvents;
        private volatile IWorkItemActivityMonitorService _client;

        internal RealWorkItemActivityMonitor()
        {
            _workItemChangedEvents = new WorkItemChangedEventWrappers();

            _connectionThread = new Thread(MonitorConnection);
            _connectionThread.Start();
            lock (_syncLock)
            {
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
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _isConnectedChanged -= value;
                }
            }
        }

        public override void Subscribe(WorkItemTypeEnum? workItemType, EventHandler<WorkItemChangedEventArgs> eventHandler)
        {
            lock (_syncLock)
            {
                _workItemChangedEvents[workItemType].Changed += eventHandler;
                _updateSubscriptions = true;
                Monitor.Pulse(_syncLock);
            }
        }

        public override void Unsubscribe(WorkItemTypeEnum? workItemType, EventHandler<WorkItemChangedEventArgs> eventHandler)
        {
            lock (_syncLock)
            {
                _workItemChangedEvents[workItemType].Changed -= eventHandler;
                _updateSubscriptions = true;
                Monitor.Pulse(_syncLock);
            }
        }

        private void MonitorConnection(object ignore)
        {
            lock (_syncLock)
            {
                //TODO (Marmot): Implement some kind of startup connection timeout, or maybe just pulse right away?

                //Try to connect first.
                Connect();
                ManageSubscriptions();

                //startup pulse.
                Monitor.Pulse(_syncLock);
            }

            while (true)
            {
                lock (_syncLock)
                {
                    //Check disposed before and after the wait because it could have changed.
                    if (_disposed) break;
                    if (!_updateSubscriptions)
                    {
                        Monitor.Wait(_syncLock, ConnectionRetryInterval);
                        if (_disposed) break;
                    }
                }

                Connect();
                ManageSubscriptions();
            }

            DropConnection();
            ManageSubscriptions();
        }

        private void ManageSubscriptions()
        {
            IList<WorkItemChangedEventWrapper> newSubscriptions;
            IList<WorkItemChangedEventWrapper> deadSubscriptions;

            if (_client == null)
            {
                lock (_syncLock)
                {
                    _updateSubscriptions = false;
                    //No connection means no subscriptions.
                    foreach (var workItemChangedEvent in _workItemChangedEvents.GetActiveWrappers())
                        workItemChangedEvent.IsSubscribedToService = false;
                }
                return;
            }

            lock (_syncLock)
            {
                _updateSubscriptions = false;
                newSubscriptions = _workItemChangedEvents.GetWrappersToSubscribeToService();
                deadSubscriptions = _workItemChangedEvents.GetWrappersToUnsubscribeFromService();
            }

            foreach (var deadSubscription in deadSubscriptions)
            {
                try
                {
                    _client.Unsubscribe(new WorkItemUnsubscribeRequest { Type = deadSubscription.WorkItemType });
                }
                catch (Exception e)
                {
                    var workItemTypeName = deadSubscription.WorkItemType.HasValue
                                               ? deadSubscription.WorkItemType.ToString()
                                               : "All";

                    Platform.Log(LogLevel.Debug, e, "Failed to unsubscribe from event type '{0}'", workItemTypeName);
                }
                finally
                {
                    //Note (Marmot): All we can do is assume we're unsubscribed and try again later if needed.
                    deadSubscription.IsSubscribedToService = false;
                }
            }

            foreach (var newSubscription in newSubscriptions)
            {
                try
                {
                    _client.Subscribe(new WorkItemSubscribeRequest
                    {
                        //TODO (Marmot): How do we deal with this, or do we need to?
                        //Culture = Thread.CurrentThread.CurrentUICulture,
                        Type = newSubscription.WorkItemType
                    });

                    newSubscription.IsSubscribedToService = true;
                }
                catch (Exception e)
                {
                    //Note (Marmot): All we can do is assume we're unsubscribed and try again later if needed.
                    newSubscription.IsSubscribedToService = false;

                    var workItemTypeName = newSubscription.WorkItemType.HasValue
                                               ? newSubscription.WorkItemType.ToString()
                                               : "All";

                    Platform.Log(LogLevel.Debug, e, "Failed to subscribe to event type '{0}'", workItemTypeName);
                }
            }
        }

        private void Connect()
        {
            if (_client != null)
                return;

            Platform.Log(LogLevel.Debug, "Attempting to connect to WorkItemService.");

            try
            {
                var callback = new Callback(this);
                var client = Platform.GetDuplexService<IWorkItemActivityMonitorService, IWorkItemActivityCallback>(callback);
                var communication = (ICommunicationObject)client;
                if (communication.State == CommunicationState.Created)
                    communication.Open();

                communication.Closed += OnConnectionClosed;
                communication.Faulted += OnConnectionFaulted;

                Platform.Log(LogLevel.Debug, "Successfully connected to WorkItemService.");

                _client = client;
                FireIsConnectedChanged();
            }
            catch (EndpointNotFoundException)
            {
                Platform.Log(LogLevel.Debug, "WorkItemService is not running.");
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e, "Unexpected error trying to connect to WorkItemService.");
            }
        }

        void OnConnectionFaulted(object sender, EventArgs e)
        {
            DropConnection();
            ManageSubscriptions();
        }

        void OnConnectionClosed(object sender, EventArgs e)
        {
            DropConnection();
            ManageSubscriptions();
        }

        private void DropConnection()
        {
            if (_client == null)
                return;

            Platform.Log(LogLevel.Debug, "Attempting to disconnect from WorkItemService.");

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
                Platform.Log(LogLevel.Debug, e, "Unexpected error disconnecting from WorkItemService.");
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
                ThreadPool.QueueUserWorkItem(ignore => CallDelegates(delegates, EventArgs.Empty));
        }

        private void OnWorkItemChanged(WorkItemData workItemData)
        {
            IList<Delegate> delegates;
            lock (_syncLock)
            {
                if (_disposed)
                    return;

                //The only events we fire are the ones for the matching work item type,
                //or the ones that are for "all" work item types.
                delegates = _workItemChangedEvents.GetChangedDelegates(workItemData.Type);
            }

            if (delegates.Count <= 0)
                return;

            var args = new WorkItemChangedEventArgs(workItemData);
            ThreadPool.QueueUserWorkItem(ignore => CallDelegates(delegates, args));
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