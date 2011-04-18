#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Enterprise.Authentication.Hibernate.Brokers
{
	public partial class AuthorityGroupBroker
	{
		public int GetUserCountForGroup(AuthorityGroup group)
		{
			var q = new HqlQuery("select count(elements(g.Users)) from AuthorityGroup g");
			q.Conditions.Add(new HqlCondition("g = ?", group));
			return (int)ExecuteHqlUnique<long>(q);
		}
	}
}
