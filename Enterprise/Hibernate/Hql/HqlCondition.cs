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
using System.Collections;

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
                    hqlConditions.Add(GetCondition(qualifier, sc.Test, sc.Values));
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

		public static HqlCondition EqualTo(string variable, object value)
		{
			return MakeCondition(variable, "= ?", value);
		}

		public static HqlCondition NotEqualTo(string variable, object value)
		{
			return MakeCondition(variable, "<> ?", value);
		}

		public static HqlCondition Like(string variable, string value)
		{
			return MakeCondition(variable, "like ?", value);
		}

		public static HqlCondition NotLike(string variable, string value)
		{
			return MakeCondition(variable, "not like ?", value);
		}

		public static HqlCondition Between(string variable, object lower, object upper)
		{
			return MakeCondition(variable, "between ? and ?", lower, upper);
		}

		public static HqlCondition In(string variable, params object[] values)
		{
			return InHelper(variable, values, false);
		}

		public static HqlCondition In(string variable, IEnumerable values)
		{
			return InHelper(variable, values, false);
		}

		public static HqlCondition NotIn(string variable, params object[] values)
		{
			return InHelper(variable, values, true);
		}

		public static HqlCondition NotIn(string variable, IEnumerable values)
		{
			return InHelper(variable, values, true);
		}

		private static HqlCondition InHelper(string variable, IEnumerable values, bool invert)
		{
			List<object> valueList = new List<object>();
			List<string> placeHolderList = new List<string>();
			foreach (object o in values)
			{
				valueList.Add(o);
				placeHolderList.Add("?");
			}

			StringBuilder sb = invert ? new StringBuilder("not in (") : new StringBuilder("in (");
			sb.Append(string.Join(",", placeHolderList.ToArray()));
			sb.Append(")");
			return MakeCondition(variable, sb.ToString(), valueList.ToArray());
		}


		public static HqlCondition LessThan(string variable, object value)
		{
			return MakeCondition(variable, "< ?", value);
		}

		public static HqlCondition LessThanOrEqual(string variable, object value)
		{
			return MakeCondition(variable, "<= ?", value);
		}

		public static HqlCondition MoreThan(string variable, object value)
		{
			return MakeCondition(variable, "> ?", value);
		}

		public static HqlCondition MoreThanOrEqual(string variable, object value)
		{
			return MakeCondition(variable, ">= ?", value);
		}

		public static HqlCondition IsNull(string variable)
		{
			return MakeCondition(variable, "is null");
		}

		public static HqlCondition IsNotNull(string variable)
		{
			return MakeCondition(variable, "is not null");
		}

		private static HqlCondition MakeCondition(string variable, string hql, params object[] values)
		{
			return new HqlCondition(string.Format("{0} {1}", variable, hql), values);
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

		internal static HqlCondition GetCondition(string variable, SearchConditionTest test, object[] values)
        {
			switch (test)
            {
                case SearchConditionTest.Equal:
            		return EqualTo(variable, values[0]);
                case SearchConditionTest.NotEqual:
            		return NotEqualTo(variable, values[0]);
                case SearchConditionTest.Like:
            		return Like(variable, (string)values[0]);
                case SearchConditionTest.NotLike:
            		return NotLike(variable, (string) values[0]);
                case SearchConditionTest.Between:
            		return Between(variable, values[0], values[1]);
                case SearchConditionTest.In:
            		return In(variable, values);
                case SearchConditionTest.LessThan:
            		return LessThan(variable, values[0]);
                case SearchConditionTest.LessThanOrEqual:
            		return LessThanOrEqual(variable, values[0]);
                case SearchConditionTest.MoreThan:
            		return MoreThan(variable, values[0]);
                case SearchConditionTest.MoreThanOrEqual:
            		return MoreThanOrEqual(variable, values[0]);
                case SearchConditionTest.NotNull:
            		return IsNotNull(variable);
                case SearchConditionTest.Null:
            		return IsNull(variable);
                case SearchConditionTest.None:
                default:
                    throw new Exception();  // invalid
            }
        }

    }
}
