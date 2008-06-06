using System.Collections.Generic;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class ExternalPractitionerContactPointBroker
	{
		#region IExternalPractitionerContactPointBroker Members

		public IList<Order> GetRelatedOrders(ExternalPractitionerContactPoint contactPoint)
		{
			HqlFrom hqlFrom = new HqlFrom(typeof(Order).Name, "o");
			hqlFrom.Joins.Add(new HqlJoin("o.ResultRecipients", "rr"));

			HqlProjectionQuery query = new HqlProjectionQuery(hqlFrom);

			ResultRecipientSearchCriteria criteria = new ResultRecipientSearchCriteria();
			criteria.PractitionerContactPoint.EqualTo(contactPoint);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("rr", criteria));

			return ExecuteHql<Order>(query);
		}

		#endregion
	}
}
