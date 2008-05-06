using System.Collections.Generic;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class CannedTextBroker
	{
		#region ICannedTextBroker Members

		public IList<CannedText> FindCannedTextForStaff(Staff staff)
		{
            HqlProjectionQuery query = GetBaseQuery();
            AddStaffConditions(query, staff);

            return ExecuteHql<CannedText>(query);
		}

		#endregion

		private HqlProjectionQuery GetBaseQuery()
		{
			HqlProjectionQuery query = new HqlProjectionQuery(new HqlFrom("CannedText", "c"));
			query.Selects.Add(new HqlSelect("c"));
			return query;
		}

		private void AddStaffConditions(HqlProjectionQuery query, Staff staff)
		{
			query.Froms.Add(new HqlFrom("Staff", "s"));
			query.Conditions.Add(new HqlCondition("s = ?", staff));

			HqlOr staffOr = new HqlOr();
			staffOr.Conditions.Add(new HqlCondition("s in elements(c.StaffSubscribers)"));
			staffOr.Conditions.Add(new HqlCondition("s in (select elements(sg.Members) from StaffGroup sg where sg in elements(c.GroupSubscribers))"));
			query.Conditions.Add(staffOr);
		}
	}
}
