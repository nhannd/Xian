using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Enterprise.Data.Hibernate.Hql
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
        /// <param name="alias">The HQL alias to prepend to the criteria variables</param>
        /// <param name="criteria">The search criteria object</param>
        /// <returns>A list of HQL conditions that are equivalent to the search criteria</returns>
        public static HqlCondition[] FromSearchCriteria(string alias, SearchCriteria criteria)
        {
            List<HqlCondition> hqlConditions = new List<HqlCondition>();
            foreach (SearchCriteria subCriteria in criteria.SubCriteria.Values)
            {
                if (subCriteria is SearchConditionBase)
                {
                    SearchConditionBase sc = (SearchConditionBase)subCriteria;
                    if (sc.Test != SearchConditionTest.None)
                    {
                        hqlConditions.Add(new HqlCondition(alias, sc.GetKey(), sc));
                    }
                }
                else
                {
                    // recur on subCriteria
                    string subAlias = string.Format("{0}.{1}", alias, subCriteria.GetKey());
                    hqlConditions.AddRange(FromSearchCriteria(subAlias, subCriteria));
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
        public HqlCondition(string alias, string fieldName, SearchConditionBase sc)
        {
            _hql = GetHqlText(alias, fieldName, sc);
            _parameters = sc.Values;
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
        public object[] Parameters
        {
            get { return _parameters; }
        }

        private static string GetHqlText(string alias, string field, SearchConditionBase sc)
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

            return string.Format("{0}.{1} {2}", alias, field, a);
        }

    }
}
