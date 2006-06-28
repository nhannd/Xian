using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// NHibernate implemenation of <see cref="IReadContext"/>.
    /// </summary>
    public class ReadContext : PersistenceContext, IReadContext
    {
        internal ReadContext(ISessionFactory sessionFactory)
            :base(sessionFactory, true)
        {
        }
    }
}
