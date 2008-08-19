using System.Collections.Generic;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
	public class WorkQueueAssembler
	{
		public WorkQueueItemSummary CreateWorkQueueItemSummary(WorkQueueItem workQueueItem, IPersistenceContext context)
		{
			WorkQueueItemSummary summary = new WorkQueueItemSummary();

			summary.WorkQueueRef = workQueueItem.GetRef();
			summary.CreationTime = workQueueItem.CreationTime;
			summary.ScheduledTime = workQueueItem.ScheduledTime;
			summary.ExpirationTime = workQueueItem.ExpirationTime;
			summary.User = workQueueItem.User;
			summary.Type = workQueueItem.Type;
			summary.Status = EnumUtils.GetEnumValueInfo(workQueueItem.Status, context);
			summary.ProcessedTime = workQueueItem.ProcessedTime;
			summary.FailureCount = workQueueItem.FailureCount;
			summary.FailureDescription = workQueueItem.FailureDescription;
			summary.ExtendedProperties = new Dictionary<string, string>(workQueueItem.ExtendedProperties);

			return summary;
		}
	}
}