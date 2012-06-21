#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    internal class WorkItemActivityMonitorProxy : WorkItemActivityMonitor
    {
        private readonly IWorkItemActivityMonitor _real;
        private readonly SynchronizationContext _synchronizationContext;

        private event EventHandler _isConnectedChanged;

        private event EventHandler<WorkItemsChangedEventArgs> _workItemsChanged;
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

        public override event EventHandler<WorkItemsChangedEventArgs> WorkItemsChanged
        {
            add
            {
                CheckDisposed();

                bool subscribeToReal = _workItemsChanged == null;
                if (subscribeToReal)
                    _real.WorkItemsChanged += OnWorkItemsChanged;

                _workItemsChanged += value;
            }
            remove
            {
                CheckDisposed();

                _workItemsChanged -= value;

                bool unsubscribeFromReal = _workItemsChanged == null;
                if (unsubscribeFromReal)
                    _real.WorkItemsChanged -= OnWorkItemsChanged;
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

        private void OnWorkItemsChanged(object sender, WorkItemsChangedEventArgs e)
        {
            if (_synchronizationContext != null)
                _synchronizationContext.Post(ignore => FireWorkItemsChanged(e), null);
            else
                FireWorkItemsChanged(e);
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

        private void FireWorkItemsChanged(WorkItemsChangedEventArgs e)
        {
            if (!_disposed)
                EventsHelper.Fire(_workItemsChanged, this, e);
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

            // TODO (CR Jun 2012 - Med): should check disposing == true or else OnProxyDisposed won't get called.
            if (_isConnectedChanged != null)
                _real.IsConnectedChanged -= OnIsConnectedChanged;

            if (_workItemsChanged != null)
                _real.WorkItemsChanged -= OnWorkItemsChanged;

            OnProxyDisposed();
        }
    }
}