using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Tables
{
    public class TableItemEventArgs : EventArgs
    {
        private int _itemIndex;
        private TableItemChangeType _changeType;

        public TableItemEventArgs(TableItemChangeType changeType, int itemIndex)
        {
            _changeType = changeType;
            _itemIndex = itemIndex;
        }

        public TableItemChangeType ChangeType
        {
            get { return _changeType; }
        }

        public int ItemIndex
        {
            get { return _itemIndex; }
        }
    }
}
