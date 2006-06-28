using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implementation of <see cref="IEnumBroker"/>.
    /// </summary>
    public abstract class EnumBroker<e, E> : Broker, IEnumBroker<e, E>
        where e : struct
        where E : EnumValue<e>
    {
        public EnumTable<e, E> Load()
        {
            HqlQuery q = new HqlQuery( string.Format("from {0}", typeof(E).Name) );
            IList results = ExecuteHql(q);

            Dictionary<e,E> values = new Dictionary<e, E>();
            foreach (E val in results)
            {
                values.Add(val.Code, val);
            }

            return new EnumTable<e, E>(values);
        }
    }
}
