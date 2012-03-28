#if UNIT_TESTS

using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    internal class TestActivityMonitor : IWorkItemActivityMonitor, IWorkItemActivityCallback
    {
        private bool _isConnected;

        #region IWorkItemActivityMonitor Members

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (Equals(value, _isConnected))
                    return;

                _isConnected = value;
                EventsHelper.Fire(IsConnectedChanged, this, EventArgs.Empty);
            }
        }

        public event EventHandler IsConnectedChanged;

        public WorkItemTypeEnum[] WorkItemTypeFilters { get; set; }
        public long[] WorkItemIdFilters { get; set; }

        public event EventHandler<WorkItemChangedEventArgs> WorkItemChanged;

        #endregion

        void IWorkItemActivityCallback.WorkItemChanged(WorkItemData workItemData)
        {
            if (this.WorkItemMatchesFilters(workItemData))
                EventsHelper.Fire(WorkItemChanged, this, new WorkItemChangedEventArgs(workItemData));
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}

#endif