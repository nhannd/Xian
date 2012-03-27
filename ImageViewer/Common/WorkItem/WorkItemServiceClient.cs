#region License

// Copyright (c) 2012, ClearCanvas Inc.
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

	    public WorkItemInsertResponse Insert(WorkItemInsertRequest request)
	    {
	        return Channel.Insert(request);
	    }

	    public WorkItemUpdateResponse Update(WorkItemUpdateRequest request)
	    {
	        return Channel.Update(request);
	    }

	    public WorkItemQueryResponse Query(WorkItemQueryRequest request)
	    {
	        return Channel.Query(request);
	    }

	    public WorkItemSubscribeResponse Subscribe(WorkItemSubscribeRequest request)
	    {
	        return Channel.Subscribe(request);
	    }

	    public WorkItemUnsubscribeResponse Unsubscribe(WorkItemUnsubscribeRequest request)
	    {
	        return Channel.Unsubscribe(request);
	    }
	}
}
