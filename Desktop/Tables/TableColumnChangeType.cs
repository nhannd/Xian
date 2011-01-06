#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Enumerates the types of column changes.
    /// </summary>
    public enum TableColumnChangeType
    {
        /// <summary>
        /// A column was added to the table.
        /// </summary>
        ColumnAdded,

        /// <summary>
        /// An existing column was changed.
        /// </summary>
        ColumnChanged,

        /// <summary>
        /// An existing column was removed.
        /// </summary>
        ColumnRemoved
    }
}
