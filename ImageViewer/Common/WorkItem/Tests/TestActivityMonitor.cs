#if UNIT_TESTS

using System;
using System.Collections.Generic;
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

        public event EventHandler<WorkItemsChangedEventArgs> WorkItemsChanged;
        public event EventHandler StudiesCleared;
        
        public void Refresh()
    	{
    		throw new NotImplementedException();
    	}

    	#endregion

        void IWorkItemActivityCallback.WorkItemsChanged(List<WorkItemData> workItems)
        {
            EventsHelper.Fire(WorkItemsChanged, this, new WorkItemsChangedEventArgs(workItems));
        }

        void IWorkItemActivityCallback.StudiesCleared()
        {
            EventsHelper.Fire(StudiesCleared, this, EventArgs.Empty);
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}

#endif