#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services
{
	public class WorkQueueItemAssembler
	{
		public WorkQueueItemSummary CreateWorkQueueItemSummary(WorkQueueItem workQueueItem, IPersistenceContext context)
		{
			WorkQueueItemSummary summary = new WorkQueueItemSummary();

			summary.WorkQueueItemRef = workQueueItem.GetRef();
			summary.CreationTime = workQueueItem.CreationTime;
			summary.ScheduledTime = workQueueItem.ScheduledTime;
			summary.ExpirationTime = workQueueItem.ExpirationTime;
			summary.User = workQueueItem.User;
			summary.Type = workQueueItem.Type;
			summary.Status = EnumUtils.GetEnumValueInfo(workQueueItem.Status, context);
			summary.ProcessedTime = workQueueItem.ProcessedTime;
			summary.FailureCount = workQueueItem.FailureCount;
			summary.FailureDescription = workQueueItem.FailureDescription;

			return summary;
		}

		public WorkQueueItemDetail CreateWorkQueueItemDetail(WorkQueueItem workQueueItem, IPersistenceContext context)
		{
			WorkQueueItemDetail detail = new WorkQueueItemDetail();

			detail.WorkQueueItemRef = workQueueItem.GetRef();
			detail.CreationTime = workQueueItem.CreationTime;
			detail.ScheduledTime = workQueueItem.ScheduledTime;
			detail.ExpirationTime = workQueueItem.ExpirationTime;
			detail.User = workQueueItem.User;
			detail.Type = workQueueItem.Type;
			detail.Status = EnumUtils.GetEnumValueInfo(workQueueItem.Status, context);
			detail.ProcessedTime = workQueueItem.ProcessedTime;
			detail.FailureCount = workQueueItem.FailureCount;
			detail.FailureDescription = workQueueItem.FailureDescription;
			detail.ExtendedProperties = ExtendedPropertyUtils.Copy(workQueueItem.ExtendedProperties);

			return detail;
		}
	}
}