using System;
using System.Linq;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public interface IWorkItemActivityMonitor : IDisposable
    {
        bool IsConnected { get; }
        event EventHandler IsConnectedChanged;

        WorkItemTypeEnum[] WorkItemTypeFilters { get; set; }
        long[] WorkItemIdFilters { get; set; }

        event EventHandler<WorkItemChangedEventArgs> WorkItemChanged;
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

        public abstract WorkItemTypeEnum[] WorkItemTypeFilters { get; set; }
        public abstract long[] WorkItemIdFilters { get; set; }

        public abstract event EventHandler<WorkItemChangedEventArgs> WorkItemChanged;

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

    internal static class WorkItemActivityMonitorExtensions
    {
        public static bool WorkItemMatchesFilters(this IWorkItemActivityMonitor activityMonitor, WorkItemData workItemData)
        {
            var typeFilters = activityMonitor.WorkItemTypeFilters;
            bool matchesTypeFilters = typeFilters == null
                                      || typeFilters.Length == 0
                                      || typeFilters.Contains(workItemData.Type);
            if (!matchesTypeFilters)
                return false;

            var idFilters = activityMonitor.WorkItemIdFilters;
            bool matchesIdFilters = idFilters == null
                                      || idFilters.Length == 0
                                      || idFilters.Contains(workItemData.Identifier);

            return matchesIdFilters;
        }
    }

}
