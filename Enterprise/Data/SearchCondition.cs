using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Provides a basic implementation of <see cref="ISearchCondition"/>.  See <see cref="SearchCriteria"/> for 
    /// usage of this class.
    /// </summary>
    /// <typeparam name="T">The type of the condition variable</typeparam>
    public class SearchCondition<T> : SearchConditionBase, ISearchCondition<T>
    {
        public SearchCondition(string name)
            : base(name)
        {
        }

        public void EqualTo(T val)
        {
            SetCondition(SearchConditionTest.Equal, val);
        }

        public void Like(T val)
        {
            SetCondition(SearchConditionTest.Like, val);
        }

        public void NotLike(T val)
        {
            SetCondition(SearchConditionTest.NotLike, val);
        }

        public void StartsWith(T val)
        {
            SetCondition(SearchConditionTest.Like, val + "%");
        }

        public void Between(T lower, T upper)
        {
            SetCondition(SearchConditionTest.Between, lower, upper);
        }

        public void In(T[] values)
        {
            // copy to an array of object
            object[] vals = new object[values.Length];
            values.CopyTo(vals, 0);

            SetCondition(SearchConditionTest.In, vals);
        }

        public void LessThan(T val)
        {
            SetCondition(SearchConditionTest.LessThan, val);
        }

        public void LessThanOrEqualTo(T val)
        {
            SetCondition(SearchConditionTest.LessThanOrEqual, val);
        }

        public void MoreThan(T val)
        {
            SetCondition(SearchConditionTest.MoreThan, val);
        }

        public void MoreThanOrEqualTo(T val)
        {
            SetCondition(SearchConditionTest.MoreThanOrEqual, val);
        }

        public void NotEqualTo(T val)
        {
            SetCondition(SearchConditionTest.NotEqual, val);
        }

        public void IsNull()
        {
            SetCondition(SearchConditionTest.Null);
        }

        public void IsNotNull()
        {
            SetCondition(SearchConditionTest.NotNull);
        }

    }
}
