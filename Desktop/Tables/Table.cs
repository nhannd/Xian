using System;
using System.Collections;
using System.Collections.Generic;
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

        /// <summary>
        /// Constructor
        /// </summary>
        public Table()
        {
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
