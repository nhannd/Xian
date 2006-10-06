using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Enumerates the types of column changes
    /// </summary>
    public enum TableColumnChangeType
    {
        /// <summary>
        /// A column was added to the table
        /// </summary>
        ColumnAdded,

        /// <summary>
        /// An existing column was changed
        /// </summary>
        ColumnChanged,

        /// <summary>
        /// An existing column was removed
        /// </summary>
        ColumnRemoved
    }
}
