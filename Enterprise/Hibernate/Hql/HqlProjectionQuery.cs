#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    public class HqlProjectionQuery : HqlQuery
    {
        private readonly List<HqlSelect> _selects;
        private List<HqlFrom> _froms;
        private bool _selectDistinct;

		/// <summary>
		/// Constructor
		/// </summary>
		public HqlProjectionQuery()
			: this(new HqlFrom[] { }, new HqlSelect[] { }, new HqlCondition[] { }, new HqlSort[] { }, null)
		{
		}

		/// <summary>
        /// Constructor
        /// </summary>
        /// <param name="from"></param>
        public HqlProjectionQuery(HqlFrom from)
            : this(new HqlFrom[]{ from }, new HqlSelect[] { }, new HqlCondition[] { }, new HqlSort[] { }, null)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="from"></param>
        /// <param name="selects"></param>
        public HqlProjectionQuery(HqlFrom from, IEnumerable<HqlSelect> selects)
            : this(new HqlFrom[] { from }, selects, new HqlCondition[] { }, new HqlSort[] { }, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="froms"></param>
        /// <param name="selectors"></param>
        /// <param name="conditions"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        public HqlProjectionQuery(IEnumerable<HqlFrom> froms, IEnumerable<HqlSelect> selectors, IEnumerable<HqlCondition> conditions,
            IEnumerable<HqlSort> sorts, SearchResultPage page)
            : base("", conditions, sorts, page)
        {
            _froms = new List<HqlFrom>(froms);
            _selects = new List<HqlSelect>(selectors);
        }

        public List<HqlFrom> Froms
        {
            get { return _froms; }
        }

        public List<HqlSelect> Selects
        {
            get { return _selects; }
        }

        public bool SelectDistinct
        {
            get { return _selectDistinct; }
            set { _selectDistinct = true; }
        }

        protected override string BaseQueryHql
        {
            get
            {
                // build the select clause
                StringBuilder select = new StringBuilder();
                foreach (HqlSelect s in _selects)
                {
                    if (select.Length != 0)
                        select.Append(", ");

                    select.Append(s.Hql);
                }

                // build the from clause
                StringBuilder from = new StringBuilder();
                foreach (HqlFrom f in _froms)
                {
                    if (from.Length != 0)
                        from.AppendLine(", ");

                    from.Append(f.Hql);
                }

                // construct the base Hql by combining the select, from, and joins
                StringBuilder baseHql = new StringBuilder();
                if (select.Length > 0)
                {
                    baseHql.Append("select ");
                    if (_selectDistinct)
                        baseHql.Append("distinct ");
                    baseHql.Append(select.ToString());
                }

                baseHql.Append(" from ");
                baseHql.Append(from.ToString());

                return baseHql.ToString();
            }
        }
    }
}
