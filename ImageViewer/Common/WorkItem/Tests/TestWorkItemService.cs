#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common.WorkItem.Tests
{
    internal class TestWorkItemService : IWorkItemService, IWorkItemActivityCallback, ICommunicationObject
    {
        private readonly object _syncLock = new object();
        private readonly Dictionary<WorkItemTypeEnum, WorkItemTypeEnum> _subscribedTypes;
        private volatile bool _subscribedToAll;
        private volatile CommunicationState _state;

        public TestWorkItemService()
        {
            State = CommunicationState.Opened;
            _subscribedTypes = new Dictionary<WorkItemTypeEnum, WorkItemTypeEnum>();
        }

        public IWorkItemActivityCallback Callback { get; set; }

        #region IWorkItemService Members

        public WorkItemInsertResponse Insert(WorkItemInsertRequest request)
        {
            throw new NotImplementedException();
        }

        public WorkItemUpdateResponse Update(WorkItemUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public WorkItemQueryResponse Query(WorkItemQueryRequest request)
        {
            throw new NotImplementedException();
        }

        public WorkItemSubscribeResponse Subscribe(WorkItemSubscribeRequest request)
        {
            lock (_syncLock)
            {
                if (!request.Type.HasValue)
                    _subscribedToAll = true;
                else
                    _subscribedTypes[request.Type.Value] = request.Type.Value;
            }

            return new WorkItemSubscribeResponse();
        }

        public WorkItemUnsubscribeResponse Unsubscribe(WorkItemUnsubscribeRequest request)
        {
            lock (_syncLock)
            {
                if (!request.Type.HasValue)
                    _subscribedToAll = false;
                else
                    _subscribedTypes.Remove(request.Type.Value);
            }

            return new WorkItemUnsubscribeResponse();
        }

        #endregion

        void IWorkItemActivityCallback.WorkItemChanged(WorkItemData workItemData)
        {
            IWorkItemActivityCallback callback;
            lock (_syncLock)
            {
                if (_subscribedToAll || _subscribedTypes.ContainsKey(workItemData.Type))
                    callback = Callback;
                else
                    return;
            }

            callback.WorkItemChanged(workItemData);
        }

        #region ICommunicationObject Members

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void Close(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            lock (_subscribedTypes)
            {
                _subscribedTypes.Clear();
                _subscribedToAll = false;
            }

            State = CommunicationState.Closed;
            EventsHelper.Fire(Closed, this, EventArgs.Empty);
        }

        public event EventHandler Closed;

        public event EventHandler Closing;

        public void EndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void EndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public event EventHandler Faulted;

        public void Open(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            State = CommunicationState.Opened;
            EventsHelper.Fire(Opened, this ,EventArgs.Empty);
        }

        public event EventHandler Opened;

        public event EventHandler Opening;

        public CommunicationState State
        {
            get { return _state; }
            private set { _state = value; }
        }

        #endregion

        public void Fault()
        {
            lock (_subscribedTypes)
            {
                _subscribedTypes.Clear();
                _subscribedToAll = false;
            }

            State = CommunicationState.Faulted;
            EventsHelper.Fire(Faulted, this, EventArgs.Empty);
        }
    }
}

#endif