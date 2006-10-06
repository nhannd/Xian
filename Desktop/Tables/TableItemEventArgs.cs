using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Event args used when a table item changes
    /// </summary>
    public class TableItemEventArgs : EventArgs
    {
        private int _itemIndex;
        private TableItemChangeType _changeType;

        internal TableItemEventArgs(TableItemChangeType changeType, int itemIndex)
        {
            _changeType = changeType;
            _itemIndex = itemIndex;
        }

        /// <summary>
        /// The type of change that occured
        /// </summary>
        public TableItemChangeType ChangeType
        {
            get { return _changeType; }
        }

        /// <summary>
        /// The index of the item that changed
        /// </summary>
        public int ItemIndex
        {
            get { return _itemIndex; }
        }
    }
}
