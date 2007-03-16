using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Authentication.Hibernate.Brokers
{
    public partial class PermissionBroker
    {
        public string[] FindPermissionsByUserName(string userName)
        {
            UserSearchCriteria where = new UserSearchCriteria();
            where.UserName.EqualTo(userName);

            // want this to be as fast as possible - use joins and only select the Permission objects
            HqlQuery query = new HqlQuery("select distinct p.PermissionName from User u join u.Groups g join g.Permissions p");
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("u", where));

            return CollectionUtils.Map<object[], string, List<string>>(
                this.ExecuteHql(query),
                delegate(object[] tuple)
                {
                    return (string)tuple[0];
                }).ToArray();
        }
    }
}
