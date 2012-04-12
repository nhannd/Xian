using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    internal class WorkItemActivityMonitorProxy : WorkItemActivityMonitor
    {
        private readonly IWorkItemActivityMonitor _real;
        private readonly SynchronizationContext _synchronizationContext;

        private event EventHandler _isConnectedChanged;

        private IList<WorkItemTypeEnum> _workItemTypeFilters;
        private IList<long> _workItemIdFilters;
        private event EventHandler<WorkItemChangedEventArgs> _workItemChanged;
        private event EventHandler _studiesCleared;

        private volatile bool _disposed;

        internal WorkItemActivityMonitorProxy(IWorkItemActivityMonitor real, SynchronizationContext synchronizationContext)
        {
            _real = real;
            _synchronizationContext = synchronizationContext;
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

        public override WorkItemTypeEnum[] WorkItemTypeFilters
        {
            get
            {
                return _workItemTypeFilters == null ? null : _workItemTypeFilters.ToArray();
            }
            set
            {
                //NOTE (Marmot): We purposely don't set the filters on the "real" monitor because it's shared among proxies.
                if (value == null || value.Length == 0)
                    _workItemTypeFilters = null;
                else
                    _workItemTypeFilters = value.ToList();
            }
        }

        public override long[] WorkItemIdFilters
        {
            get
            {
                return _workItemIdFilters == null ? null : _workItemIdFilters.ToArray();
            }
            set
            {
                //NOTE (Marmot): We purposely don't set the filters on the "real" monitor because it's shared among proxies.
                if (value == null || value.Length == 0)
                    _workItemIdFilters = null;
                else
                    _workItemIdFilters = value.ToList();
            }
        }

        public override event EventHandler<WorkItemChangedEventArgs> WorkItemChanged
        {
            add
            {
                CheckDisposed();

                bool subscribeToReal = _workItemChanged == null;
                if (subscribeToReal)
                    _real.WorkItemChanged += OnWorkItemChanged;

                _workItemChanged += value;
            }
            remove
            {
                CheckDisposed();

                _workItemChanged -= value;

                bool unsubscribeFromReal = _workItemChanged == null;
                if (unsubscribeFromReal)
                    _real.WorkItemChanged -= OnWorkItemChanged;
            }
        }

        public override event EventHandler StudiesCleared
        {
            add
            {
                CheckDisposed();

                bool subscribeToReal = _studiesCleared == null;
                if (subscribeToReal)
                    _real.StudiesCleared += OnStudiesCleared;

                _studiesCleared += value;
            }
            remove
            {
                CheckDisposed();

                _studiesCleared -= value;

                bool unsubscribeFromReal = _studiesCleared == null;
                if (unsubscribeFromReal)
                    _real.StudiesCleared -= OnStudiesCleared;
            }
        }

        private void OnIsConnectedChanged(object sender, EventArgs e)
        {
            if (_synchronizationContext != null)
                _synchronizationContext.Post(ignore => FireIsConnectedChanged(), null);
            else
                FireIsConnectedChanged();
        }

        private void OnWorkItemChanged(object sender, WorkItemChangedEventArgs e)
        {
            if (_synchronizationContext != null)
                _synchronizationContext.Post(ignore => FireWorkItemChanged(e), null);
            else
                FireWorkItemChanged(e);
        }

        private void OnStudiesCleared(object sender, EventArgs e)
        {
            if (_synchronizationContext != null)
                _synchronizationContext.Post(ignore => FireStudiesCleared(e), null);
            else
                FireStudiesCleared(e);
        }

        private void FireIsConnectedChanged()
        {
            if (!_disposed)
                EventsHelper.Fire(_isConnectedChanged, this, EventArgs.Empty);
        }

        private void FireWorkItemChanged(WorkItemChangedEventArgs e)
        {
            if (!_disposed && this.WorkItemMatchesFilters(e.ItemData))
                EventsHelper.Fire(_workItemChanged, this, e);
        }

        private void FireStudiesCleared(EventArgs e)
        {
            if (!_disposed)
                EventsHelper.Fire(_studiesCleared, this, e);
        }

        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("WorkItemActivityMonitor has already been disposed.");
        }

        protected override void Dispose(bool disposing)
        {
            _disposed = true;

            if (_isConnectedChanged != null)
                _real.IsConnectedChanged -= OnIsConnectedChanged;

            if (_workItemChanged != null)
                _real.WorkItemChanged -= OnWorkItemChanged;

            OnProxyDisposed();
        }
    }
}