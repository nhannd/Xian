using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Event args used when a table column changes
    /// </summary>
    public class TableColumnEventArgs : EventArgs
    {
        private ITableColumn _column;
        private TableColumnChangeType _changeType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="changeType"></param>
        /// <param name="column"></param>
        internal TableColumnEventArgs(TableColumnChangeType changeType, ITableColumn column)
        {
            _changeType = changeType;
            _column = column;
        }

        /// <summary>
        /// The type of change that occured
        /// </summary>
        public TableColumnChangeType ChangeType
        {
            get { return _changeType; }
        }

        /// <summary>
        /// The column that changed
        /// </summary>
        public ITableColumn Column
        {
            get { return _column; }
        }
    }
}
