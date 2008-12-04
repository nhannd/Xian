using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common;
using System;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class WorkQueueItemBroker
	{
		private static readonly HqlFrom FromWorkQueueItem = new HqlFrom("WorkQueueItem", "item");

		private static readonly HqlSelect SelectType = new HqlSelect("item.Type");

		#region IWorkQueueItemBroker Members

		public IList<string> GetTypes()
		{
			HqlProjectionQuery query = new HqlProjectionQuery(
				FromWorkQueueItem,
				new HqlSelect[] { SelectType });

			query.SelectDistinct = true;

			return ExecuteHql<string>(query);
		}

		public IList<WorkQueueItem> GetPendingItems(string type, int maxItems)
		{
			HqlQuery query = new HqlQuery("from WorkQueueItem item");
			query.Conditions.Add(new HqlCondition("item.Type = ?", type));
			query.Conditions.Add(new HqlCondition("item.Status = ?", WorkQueueStatus.PN));

			DateTime now = Platform.Time;
			query.Conditions.Add(new HqlCondition("(item.ScheduledTime is null or item.ScheduledTime < ?)", now));
			query.Conditions.Add(new HqlCondition("(item.ExpirationTime is null or item.ExpirationTime > ?)", now));
            query.Sorts.Add(new HqlSort("item.ScheduledTime", true, 0));
			query.Page = new SearchResultPage(0, maxItems);

			return ExecuteHql<WorkQueueItem>(query);
		}

		#endregion
	}
}
