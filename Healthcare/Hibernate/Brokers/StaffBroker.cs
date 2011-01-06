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
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Hql;
using System.Collections;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public partial class StaffBroker
    {
        // JR: this is no longer needed, but left it here because it is likely we'll need to do something similar in future
        // if we end up defining a User shadow class within Healthcare

        //public Staff FindStaffForUser(string userName)
        //{
        //    HqlQuery query = new HqlQuery("from Staff s");
        //    query.Conditions.Add(new HqlCondition("s.User.UserName = ?", new object[] { userName }));

        //    IList<Staff> results = this.ExecuteHql<Staff>(query);
        //    if (results.Count > 0)
        //    {
        //        return results[0];
        //    }
        //    else
        //    {
        //        throw new EntityNotFoundException(string.Format(SR.ErrorNoStaffForUser, userName), null);
        //    }
        //}
    }
}
