using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Hibernate;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public abstract class WorklistItemBrokerBase<TItem> : Broker
    {
        protected List<TItem> DoQuery(HqlQuery query)
        {
            IList<object[]> list = ExecuteHql<object[]>(query);
            List<TItem> results = new List<TItem>();
            foreach (object[] tuple in list)
            {
                TItem item = (TItem)Activator.CreateInstance(typeof(TItem), tuple);
                results.Add(item);
            }

            return results;
        }

        protected int DoQueryCount(HqlQuery query)
        {
            return (int)ExecuteHqlUnique<long>(query);
        }
    }
}
