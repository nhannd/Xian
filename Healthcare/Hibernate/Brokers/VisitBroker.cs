#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion


using System.Collections.Generic;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class VisitBroker
	{
		#region IVisitBroker members

		public IList<Visit> FindByVisitPractitioner(VisitSearchCriteria visitSearchCriteria, VisitPractitionerSearchCriteria practitionerSearchCriteria)
		{
			return FindByVisitPractitioner(visitSearchCriteria, practitionerSearchCriteria, new SearchResultPage());
		}

		public IList<Visit> FindByVisitPractitioner(VisitSearchCriteria visitSearchCriteria, VisitPractitionerSearchCriteria practitionerSearchCriteria, SearchResultPage page)
		{
			var query = GetBaseVisitPractitionerQuery(visitSearchCriteria, practitionerSearchCriteria);
			query.Page = page;
			return ExecuteHql<Visit>(query);
		}

		public long CountByVisitPractitioner(VisitSearchCriteria visitSearchCriteria, VisitPractitionerSearchCriteria practitionerSearchCriteria)
		{
			var query = GetBaseVisitPractitionerQuery(visitSearchCriteria, practitionerSearchCriteria);
			query.Selects.Add(new HqlSelect("count(*)"));
			return ExecuteHqlUnique<long>(query);
		}

		#endregion

		private static HqlProjectionQuery GetBaseVisitPractitionerQuery(VisitSearchCriteria visitSearchCriteria, VisitPractitionerSearchCriteria practitionerSearchCriteria)
		{
			var hqlFrom = new HqlFrom(typeof(Visit).Name, "v");
			hqlFrom.Joins.Add(new HqlJoin("v.Practitioners", "vp"));

			var query = new HqlProjectionQuery(hqlFrom);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("vp", practitionerSearchCriteria));
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("v", visitSearchCriteria));
			return query;
		}
	}
}
