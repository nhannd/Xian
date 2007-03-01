using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implementation of <see cref="IEnumBroker"/>.
    /// </summary>
    public abstract class EnumBroker<E, ETable> : Broker, IEnumBroker<E, ETable>
        where ETable : new()
    {
        public ETable Load()
        {
            //HqlQuery q = new HqlQuery( string.Format("from {0}", typeof(E).Name) );
            //return MakeTypeSafe<E>( ExecuteHql(q) );
            return new ETable();
        }
    }
}
