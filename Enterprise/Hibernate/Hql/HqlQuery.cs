using System;
using System.Collections.Generic;
using System.Text;

using NHibernate;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    /// <summary>
    /// Provides support for building HQL queries dynamically from <see cref="SearchCriteria"/> objects.
    /// </summary>
    public class HqlQuery
    {
        /// <summary>
        /// Creates an <see cref="HqlQuery"/> from the specified <see cref="SearchCriteria"/>.
        /// </summary>
        /// <param name="baseHql">The base HQL query, without any criteria</param>
        /// <param name="alias">The alias used in the base criteria that must be prepended to the criteria</param>
        /// <param name="criteria">The search criteria</param>
        /// <returns>a new instance of <see cref="HqlQuery"/></returns>
        /// <example>
        /// Given an entity class Person, and a corresponding PersonSearchCriteria class with a LastName field,
        /// a query might be constructed like so
        /// <code>
        ///     HqlQuery query = HqlQuery.FromSearchCriteria("from Person p", "p", criteria);
        /// </code>
        /// This would produce the following HQL:
        ///     "from Person p where p.LastName = ?"
        /// </example>
        public static HqlQuery FromSearchCriteria(string baseHql, string alias, SearchCriteria criteria)
        {
            return FromSearchCriteria(baseHql, new string[] { alias }, new SearchCriteria[] { criteria }, criteria.FirstRow, criteria.MaxRows);
        }

        /// <summary>
        /// Creates an <see cref="HqlQuery"/> from multiple <see cref="SearchCriteria"/> classes.  Note that since multiple
        /// <see cref="SearchCriteria"/> objects are passed, the <see cref="SearchCriteria.FirstRow"/> and <see cref="SearchCriteria.MaxRows"/>
        /// members of any given criteria object are ignored.  Instead, these values must be passed explicitly.
        /// </summary>
        /// <param name="baseHql">The base HQL query, without any criteria</param>
        /// <param name="alias">The aliases used in the base criteria that must be prepended to the criteria</param>
        /// <param name="criteria">The search criteria</param>
        /// <returns>a new instance of <see cref="HqlQuery"/></returns>
        public static HqlQuery FromSearchCriteria(string baseHql, string[] aliases, SearchCriteria[] criteria, int firstRow, int maxRows)
        {
            if (aliases.Length != criteria.Length)
            {
                throw new ArgumentException();  // TODO elaborate
            }

            HqlQuery q = new HqlQuery(baseHql, firstRow, maxRows);
            for (int i = 0; i < criteria.Length; i++)
            {
                q.AddConditions(HqlCondition.FromSearchCriteria(aliases[i], criteria[i]));
                q.AddSorts(HqlSort.FromSearchCriteria(aliases[i], criteria[i]));
            }
            return q;
        }

        private string _baseQuery;
        private List<HqlCondition> _conditions;
        private List<HqlSort> _sorts;
        private int _firstRow;
        private int _maxRows;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseHql">The base HQL statement, without the "where" or "order by" clauses</param>
        /// <param name="firstRow">First record to retrieve</param>
        /// <param name="maxRows">Maximum number of records to retrieve</param>
        public HqlQuery(string baseHql, int firstRow, int maxRows)
        {
            _baseQuery = baseHql;
            _firstRow = firstRow;
            _maxRows = maxRows;

            _conditions = new List<HqlCondition>();
            _sorts = new List<HqlSort>();
        }

        /// <summary>
        /// Constructor creates a query that will retrieve the first 100 rows at most.
        /// </summary>
        /// <param name="baseHql">The base HQL statement, without the "where" or "order by" clauses</param>
        public HqlQuery(string baseQuery)
            :this(baseQuery, 0, 100)
        {
        }

        /// <summary>
        /// Adds the specified list of <see cref="HqlCondition"/> objects to this query
        /// </summary>
        /// <param name="conditions"></param>
        public void AddConditions(IList<HqlCondition> conditions)
        {
            _conditions.AddRange(conditions);
        }

        /// <summary>
        /// Adds the specified list of <see cref="HqlSort"/> objects to this query
        /// </summary>
        /// <param name="sorts"></param>
        public void AddSorts(IList<HqlSort> sorts)
        {
            _sorts.AddRange(sorts);

            // keep the sorts in the correct order
            _sorts.Sort();
        }

        /// <summary>
        /// Returns the HQL text of this query.
        /// </summary>
        public string Hql
        {
            get
            {
                // build the where clause
                StringBuilder where = new StringBuilder();
                foreach (HqlCondition c in _conditions)
                {
                    if (where.Length != 0)
                        where.Append(" and ");

                    where.Append(c.Hql);
                }

                // build the order by clause
                StringBuilder orderBy = new StringBuilder();
                foreach (HqlSort s in _sorts)
                {
                    if (orderBy.Length != 0)
                        orderBy.Append(", ");

                    orderBy.Append(s.Hql);
                }

                // append where and order by to base query
                StringBuilder hql = new StringBuilder();
                hql.Append(_baseQuery);
                if (where.Length > 0)
                {
                    hql.Append(" where ");
                    hql.Append(where.ToString());
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
            int i = 0;
            foreach (HqlCondition c in _conditions)
            {
                foreach (object val in c.Parameters)
                {
                    if (val is Enum)
                    {
                        // convert to string, since hibernate doesn't know what to do with enums
                        q.SetParameter(i++, val.ToString());
                    }
                    else
                    {
                        q.SetParameter(i++, val);
                    }

                }
            }

            q.SetFirstResult(_firstRow);
            q.SetMaxResults(_maxRows);

            return q;
        }
    }
}
