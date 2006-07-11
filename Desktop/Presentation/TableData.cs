using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Presentation
{

    public class TableData<TItem> : ITableData
    {
        public class TableRow<T> : ITableRow
        {
            private T _item;
            private TableData<T> _table;

            internal TableRow(TableData<T> table, T item)
            {
                _table = table;
                _item = item;
            }

            public T Item
            {
                get { return _item; }
                set { _item = value; }
            }

            #region ITableRow Members

            object ITableRow.Item
            {
                get { return _item; }
            }

            object ITableRow.GetValue(int column)
            {
                return _table.Columns[column].GetValue(_item);
            }

            #endregion
        }


        private ArrayList _columns;
        private ArrayList _rows;

        public TableData()
            :this(new TableColumn<TItem>[] {})
        {
        }

        public TableData(TableColumn<TItem>[] columns)
        {
            _rows = new ArrayList();
            _columns = new ArrayList();
            _columns.AddRange(columns);
        }
        
        public void AddColumn(TableColumn<TItem> column)
        {
            _columns.Add(column);
        }

        public void AddColumns(IList<TableColumn<TItem>> columns)
        {
            foreach (TableColumn<TItem> col in columns)
            {
                AddColumn(col);
            }
        }

        public void Fill(ICollection<TItem> data)
        {
            _rows.Clear();
            foreach (TItem item in data)
            {
                _rows.Add(new TableRow<TItem>(this, item));
            }
        }

        public TableRow<TItem>[] Rows
        {
            get { return (TableRow<TItem>[])_rows.ToArray(typeof(TableRow<TItem>)); }
        }

        public TableColumn<TItem>[] Columns
        {
            get { return (TableColumn<TItem>[])_columns.ToArray(typeof(TableColumn<TItem>)); }
        }

        #region ITableData Members

        ITableRow[] ITableData.Rows
        {
            get { return (ITableRow[])_rows.ToArray(typeof(ITableRow)); }
        }

        ITableColumn[] ITableData.Columns
        {
            get { return (ITableColumn[])_columns.ToArray(typeof(ITableColumn)); }
        }

        #endregion
    }
}
