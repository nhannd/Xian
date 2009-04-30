#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
    public class HqlProjectionQuery : HqlQuery
    {
        private readonly List<HqlSelect> _selects;
        private List<HqlFrom> _froms;
        private bool _selectDistinct;

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
