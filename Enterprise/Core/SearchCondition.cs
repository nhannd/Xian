#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Provides a basic implementation of <see cref="ISearchCondition"/>.  See <see cref="SearchCriteria"/> for 
    /// usage of this class.
    /// </summary>
    /// <typeparam name="T">The type of the condition variable</typeparam>
    public class SearchCondition<T> : SearchConditionBase, ISearchCondition<T>, ISearchCondition
    {
        public SearchCondition()
        {
        }

        public SearchCondition(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected internal SearchCondition(SearchCondition<T> other)
            :base(other)
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

        public void In(IEnumerable<T> values)
        {
            // copy to an array of object
            object[] vals = CollectionUtils.Map<T, object>(values, delegate(T val) { return val; }).ToArray();

            SetCondition(SearchConditionTest.In, vals);
        }

		public void NotIn(IEnumerable<T> values)
		{
			// copy to an array of object
			object[] vals = CollectionUtils.Map<T, object>(values, delegate(T val) { return val; }).ToArray();

			SetCondition(SearchConditionTest.NotIn, vals);
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


        #region ISearchCondition Members

        void ISearchCondition.EqualTo(object val)
        {
            SetCondition(SearchConditionTest.Equal, val);
        }

        void ISearchCondition.NotEqualTo(object val)
        {
            SetCondition(SearchConditionTest.NotEqual, val);
        }

        void ISearchCondition.Like(object val)
        {
            SetCondition(SearchConditionTest.Like, val);
        }

        void ISearchCondition.NotLike(object val)
        {
            SetCondition(SearchConditionTest.NotLike, val);
        }

        void ISearchCondition.StartsWith(object val)
        {
            SetCondition(SearchConditionTest.Like, val + "%");
        }

        void ISearchCondition.Between(object lower, object upper)
        {
            SetCondition(SearchConditionTest.Between, lower, upper);
        }

        void ISearchCondition.In(System.Collections.IEnumerable values)
        {
            // copy to an array of object
            object[] vals = CollectionUtils.Map<T, object>(values, delegate(T val) { return val; }).ToArray();

            SetCondition(SearchConditionTest.In, vals);
        }

		void ISearchCondition.NotIn(System.Collections.IEnumerable values)
		{
			// copy to an array of object
			object[] vals = CollectionUtils.Map<T, object>(values, delegate(T val) { return val; }).ToArray();

			SetCondition(SearchConditionTest.NotIn, vals);
		}

        void ISearchCondition.LessThan(object val)
        {
            SetCondition(SearchConditionTest.LessThan, val);
        }

        void ISearchCondition.LessThanOrEqualTo(object val)
        {
            SetCondition(SearchConditionTest.LessThanOrEqual, val);
        }

        void ISearchCondition.MoreThan(object val)
        {
            SetCondition(SearchConditionTest.MoreThan, val);
        }

        void ISearchCondition.MoreThanOrEqualTo(object val)
        {
            SetCondition(SearchConditionTest.MoreThanOrEqual, val);
        }

        #endregion

        public override object Clone()
        {
            return new SearchCondition<T>(this);
        }
    }
}
