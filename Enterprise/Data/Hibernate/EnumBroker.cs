using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Data.Hibernate.Hql;

namespace ClearCanvas.Enterprise.Data.Hibernate
{
    /// <summary>
    /// NHibernate implementation of <see cref="IEnumBroker"/>.
    /// </summary>
    public abstract class EnumBroker<E> : Broker, IEnumBroker<E>
    {
        public IList<E> Load()
        {
            HqlQuery q = new HqlQuery( string.Format("from {0}", typeof(E).Name) );
            return MakeTypeSafe<E>( ExecuteHql(q) );
        }
    }
}
