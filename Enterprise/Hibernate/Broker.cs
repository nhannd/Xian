using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Hibernate.Hql;
using NHibernate;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Abstract base class for all NHibernate broker implementations.
    /// </summary>
    public abstract class Broker : IPersistenceBroker
    {
        private PersistenceContext _ctx;

        /// <summary>
        /// Returns the persistence context associated with this broker instance.
        /// </summary>
        protected PersistenceContext Context
        {
            get { return _ctx; }
        }

        public void SetContext(IPersistenceContext context)
        {
            _ctx = (PersistenceContext)context;
        }

        /// <summary>
        /// Executes the specified <see cref="HqlQuery"/> against the database, returning the results
        /// as an untyped <see cref="IList"/>.
        /// </summary>
        /// <param name="query">the query to execute</param>
        /// <returns>the result set</returns>
        public IList ExecuteHql(HqlQuery query)
        {
            IQuery hibQuery = query.BuildHibernateQueryObject(_ctx);
            return hibQuery.List();
        }
    }
}
