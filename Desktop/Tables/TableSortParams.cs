using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Tables
{
    public class TableSortParams
    {
        private ITableColumn _column;
        private bool _ascending;

        public TableSortParams(ITableColumn column, bool ascending)
        {
            _column = column;
            _ascending = ascending;
        }

        public ITableColumn Column
        {
            get { return _column; }
            set { _column = value; }
        }

        public bool Ascending
        {
            get { return _ascending; }
            set { _ascending = value; }
        }
    }
}
