using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Hql;
using System.Collections;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public partial class StaffBroker
    {
        public Staff FindStaffForUser(string userName)
        {
            HqlQuery query = new HqlQuery("from Staff s");
            query.Conditions.Add(new HqlCondition("s.User.UserName = ?", new object[] { userName }));

            IList results = this.ExecuteHql(query);
            if (results.Count > 0)
            {
                return (Staff)results[0];
            }
            else
            {
                throw new EntityNotFoundException(string.Format(SR.ErrorNoStaffForUser, userName), null);
            }
        }
    }
}
