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
using System.Reflection;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    /// <summary>
    /// Provides support for building HQL queries dynamically from <see cref="SearchCriteria"/> objects.
    /// </summary>
    /// <seealso cref="HqlQuery"/>
    public class HqlSort : HqlElement, IComparable<HqlSort>
    {
        /// <summary>
        /// Extracts a list of <see cref="HqlSort"/> objects from the specified <see cref="SearchCriteria"/>
        /// </summary>
        /// <param name="qualifier">The HQL qualifier to prepend to the sort variables</param>
        /// <param name="criteria">The search criteria object</param>
        /// <returns>A list of HQL sort object that are equivalent to the specified criteria</returns>
        public static HqlSort[] FromSearchCriteria(string qualifier, SearchCriteria criteria)
        {
            List<HqlSort> hqlSorts = new List<HqlSort>();
            if (criteria is SearchConditionBase)
            {
                SearchConditionBase sc = (SearchConditionBase)criteria;
                if (sc.SortPosition > -1)
                {
                    hqlSorts.Add(new HqlSort(qualifier, sc.SortDirection, sc.SortPosition));
                }
            }
            else
            {
                // recur on subCriteria
                foreach (SearchCriteria subCriteria in criteria.SubCriteria.Values)
                {
                    string subQualifier = string.Format("{0}.{1}", qualifier, subCriteria.GetKey());
                    hqlSorts.AddRange(FromSearchCriteria(subQualifier, subCriteria));
                }
            }

            return hqlSorts.ToArray();
        }

        private string _hql;
        private int _position;

        /// <summary>
        /// Constructs an <see cref="HqlSort"/> object.
        /// </summary>
        /// <param name="variable">The HQL variable on which to sort</param>
        /// <param name="ascending">Specifies whether the sort is ascending or descending</param>
        /// <param name="position">Specifies the relative priority of the sort condition</param>
        public HqlSort(string variable, bool ascending, int position)
        {
            _hql = string.Format("{0} {1}", variable, ascending ? "asc" : "desc");
            _position = position;
        }

        /// <summary>
        /// The HQL for this sort.
        /// </summary>
        public override string Hql
        {
            get { return _hql; }
        }

        /// <summary>
        /// The position of this sort in the order by clause.
        /// </summary>
        public int Position
        {
            get { return _position; }
        }

        #region IComparable<HqlSort> Members

        public int CompareTo(HqlSort other)
        {
            return this._position.CompareTo(other._position);
        }

        #endregion
    }
}
