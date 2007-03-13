using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    /// <summary>
    /// Provides support for building HQL queries dynamically.
    /// </summary>
    public class HqlQuery : HqlElement
    {
        private string _baseQuery;
        private HqlAnd _where;
        private List<HqlSort> _sorts;
        private SearchResultPage _page;

        /// <summary>
        /// Constructs an empty query
        /// </summary>
        protected HqlQuery()
            : this(null, new HqlCondition[] { }, new HqlSort[] { }, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseHql">The base HQL statement, without the "where" or "order by" clauses</param>
        public HqlQuery(string baseHql)
            : this(baseHql, new HqlCondition[] { }, new HqlSort[] { }, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseHql"></param>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        public HqlQuery(string baseHql, HqlCondition[] conditions, HqlSort[] sorts, SearchResultPage page)
        {
            _baseQuery = baseHql;
            _page = page;
            _where = new HqlAnd(conditions);
            _sorts = new List<HqlSort>(sorts);
        }

        /// <summary>
        /// Exposes the set of conditions that will form the "where" clause
        /// </summary>
        public List<HqlCondition> Conditions
        {
            get { return _where.Conditions; }
        }

        /// <summary>
        /// Exposes the collection of sorts, which will form the "order by" clause
        /// </summary>
        public List<HqlSort> Sorts
        {
            get { return _sorts; }
        }

        /// <summary>
        /// Gets or sets the result page
        /// </summary>
        public SearchResultPage Page
        {
            get { return _page; }
            set { _page = value; }
        }

        /// <summary>
        /// Gets the base query.  Allows subclasses to set the base query.
        /// </summary>
        public string BaseQuery
        {
            get { return _baseQuery; }
            protected set { _baseQuery = value; }
        }

        /// <summary>
        /// Returns the HQL text of this query.
        /// </summary>
        public override string Hql
        {
            get
            {
                // build the order by clause
                StringBuilder orderBy = new StringBuilder();

                // order the sorts first, so that they are injected into the hql string in the right order
                _sorts.Sort();
                foreach (HqlSort s in _sorts)
                {
                    if (orderBy.Length != 0)
                        orderBy.Append(", ");

                    orderBy.Append(s.Hql);
                }

                // append where and order by to base query
                StringBuilder hql = new StringBuilder();
                hql.Append(_baseQuery);
                if (_where.Conditions.Count > 0)
                {
                    hql.Append(" where ");
                    hql.Append(_where.Hql);
                }
                if (orderBy.Length > 0)
                {
                    hql.Append(" order by ");
                    hql.Append(orderBy.ToString());
                }
                return hql.ToString();
            }
        }

        /// <summary>
        /// Constructs an NHibernate IQuery object from this object.
        /// </summary>
        /// <param name="ctx">The persistence context that wraps the NHibernate session</param>
        /// <returns>an IQuery object</returns>
        internal IQuery BuildHibernateQueryObject(PersistenceContext ctx)
        {
            IQuery q = ctx.Session.CreateQuery(this.Hql);

            // add the parameters to the query
            int i = 0;
            foreach (object val in _where.Parameters)
            {
                if (val is Enum || val is CodedValue)
                {
                    // convert to string, since nhibernate doesn't know what to do with enums
                    q.SetParameter(i++, val.ToString());
                }
                else
                {
                    q.SetParameter(i++, val);
                }

            }

            // if limits were specified, pass them to nhibernate
            if (_page != null)
            {
                if(_page.FirstRow > -1)
                    q.SetFirstResult(_page.FirstRow);
                if(_page.MaxRows > -1)
                    q.SetMaxResults(_page.MaxRows);
            }

            return q;
        }
    }
}
