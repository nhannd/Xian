#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
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

        public Guid[] FindDataGroupsByUserName(string userName)
        {
            UserSearchCriteria where = new UserSearchCriteria();
            where.UserName.EqualTo(userName);

            AuthorityGroupSearchCriteria groupWhere = new AuthorityGroupSearchCriteria();
            groupWhere.DataGroup.EqualTo(true);

            // want this to be as fast as possible - use joins and only select the AuthorityToken names
            HqlQuery query = new HqlQuery("select distinct g.OID from User u join u.AuthorityGroups g");
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("u", where));
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("g", groupWhere));

            // take advantage of query caching if possible
            query.Cacheable = true;

            IList<Guid> oids = ExecuteHql<Guid>(query);
            var result = new Guid[oids.Count];
            oids.CopyTo(result, 0);
            return result;
        }
	}
}
