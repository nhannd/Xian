#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common;

namespace ClearCanvas.Workflow.Hibernate.Brokers
{
	public partial class WorkQueueItemBroker
	{
		#region IWorkQueueItemBroker Members

		public IList<WorkQueueItem> GetPendingItems(string type, int maxItems)
		{
			var query = new HqlQuery("from WorkQueueItem item");
			query.Conditions.Add(new HqlCondition("item.Type = ?", type));
			query.Conditions.Add(new HqlCondition("item.Status = ?", WorkQueueStatus.PN));

			var now = Platform.Time;
			query.Conditions.Add(new HqlCondition("item.ScheduledTime < ?", now));
			query.Conditions.Add(new HqlCondition("(item.ExpirationTime is null or item.ExpirationTime > ?)", now));
			query.Sorts.Add(new HqlSort("item.ScheduledTime", true, 0));
			query.Page = new SearchResultPage(0, maxItems);

			return ExecuteHql<WorkQueueItem>(query);
		}

		#endregion
	}
}
