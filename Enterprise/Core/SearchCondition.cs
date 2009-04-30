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
        protected SearchCondition(SearchCondition<T> other)
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
