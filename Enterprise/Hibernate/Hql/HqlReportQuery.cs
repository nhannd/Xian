using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    public class HqlReportQuery : HqlQuery
    {
        private List<HqlSelector> _selectors;
        private List<HqlJoin> _joins;
        private HqlFrom _from;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="from"></param>
        public HqlReportQuery(HqlFrom from)
            :this(from, new HqlSelector[]{}, new HqlJoin[]{}, new HqlCondition[]{}, new HqlSort[]{}, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="from"></param>
        /// <param name="selectors"></param>
        /// <param name="joins"></param>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        public HqlReportQuery(HqlFrom from, HqlSelector[] selectors, HqlJoin[] joins, HqlCondition[] conditions, HqlSort[] sorts, SearchResultPage page)
            :base("", conditions, sorts, page)
        {
            _from = from;
            _selectors = new List<HqlSelector>(selectors);
            _joins = new List<HqlJoin>(joins);
        }

        public HqlFrom From
        {
            get { return _from; }
            set { _from = value; }
        }

        public List<HqlSelector> Selectors
        {
            get { return _selectors; }
        }

        public List<HqlJoin> Joins
        {
            get { return _joins; }
        }

        public override string Hql
        {
            get
            {
                // build the select clause
                StringBuilder select = new StringBuilder();
                foreach (HqlSelector s in _selectors)
                {
                    if (select.Length != 0)
                        select.Append(", ");

                    select.Append(s.Hql);
                }

                // build the joins clause
                StringBuilder joins = new StringBuilder();
                foreach (HqlJoin j in _joins)
                {
                    joins.Append(" ");
                    joins.Append(j.Hql);
                }

                // construct the base Hql as by combining the select, from, and joins
                StringBuilder baseHql = new StringBuilder();
                if (select.Length > 0)
                {
                    baseHql.Append("select ");
                    baseHql.Append(select.ToString());
                }

                baseHql.Append(" from ");
                baseHql.Append(_from.Hql);
                baseHql.Append(joins.ToString());

                // set this as the base query on the base class, and then delegate to the base class for the rest of the query
                this.BaseQuery = baseHql.ToString();
                return base.Hql;
            }
        }
    }
}
