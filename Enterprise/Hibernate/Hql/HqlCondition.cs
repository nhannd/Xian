using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    /// <summary>
    /// Provides support for building HQL queries dynamically from <see cref="SearchCriteria"/> objects.
    /// </summary>
    /// <seealso cref="HqlQuery"/>
    public class HqlCondition
    {
        /// <summary>
        /// Extracts a list of <see cref="HqlCondition"/> objects from the specified <see cref="SearchCriteria"/>
        /// </summary>
        /// <param name="alias">The HQL alias to prepend to the criteria variables</param>
        /// <param name="criteria">The search criteria object</param>
        /// <returns>A list of HQL conditions that are equivalent to the search criteria</returns>
        public static IList<HqlCondition> FromSearchCriteria(string alias, SearchCriteria criteria)
        {
            List<HqlCondition> hqlConditions = new List<HqlCondition>();
            foreach (SearchCriteria subCriteria in criteria.SubCriteria.Values)
            {
                if (subCriteria is SearchConditionBase)
                {
                    SearchConditionBase sc = (SearchConditionBase)subCriteria;
                    if (sc.Test != SearchConditionTest.None)
                    {
                        hqlConditions.Add(FromSearchCondition(alias, sc.GetKey(), sc));
                    }
                }
                else
                {
                    // recur on subCriteria
                    string subAlias = string.Format("{0}.{1}", alias, subCriteria.GetKey());
                    hqlConditions.AddRange(FromSearchCriteria(subAlias, subCriteria));
                }
            }
            return hqlConditions;
        }

        /// <summary>
        /// Translates a <see cref="SearchCondition"/> object to an <see cref="HqlCondition"/>
        /// </summary>
        /// <param name="alias">The HQL alias to prepend to the condition variable</param>
        /// <param name="fieldName">The HQL field name that serves as the condition variable</param>
        /// <param name="sc">the search condition</param>
        /// <returns>The HQL condition object equivalent to the specified search condition</returns>
        public static HqlCondition FromSearchCondition(string alias, string fieldName, SearchConditionBase sc)
        {
            return FromSearchCondition(string.Format("{0}.{1}", alias, fieldName), sc);
        }

        /// <summary>
        /// Translates a <see cref="SearchCondition"/> object to an <see cref="HqlCondition"/>
        /// </summary>
        /// <param name="variable">The HQL variable</param>
        /// <param name="sc">the search condition</param>
        /// <returns>The HQL condition object equivalent to the specified search condition</returns>
        public static HqlCondition FromSearchCondition(string variable, SearchConditionBase sc)
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
                    throw new NotImplementedException();
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

            return new HqlCondition(string.Format("{0} {1}", variable, a), sc.Values);
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
            _parameters = parameters;
            _hql = hql;
        }

        /// <summary>
        /// Constructs an <see cref="HqlCondition"/> from the specified HQL string and parameters.
        /// </summary>
        /// <param name="hql">The HQL string containing conditional parameter placeholders</param>
        /// <param name="parameters">Set of parameters to substitute</param>
        public HqlCondition(string hql, object parameter)
            : this(hql, new object[] { parameter })
        {
        }

        /// <summary>
        /// Constructs an <see cref="HqlCondition"/> from the specified HQL string.
        /// </summary>
        /// <param name="hql">The HQL string</param>
        public HqlCondition(string hql)
            : this(hql, new object[0])
        {
        }

        /// <summary>
        /// The HQL for this condition.
        /// </summary>
        public string Hql
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
    }
}
