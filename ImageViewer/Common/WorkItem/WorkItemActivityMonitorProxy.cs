using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    internal struct WorkItemChangedEventProxyKey
    {
        private readonly WorkItemTypeEnum? _workItemType;
        private readonly EventHandler<WorkItemChangedEventArgs> _realHandler;

        public WorkItemChangedEventProxyKey(WorkItemTypeEnum? workItemType,
                                            EventHandler<WorkItemChangedEventArgs> realHandler)
        {
            _workItemType = workItemType;
            _realHandler = realHandler;
        }

        public WorkItemTypeEnum? WorkItemType { get { return _workItemType; } }
        public EventHandler<WorkItemChangedEventArgs> RealHandler { get { return _realHandler; } }
    }

    internal class WorkItemChangedEventProxy
    {
        private readonly EventHandler<WorkItemChangedEventArgs> _realHandler;
        private readonly SynchronizationContext _synchronizationContext;

        public WorkItemChangedEventProxy(EventHandler<WorkItemChangedEventArgs> realHandler, SynchronizationContext synchronizationContext)
        {
            _realHandler = realHandler;
            _synchronizationContext = synchronizationContext;
        }

        public void OnWorkItemChanged(object sender, WorkItemChangedEventArgs e)
        {
            if (_synchronizationContext != null)
                _synchronizationContext.Post(ignore => _realHandler(sender, e), null);
            else
                _realHandler(sender, e);
        }
    }

    internal class WorkItemActivityMonitorProxy : WorkItemActivityMonitor
    {
        private readonly IWorkItemActivityMonitor _real;
        private readonly SynchronizationContext _synchronizationContext;

        private readonly Dictionary<WorkItemChangedEventProxyKey, WorkItemChangedEventProxy> _eventProxies;
        private event EventHandler _isConnectedChanged;
        private volatile bool _disposed;

        internal WorkItemActivityMonitorProxy(IWorkItemActivityMonitor real, SynchronizationContext synchronizationContext)
        {
            _real = real;
            _synchronizationContext = synchronizationContext;
            _eventProxies = new Dictionary<WorkItemChangedEventProxyKey, WorkItemChangedEventProxy>();
        }

        public override bool IsConnected
        {
            get
            {
                CheckDisposed();
                return _real.IsConnected;
            }
        }

        public override event EventHandler IsConnectedChanged
        {
            add
            {
                CheckDisposed();
                
                bool subscribeToReal = _isConnectedChanged == null;
                if (subscribeToReal)
                    _real.IsConnectedChanged += OnIsConnectedChanged;

                _isConnectedChanged += value;
            }
            remove
            {
                CheckDisposed();

                _isConnectedChanged -= value;

                bool unsubscribeFromReal = _isConnectedChanged == null;
                if (unsubscribeFromReal)
                    _real.IsConnectedChanged -= OnIsConnectedChanged;
            }
        }

        public override void Subscribe(WorkItemTypeEnum? workItemType, EventHandler<WorkItemChangedEventArgs> eventHandler)
        {
            CheckDisposed();
            var key = new WorkItemChangedEventProxyKey(workItemType, eventHandler);
            if (_eventProxies.ContainsKey(key))
                return;

            var eventProxy = new WorkItemChangedEventProxy(eventHandler, _synchronizationContext);
            _eventProxies[key] = eventProxy;
            _real.Subscribe(workItemType, eventProxy.OnWorkItemChanged);
        }

        public override void Unsubscribe(WorkItemTypeEnum? workItemType, EventHandler<WorkItemChangedEventArgs> eventHandler)
        {
            CheckDisposed();
            var key = new WorkItemChangedEventProxyKey(workItemType, eventHandler);
            if (!_eventProxies.ContainsKey(key))
                return;

            _real.Unsubscribe(workItemType, _eventProxies[key].OnWorkItemChanged);
        }

        private void OnIsConnectedChanged(object sender, EventArgs e)
        {
            if (_synchronizationContext != null)
                _synchronizationContext.Post(ignore => FireIsConnectedChanged(), null);
            else
                FireIsConnectedChanged();
        }

        private void FireIsConnectedChanged()
        {
            if (!_disposed)
                EventsHelper.Fire(_isConnectedChanged, this, EventArgs.Empty);
        }

        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("WorkItemActivityMonitor has already been disposed.");
        }

        protected override void Dispose(bool disposing)
        {
            _disposed = true;

            foreach (var eventProxy in _eventProxies)
                _real.Unsubscribe(eventProxy.Key.WorkItemType, eventProxy.Value.OnWorkItemChanged);

            if (_isConnectedChanged != null)
                _real.IsConnectedChanged -= OnIsConnectedChanged;

            OnProxyDisposed();
        }
    }
}