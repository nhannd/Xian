using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Presentation
{

    public class TableData<TItem> : ITableData
    {
        class TableRow<T> : ITableRow
        {
            private T _item;
            private ITableData _table;

            internal TableRow(ITableData table, T item)
            {
                _table = table;
                _item = item;
            }

            #region ITableRow Members

            public object Item
            {
                get { return _item; }
            }

            public object GetValue(int column)
            {
                return ((TableColumn<T>)_table.Columns[column]).GetValue(_item);
            }

            #endregion
        }


        private List<ITableColumn> _columns;
        private List<ITableRow> _rows;

        public TableData()
            :this(new TableColumn<TItem>[] {})
        {
        }

        public TableData(TableColumn<TItem>[] columns)
        {
            _rows = new List<ITableRow>();
            _columns = new List<ITableColumn>();
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

        public void Fill(IList<TItem> data)
        {
            _rows = new List<ITableRow>();
            foreach (TItem item in data)
            {
                _rows.Add(new TableRow<TItem>(this, item));
            }
        }

        #region ITableDataModel Members

        public IList<ITableRow> Rows
        {
            get { return _rows; }
        }

        public IList<ITableColumn> Columns
        {
            get { return _columns; }
        }

        #endregion
    }
}
