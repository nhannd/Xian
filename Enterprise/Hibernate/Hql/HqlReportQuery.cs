#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    public class HqlReportQuery : HqlQuery
    {
        private List<HqlSelect> _selects;
        private List<HqlJoin> _joins;
        private HqlFrom _from;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="from"></param>
        public HqlReportQuery(HqlFrom from)
            : this(from, new HqlSelect[] { }, new HqlJoin[] { }, new HqlCondition[] { }, new HqlSort[] { }, null)
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
        public HqlReportQuery(HqlFrom from, HqlSelect[] selectors, HqlJoin[] joins, HqlCondition[] conditions, HqlSort[] sorts, SearchResultPage page)
            : base("", conditions, sorts, page)
        {
            _from = from;
            _selects = new List<HqlSelect>(selectors);
            _joins = new List<HqlJoin>(joins);
        }

        public HqlFrom From
        {
            get { return _from; }
            set { _from = value; }
        }

        public List<HqlSelect> Selects
        {
            get { return _selects; }
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
                foreach (HqlSelect s in _selects)
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
