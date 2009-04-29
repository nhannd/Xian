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

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Set of possible test conditions.
    /// </summary>
    public enum SearchConditionTest
    {
        None,
        Equal,
        Like,
        NotLike,
        Between,
        In,
        LessThan,
        LessThanOrEqual,
        MoreThan,
        MoreThanOrEqual,
        NotEqual,
        Null,
        NotNull,
        Exists,
        NotExists,
    }

    /// <summary>
    /// Type-independent base class for the <see cref="SearchCondition{T}"/> and <see cref="RelatedEntityCondition{T}"/> classes.
    /// </summary>
    public abstract class SearchConditionBase : SearchCriteria
    {
        private object[] _values;
        private SearchConditionTest _test;
        private int _sortPosition;
        private bool _sortDirection;

        public SearchConditionBase()
            : this((string)null)
        {
        }

        public SearchConditionBase(string key)
            : base(key)
        {
            _test = SearchConditionTest.None;
            _sortPosition = -1;     // do not sort on this field
            _sortDirection = true;    // default sort direction to ascending
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected SearchConditionBase(SearchConditionBase other)
            :base(other)
        {
            _values = other._values;
            _test = other._test;
            _sortPosition = other._sortPosition;
            _sortDirection = other._sortDirection;
        }

        /// <summary>
        /// Returns the test that this condition uses.
        /// </summary>
        public SearchConditionTest Test
        {
            get { return _test; }
        }

        public override bool IsEmpty
        {
            get
            {
                return _test == SearchConditionTest.None && _sortPosition == -1;
            }
        }

        /// <summary>
        /// Returns the set of values that this condition uses to test against.  The number of values in the set
        /// depends on the type test being performed.  Most tests test agains a single value, but Between requires
        /// 2 values and In can work with any number of values.  Null and NotNull do not test against any values.
        /// </summary>
        public object[] Values
        {
            get { return _values; }
        }

        /// <summary>
        /// The relative priority of this sort constraint.
        /// </summary>
        public int SortPosition
        {
            get { return _sortPosition; }
        }

        /// <summary>
        /// The direction of this sort constraint.  True for ascending, false for descending.
        /// </summary>
        public bool SortDirection
        {
            get { return _sortDirection; }
        }

        public void SortAsc(int position)
        {
            _sortPosition = position;
            _sortDirection = true;
        }

        public void SortDesc(int position)
        {
            _sortPosition = position;
            _sortDirection = false;
        }

        protected void SetCondition(SearchConditionTest test, params object[] values)
        {
            // do not set a condition if any value is null
            foreach (object val in values)
            {
                if (IsNullValue(val))
                    throw new ArgumentNullException();
            }

            _test = test;
            _values = values;
        }

        protected bool IsNullValue(object val)
        {
            return val == null || val.ToString().Length == 0;
        }
    }
}
