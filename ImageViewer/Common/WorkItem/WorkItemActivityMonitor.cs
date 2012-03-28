using System;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public class WorkItemChangedEventArgs : EventArgs
    {
        public WorkItemChangedEventArgs(WorkItemData itemData)
        {
            ItemData = itemData;
        }

        public WorkItemData ItemData { get; private set; }
    }

    public interface IWorkItemActivityMonitor : IDisposable
    {
        bool IsConnected { get; }
        event EventHandler IsConnectedChanged;

        void Subscribe(WorkItemTypeEnum? workItemType, EventHandler<WorkItemChangedEventArgs> eventHandler);
        void Unsubscribe(WorkItemTypeEnum? workItemType, EventHandler<WorkItemChangedEventArgs> eventHandler);
    }

    public abstract class WorkItemActivityMonitor : IWorkItemActivityMonitor
    {
        private static readonly object _instanceLock = new object();
        private static RealWorkItemActivityMonitor _instance;
        internal static volatile int _proxyCount = 0;

        internal WorkItemActivityMonitor()
        {
        }

        ~WorkItemActivityMonitor()
        {
            Dispose(false);
        }

        #region IWorkItemActivityMonitor Members

        public abstract bool IsConnected { get; }
        public abstract event EventHandler IsConnectedChanged;

        public abstract void Subscribe(WorkItemTypeEnum? workItemType, EventHandler<WorkItemChangedEventArgs> eventHandler);
        public abstract void Unsubscribe(WorkItemTypeEnum? workItemType, EventHandler<WorkItemChangedEventArgs> eventHandler);

        #endregion

        public static IWorkItemActivityMonitor Create()
        {
            return Create(true);
        }

        public static IWorkItemActivityMonitor Create(bool useSynchronizationContext)
        {
            var syncContext = useSynchronizationContext ? SynchronizationContext.Current : null;
            if (useSynchronizationContext && syncContext == null)
                throw new ArgumentException("Current thread has no synchronization context.", "useSynchronizationContext");

            return Create(syncContext);
        }

        public static IWorkItemActivityMonitor Create(SynchronizationContext synchronizationContext)
        {
            lock (_instanceLock)
            {
                if (_instance == null)
                    _instance = new RealWorkItemActivityMonitor();

                ++_proxyCount;
                Platform.Log(LogLevel.Debug, "WorkItemActivityMonitor proxy created (count = {0})", _proxyCount);
                return new WorkItemActivityMonitorProxy(_instance, synchronizationContext);
            }
        }

        internal static void OnProxyDisposed()
        {
            lock (_instanceLock)
            {
                if (_proxyCount == 0)
                    return; //Should never happen, except possibly when there's unit tests running.

                --_proxyCount;
                Platform.Log(LogLevel.Debug, "WorkItemActivityMonitor proxy disposed (count = {0}).", _proxyCount);
                if (_proxyCount > 0)
                    return;

                var monitor = _instance;
                _instance = null;
                //No need to do this synchronously.
                ThreadPool.QueueUserWorkItem(ignore => monitor.Dispose());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e, "Unexpected error disposing WorkItemActivityMonitor.");
            }
        }

        #endregion
    }

}
