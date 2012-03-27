#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
	public class WorkItemServiceClient : DuplexClientBase<IWorkItemService>, IWorkItemService
	{
        public WorkItemServiceClient(InstanceContext callbackInstance)
            : base(callbackInstance)
	    {
        }

        #region IWorkItemService Members

        public WorkItemInsertResponse Insert(WorkItemInsertRequest request)
        {
            return base.Channel.Insert(request);
        }

        public WorkItemUpdateResponse Update(WorkItemUpdateRequest request)
        {
            return base.Channel.Update(request);
        }

        public WorkItemQueryResponse Query(WorkItemQueryRequest request)
        {
            return base.Channel.Query(request);
        }

        public WorkItemSubscribeResponse Subscribe(WorkItemSubscribeRequest request)
        {
            return base.Channel.Subscribe(request);
        }

        public WorkItemUnsubscribeResponse Unsubscribe(WorkItemUnsubscribeRequest request)
        {
            return base.Channel.Unsubscribe(request);
        }

        #endregion
    }
}