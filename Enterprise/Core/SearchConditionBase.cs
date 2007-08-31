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
    /// Type-independent base class for the <see cref="SearchCondition{T}"/> and <see cref="SubSelect{T}"/> classes.
    /// </summary>
    public class SearchConditionBase : SearchCriteria
    {
        private object[] _values;
        private SearchConditionTest _test;
        private int _sortPosition;
        private bool _sortDirection;

        public SearchConditionBase()
            : this(null)
        {
        }

        public SearchConditionBase(string name)
            :base(name)
        {
            _test = SearchConditionTest.None;
            _sortPosition = -1;     // do not sort on this field
            _sortDirection = true;    // default sort direction to ascending
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
