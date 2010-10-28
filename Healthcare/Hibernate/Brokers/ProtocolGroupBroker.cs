#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public partial class ProtocolGroupBroker
    {
        #region IProtocolGroupBroker Members

        public IList<ProtocolGroup> FindAll(ProcedureType procedureType)
        {
            if (procedureType == null)
                return new List<ProtocolGroup>();

            string hql =
                "select distinct p from ProtocolGroup p"
                + " join p.ReadingGroups r "
                + " join r.ProcedureTypes t"
                + " where t = :requesteProcedureType";

            IQuery query = this.CreateHibernateQuery(hql);
            query.SetParameter("requesteProcedureType", procedureType);
            return query.List<ProtocolGroup>();
        }

        #endregion
    }
}
