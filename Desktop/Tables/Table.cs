using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.Desktop.Tables
{
    public class Table<TItem> : ITable
    {
        /// <summary>
        /// Provides a typed wrapper around an untyped <see cref="IComparer"/>
        /// </summary>
        class ComparerWrapper : Comparer<TItem>
        {
            private IComparer _inner;
            public ComparerWrapper(IComparer inner)
            {
                _inner = inner;
            }
            public override int Compare(TItem x, TItem y)
            {
                return _inner.Compare(x, y);
            }
        }


        private TableColumnCollection<TItem> _columns;
        private TableItemCollection<TItem> _data;
        private float _baseColumnWidth;

        private TableSortParams _sortParams;

        private event EventHandler<TableItemEventArgs> _dataChanged;
        private event EventHandler<TableColumnEventArgs> _structureChanged;

        public Table()
        {
            _baseColumnWidth = 100;
            _columns = new TableColumnCollection<TItem>();
            _columns.ItemAdded += delegate(object sender, CollectionEventArgs<TableColumnBase<TItem>> args)
                {
                    NotifyStructureChanged(TableColumnChangeType.ColumnAdded, args.Item);
                };
            _columns.ItemRemoved += delegate(object sender, CollectionEventArgs<TableColumnBase<TItem>> args)
                {
                    NotifyStructureChanged(TableColumnChangeType.ColumnRemoved, args.Item);
                };

            _data = new TableItemCollection<TItem>(this);
        }

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

        /// <summary>
        /// Accesses the list of columns that describe this table data.  Use this property to add
        /// <see cref="TableColumn"/> objects.
        /// </summary>
        public TableColumnCollection<TItem> Columns
        {
            get { return _columns; }
        }

        ITableColumnCollection ITable.Columns
        {
            get { return _columns; }
        }

        public void Sort()
        {
            if (_sortParams != null)
            {
                _data.Sort(new ComparerWrapper(_sortParams.Column.GetComparer(_sortParams.Ascending)));

                // notify that the list has been sorted
                NotifyDataChanged(TableItemChangeType.Reset, -1);
            }
        }

        public void Sort(TableSortParams sortParams)
        {
            _sortParams = sortParams;
            Sort();
        }

        public float BaseColumnWidth
        {
            get { return _baseColumnWidth; }
            set { _baseColumnWidth = value; }
        }

        public TableItemCollection<TItem> Items
        {
            get { return _data; }
        }

        ITableItemCollection ITable.Items
        {
            get { return _data; }
        }

        #region Private methods

        private int GetColumnWidthPercent(ITableColumn column)
        {
            float sum = 0;
            foreach (ITableColumn c in _columns)
            {
                sum += c.WidthFactor;
            }
            return (int)(100 * column.WidthFactor / sum);
        }

        internal void NotifyDataChanged(TableItemChangeType changeType, int index)
        {
            EventsHelper.Fire(_dataChanged, this, new TableItemEventArgs(changeType, index));
        }

        internal void NotifyStructureChanged(TableColumnChangeType changeType, ITableColumn column)
        {
            EventsHelper.Fire(_structureChanged, this, new TableColumnEventArgs(changeType, column));
        }

        #endregion

    }
}
