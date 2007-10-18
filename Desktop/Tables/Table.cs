#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// A useful generic implementation of <see cref="ITable"/>
    /// </summary>
    /// <typeparam name="TItem">The type of item that this table holds</typeparam>
    public class Table<TItem> : ITable, ITable<TItem>
    {
        private TableColumnCollection<TItem> _columns;
        private ItemCollection<TItem> _data;
        private float _baseColumnWidth;

        private TableSortParams _sortParams;

        private bool _isFiltered = false;
        private TableFilterParams _filterParams;
        private FilteredItemCollection<TItem> _filteredData;

        private readonly uint _cellRowCount;
        private ColorSelector _backgroundColorSelector;
        private ColorSelector _outlineColorSelector;

        /// <summary>
        /// Constructor
        /// </summary>
        public Table()
            : this(1)
        {
        }

        ///<summary>
        /// Constructor a table column with cellRowCount in each row
        ///</summary>
        ///<param name="cellRowCount"></param>
        public Table(uint cellRowCount)
        {
            Platform.CheckArgumentRange((int)cellRowCount, 1, int.MaxValue, "cellRowCount");
            _cellRowCount = cellRowCount;

            _baseColumnWidth = 10;
            _columns = new TableColumnCollection<TItem>();
            _columns.ItemsChanged += delegate(object sender, ItemChangedEventArgs args)
            {
                switch (args.ChangeType)
                {
                    case ItemChangeType.ItemAdded:
                        _columns[args.ItemIndex].Table = this;
                        break;
                    case ItemChangeType.ItemRemoved:
                        _columns[args.ItemIndex].Table = null;
                        break;
                }
            };

            _data = new ItemCollection<TItem>();
            _filteredData = null;
        }

        #region ITable members

        public Type ItemType
        {
            get { return typeof(TItem); }
        }

        public TableSortParams SortParams
        {
            get { return _sortParams; }
        }

        ITableColumnCollection ITable.Columns
        {
            get { return _columns; }
        }

        public void Sort()
        {
            if (_sortParams != null)
            {
                _data.Sort(new TypeSafeComparerWrapper<TItem>(_sortParams.Column.GetComparer(_sortParams.Ascending)));
            }
        }

        public void Sort(TableSortParams sortParams)
        {
            _sortParams = sortParams;
            Sort();
        }

        public float BaseColumnWidthChars
        {
            get { return _baseColumnWidth; }
            set { _baseColumnWidth = value; }
        }

        IItemCollection ITable.Items
        {
            get { return _isFiltered ? _filteredData : _data; }
        }

        public bool IsFiltered
        {
            get { return _isFiltered; }
        }

        public void Filter(TableFilterParams filterParams)
        {
            _filterParams = filterParams;
            Filter();
        }

        public void Filter()
        {
            if(_filterParams != null && _filterParams.Value != null)
            {
                _isFiltered = true;
                
                _filteredData = new FilteredItemCollection<TItem>(this, _filterParams);
            }
        }

        public void RemoveFilter()
        {
            _isFiltered = false;
        }

        public uint CellRowCount
        {
            get { return _cellRowCount; }
        }

        public ColorSelector BackgroundColorSelector
        {
            get { return _backgroundColorSelector; }
            set { _backgroundColorSelector = value; }
        }

        public ColorSelector OutlineColorSelector
        {
            get { return _outlineColorSelector; }
            set { _outlineColorSelector = value; }
        }

        #endregion

        #region ITable<TItem> members

        /// <summary>
        /// Gets the collection of columns for the table.  Use this property to add <see cref="ITableColumn"/> objects.
        /// </summary>
        public TableColumnCollection<TItem> Columns
        {
            get { return _columns; }
        }

        /// <summary>
        /// Gets the collection of items in the table.
        /// </summary>
        /// <remarks>The returned collection is never filtered.</remarks>
        public ItemCollection<TItem> Items
        {
            get { return _data; }
        }

        #endregion

        #region Internal methods

        internal int GetColumnWidthPercent(TableColumnBase<TItem> column)
        {
            float sum = 0;
            foreach (TableColumnBase<TItem> c in _columns)
            {
                sum += c.WidthFactor;
            }
            return (int)(100 * column.WidthFactor / sum);
        }

        internal void NotifyColumnChanged(TableColumnBase<TItem> column)
        {
            _columns.NotifyItemUpdated(column);
        }

        #endregion
    }
}
