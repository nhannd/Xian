#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
        #region Private Members
        private readonly string _baseTableColumn;
        private readonly string _relatedTableColumn;
        #endregion

        #region Public Properties
        public string BaseTableColumn
        {
            get { return _baseTableColumn; }
        }
        public string RelatedTableColumn
        {
            get { return _relatedTableColumn; }
        }
        #endregion

        #region Constructors
        public RelatedEntityCondition()
        {
        }

        public RelatedEntityCondition(string name, string baseTableColumn, string relatedTableColumn)
            : base(name)
        {
            _baseTableColumn = baseTableColumn;
            _relatedTableColumn = relatedTableColumn;
        }

        protected RelatedEntityCondition(RelatedEntityCondition<T> other)
            :base(other)
        {
            _baseTableColumn = other._baseTableColumn;
            _relatedTableColumn = other._relatedTableColumn;
        }
        #endregion

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

        public override object Clone()
        {
            return new RelatedEntityCondition<T>(this);
        }
    }
}
