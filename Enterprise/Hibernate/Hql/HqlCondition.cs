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
                    string hqlVariable = string.IsNullOrEmpty(sc.GetKey()) ? qualifier : string.Format("{0}.{1}", qualifier, sc.GetKey());
                    hqlConditions.Add(new HqlCondition(GetHqlText(hqlVariable, sc), sc.Values));
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
        public HqlCondition(string hql, object[] parameters)
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
