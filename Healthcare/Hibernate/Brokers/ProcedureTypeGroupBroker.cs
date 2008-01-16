// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    /// <summary>
    /// NHibernate implementation of <see cref="IProcedureTypeGroupBroker"/>.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(BrokerExtensionPoint))]
	public partial class ProcedureTypeGroupBroker : IProcedureTypeGroupBroker
	{
        #region IProcedureTypeGroupBroker Members

        public IList<ProcedureTypeGroup> Find(ProcedureTypeGroupSearchCriteria criteria, Type subClass)
        {
            HqlQuery query = new HqlQuery(string.Format("from {0} x", subClass.Name));

            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("x", criteria));
            query.Sorts.AddRange(HqlSort.FromSearchCriteria("x", criteria));

            return ExecuteHql<ProcedureTypeGroup>(query);
        }

        public ProcedureTypeGroup FindOne(ProcedureTypeGroupSearchCriteria criteria, Type subClass)
        {
            IList<ProcedureTypeGroup> groups = Find(criteria, subClass);

            if(groups.Count == 0)
                throw new EntityNotFoundException(null);

            return CollectionUtils.FirstElement(groups);
        }

        #endregion
    }
}