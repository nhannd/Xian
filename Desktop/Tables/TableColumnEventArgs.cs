using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Tables
{
    public class TableColumnEventArgs : EventArgs
    {
        private ITableColumn _column;
        private TableColumnChangeType _changeType;

        public TableColumnEventArgs(TableColumnChangeType changeType, ITableColumn column)
        {
            _changeType = changeType;
            _column = column;
        }

        public TableColumnChangeType ChangeType
        {
            get { return _changeType; }
        }

        public ITableColumn Column
        {
            get { return _column; }
        }
    }
}
