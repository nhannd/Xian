using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Hibernate.Hql;
using NHibernate;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

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
            return ExecuteHql(query.BuildHibernateQueryObject(_ctx));
        }

        /// <summary>
        /// Executes the specified <see cref="NHibernate.IQuery"/> against the database, returning the results
        /// as an untyped <see cref="IList"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IList ExecuteHql(NHibernate.IQuery query)
        {
            return query.List();
        }

        protected IList<T> MakeTypeSafe<T>(IList list)
        {
            return new TypeSafeListWrapper<T>(list);
        }
    }
}
