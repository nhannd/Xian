using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// A useful generic implementation of <see cref="ITable"/>
    /// </summary>
    /// <typeparam name="TItem">The type of item that this table holds</typeparam>
    public class Table<TItem> : ITable
    {
        private TableColumnCollection<TItem> _columns;
        private TableItemCollection<TItem> _data;
        private float _baseColumnWidth;

        private TableSortParams _sortParams;

        private event EventHandler<TableItemEventArgs> _dataChanged;
        private event EventHandler<TableColumnEventArgs> _structureChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public Table()
        {
            _baseColumnWidth = 10;
            _columns = new TableColumnCollection<TItem>();
            _columns.ItemAdded += delegate(object sender, CollectionEventArgs<TableColumnBase<TItem>> args)
                {
                    args.Item.Table = this;
                    NotifyColumnChanged(TableColumnChangeType.ColumnAdded, args.Item);
                };
            _columns.ItemRemoved += delegate(object sender, CollectionEventArgs<TableColumnBase<TItem>> args)
                {
                    args.Item.Table = null;
                    NotifyColumnChanged(TableColumnChangeType.ColumnRemoved, args.Item);
                };

            _data = new TableItemCollection<TItem>(this);
        }

        /// <summary>
        /// Gets the collection of columns for the table.  Use this property to add <see cref="TableColumn"/> objects.
        /// </summary>
        public TableColumnCollection<TItem> Columns
        {
            get { return _columns; }
        }

        /// <summary>
        /// Gets the collection of items in the table.
        /// </summary>
        public TableItemCollection<TItem> Items
        {
            get { return _data; }
        }

        #region ITable members

        public event EventHandler<TableItemEventArgs> ItemsChanged
        {
            add { _dataChanged += value; }
            remove { _dataChanged -= value; }
        }

        public event EventHandler<TableColumnEventArgs> ColumnsChanged
        {
            add { _structureChanged += value; }
            remove { _structureChanged -= value; }
        }

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
                _data.Sort(new ComparerWrapper<TItem>(_sortParams.Column.GetComparer(_sortParams.Ascending)));

                // notify that the list has been sorted
                NotifyDataChanged(TableItemChangeType.Reset, -1);
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

        ITableItemCollection ITable.Items
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

        internal void NotifyDataChanged(TableItemChangeType changeType, int index)
        {
            EventsHelper.Fire(_dataChanged, this, new TableItemEventArgs(changeType, index));
        }

        internal void NotifyColumnChanged(TableColumnChangeType changeType, TableColumnBase<TItem> column)
        {
            EventsHelper.Fire(_structureChanged, this, new TableColumnEventArgs(changeType, column));
        }

        #endregion

    }
}
