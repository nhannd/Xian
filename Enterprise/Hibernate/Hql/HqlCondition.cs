#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
    public class HqlCondition : HqlElement
    {
        /// <summary>
        /// Extracts a list of <see cref="HqlCondition"/> objects from the specified <see cref="SearchCriteria"/>
        /// </summary>
        /// <param name="qualifier">The HQL qualifier to prepend to the criteria variables</param>
        /// <param name="criteria">The search criteria object</param>
        /// <returns>A list of HQL conditions that are equivalent to the search criteria</returns>
        public static HqlCondition[] FromSearchCriteria(string qualifier, SearchCriteria criteria)
        {
            List<HqlCondition> hqlConditions = new List<HqlCondition>();
            if (criteria is SearchConditionBase)
            {
                SearchConditionBase sc = (SearchConditionBase)criteria;
                if (sc.Test != SearchConditionTest.None)
                {
                    hqlConditions.Add(new HqlCondition(GetHqlText(qualifier, sc), sc.Values));
                }
            }
            else
            {
                // recur on subCriteria
                foreach (SearchCriteria subCriteria in criteria.SubCriteria.Values)
                {
                    string subQualifier = string.Format("{0}.{1}", qualifier, subCriteria.GetKey());
                    hqlConditions.AddRange(FromSearchCriteria(subQualifier, subCriteria));
                }
            }

            return hqlConditions.ToArray();
        }

        private string _hql;
        private object[] _parameters;

        /// <summary>
        /// Constructs an <see cref="HqlCondition"/> from the specified HQL string and parameters.
        /// </summary>
        /// <param name="hql">The HQL string containing conditional parameter placeholders</param>
        /// <param name="parameters">Set of parameters to substitute</param>
        public HqlCondition(string hql, params object[] parameters)
        {
            _hql = hql;
            _parameters = parameters;
        }

        /// <summary>
        /// The HQL for this condition.
        /// </summary>
        public override string Hql
        {
            get { return _hql; }
        }

        /// <summary>
        /// The set of parameters to be substituted into this condition.
        /// </summary>
        public virtual object[] Parameters
        {
            get { return _parameters; }
        }

        private static string GetHqlText(string variable, SearchConditionBase sc)
        {
            string a;
            switch (sc.Test)
            {
                case SearchConditionTest.Equal:
                    a = "= ?";
                    break;
                case SearchConditionTest.NotEqual:
                    a = "<> ?";
                    break;
                case SearchConditionTest.Like:
                    a = "like ?";
                    break;
                case SearchConditionTest.NotLike:
                    a = "not like ?";
                    break;
                case SearchConditionTest.Between:
                    a = "between ? and ?";
                    break;
                case SearchConditionTest.In:
                    StringBuilder sb = new StringBuilder("in (?");  // assume at least one param
                    for (int i = 1; i < sc.Values.Length; i++) { sb.Append(", ?"); }
                    sb.Append(")");
                    a = sb.ToString();
                    break;
                case SearchConditionTest.LessThan:
                    a = "< ?";
                    break;
                case SearchConditionTest.LessThanOrEqual:
                    a = "<= ?";
                    break;
                case SearchConditionTest.MoreThan:
                    a = "> ?";
                    break;
                case SearchConditionTest.MoreThanOrEqual:
                    a = ">= ?";
                    break;
                case SearchConditionTest.NotNull:
                    a = "is not null";
                    break;
                case SearchConditionTest.Null:
                    a = "is null";
                    break;
                case SearchConditionTest.None:
                default:
                    throw new Exception();  // invalid
            }

            return string.Format("{0} {1}", variable, a);
        }

    }
}
