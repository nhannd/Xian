using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Authentication.Hibernate.Brokers
{
    public partial class AuthorityTokenBroker
    {
        public string[] FindTokensByUserName(string userName)
        {
            UserSearchCriteria where = new UserSearchCriteria();
            where.UserName.EqualTo(userName);

            // want this to be as fast as possible - use joins and only select the AuthorityToken names
            HqlQuery query = new HqlQuery("select distinct t.Name from User u join u.AuthorityGroups g join g.AuthorityTokens t");
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
