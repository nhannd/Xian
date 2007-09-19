using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Provides a basic implementation of <see cref="IRelatedEntityCondition{T}"/>.  See <see cref="SearchCriteria"/> for 
    /// usage of this class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelatedEntityCondition<T> : SearchConditionBase, IRelatedEntityCondition<T>
        where T: SearchCriteria
    {
        public RelatedEntityCondition()
        {
        }

        public RelatedEntityCondition(string name)
            : base(name)
        {
        }

        #region IRelatedEntityCondition<T> Members

        public void Exists(T val)
        {
            SetCondition(SearchConditionTest.Exists, val);
        }

        public void NotExists(T val)
        {
            SetCondition(SearchConditionTest.NotExists, val);
        }

        #endregion
    }
}
