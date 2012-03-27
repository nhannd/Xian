using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    internal class TestActivityMonitor : IWorkItemActivityMonitor
    {
        private bool _isConnected;
        private readonly WorkItemChangedEventWrappers _workItemChangedEvents;

        public TestActivityMonitor()
        {
            _workItemChangedEvents = new WorkItemChangedEventWrappers();
        }

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

        public void Subscribe(WorkItemTypeEnum? workItemType, System.EventHandler<WorkItemChangedEventArgs> eventHandler)
        {
            var wrapper = _workItemChangedEvents[workItemType];
            wrapper.Changed += eventHandler;
            wrapper.IsSubscribedToService = true;
        }

        public void Unsubscribe(WorkItemTypeEnum? workItemType, System.EventHandler<WorkItemChangedEventArgs> eventHandler)
        {
            var wrapper = _workItemChangedEvents[workItemType];
            wrapper.Changed -= eventHandler;
            wrapper.IsSubscribedToService = false;
        }

        public void OnWorkItemChanged(WorkItemData workItemData)
        {
            var args = new WorkItemChangedEventArgs(workItemData);
            var delegates = _workItemChangedEvents.GetChangedDelegates(workItemData.Type);
            foreach (var @delegate in delegates)
                @delegate.DynamicInvoke(this, args);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}