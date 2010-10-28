#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Event args used when a table column changes.
    /// </summary>
    public class TableColumnEventArgs : EventArgs
    {
        private ITableColumn _column;
        private TableColumnChangeType _changeType;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal TableColumnEventArgs(TableColumnChangeType changeType, ITableColumn column)
        {
            _changeType = changeType;
            _column = column;
        }

        /// <summary>
        /// The type of change that occured.
        /// </summary>
        public TableColumnChangeType ChangeType
        {
            get { return _changeType; }
        }

        /// <summary>
        /// The column that changed.
        /// </summary>
        public ITableColumn Column
        {
            get { return _column; }
        }
    }
}
