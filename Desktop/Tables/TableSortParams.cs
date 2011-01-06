#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Sort parameters that can be applied to a table.
    /// </summary>
    public class TableSortParams
    {
        private ITableColumn _column;
        private bool _ascending;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="column">The column to sort by.</param>
        /// <param name="ascending">True if the items should be sorted in ascending orders.</param>
        public TableSortParams(ITableColumn column, bool ascending)
        {
            _column = column;
            _ascending = ascending;
        }

        /// <summary>
        /// Gets or sets the column to sort by.
        /// </summary>
        public ITableColumn Column
        {
            get { return _column; }
            set { _column = value; }
        }

        /// <summary>
        /// Gets or sets whether the items should be sorted in ascending or descending order.
        /// </summary>
        public bool Ascending
        {
            get { return _ascending; }
            set { _ascending = value; }
        }
    }
}
