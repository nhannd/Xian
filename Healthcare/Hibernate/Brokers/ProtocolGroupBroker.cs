using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public partial class ProtocolGroupBroker
    {
        #region IProtocolGroupBroker Members

        public IList<ProtocolGroup> FindAll(RequestedProcedureType procedureType)
        {
            if (procedureType == null)
                return new List<ProtocolGroup>();

            string hql =
                "select p from ProtocolGroup p"
                + " join p.ReadingGroups r "
                + " join r.RequestedProcedureTypes t"
                + " where t = :requesteProcedureType";

            IQuery query = this.Context.CreateHibernateQuery(hql);
            query.SetParameter("requesteProcedureType", procedureType);
            return query.List<ProtocolGroup>();
        }

        #endregion
    }
}
