using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Enumerates the types of item changes
    /// </summary>
    public enum TableItemChangeType
    {
        /// <summary>
        /// An item was added to the table
        /// </summary>
        ItemAdded,

        /// <summary>
        /// An item in the table was changed
        /// </summary>
        ItemChanged,

        /// <summary>
        /// An item was removed from the table
        /// </summary>
        ItemRemoved,

        /// <summary>
        /// All items in the table may have changed
        /// </summary>
        Reset
    }
}
