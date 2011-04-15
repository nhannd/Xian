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

namespace ClearCanvas.Enterprise.Authentication.Hibernate.Brokers
{
    public partial class AuthorityGroupBroker
    {
        public int GetUserCountForGroup(AuthorityGroup group)
        {
            HqlQuery query = new HqlQuery("select count(*) from User u join u.AuthorityGroups g");

            AuthorityGroupSearchCriteria whereGroup = new AuthorityGroupSearchCriteria();
            whereGroup.OID.EqualTo(group.OID);
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("g", whereGroup));
            IList<long> results = ExecuteHql<long>(query);
            return results.Count == 0 ? 0 : (int)results[0];
        }
    }
}
