using System.Collections.Generic;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class ExternalPractitionerBroker
	{
		#region IExternalPractitionerBroker Members

		public IList<Order> GetRelatedOrders(ExternalPractitioner practitioner)
		{
			HqlFrom hqlFrom = new HqlFrom(typeof(Order).Name, "o");
			HqlProjectionQuery query = new HqlProjectionQuery(hqlFrom);

			OrderSearchCriteria criteria = new OrderSearchCriteria();
			criteria.OrderingPractitioner.EqualTo(practitioner);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("o", criteria));

			return ExecuteHql<Order>(query);
		}

		public IList<Visit> GetRelatedVisits(ExternalPractitioner practitioner)
		{
			HqlFrom hqlFrom = new HqlFrom(typeof(Visit).Name, "v");
			hqlFrom.Joins.Add(new HqlJoin("v.Practitioners", "vp"));
			HqlProjectionQuery query = new HqlProjectionQuery(hqlFrom);

			VisitPractitionerSearchCriteria criteria = new VisitPractitionerSearchCriteria();
			criteria.Practitioner.EqualTo(practitioner);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("vp", criteria));

			return ExecuteHql<Visit>(query);
		}

		#endregion
	}
}
