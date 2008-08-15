using System.Collections.Generic;
using ClearCanvas.Enterprise.Hibernate.Hql;

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

			return DoQuery(query);
		}

		#endregion

		protected IList<string> DoQuery(HqlQuery query)
		{
			return ExecuteHql<string>(query);
		}
	}
}
