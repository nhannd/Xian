using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// Enumerates the types of item changes
    /// </summary>
    public enum TreeItemChangeType
    {
        /// <summary>
        /// An item was added to the tree
        /// </summary>
        ItemAdded,

        /// <summary>
        /// An item in the tree was changed
        /// </summary>
        ItemChanged,

        /// <summary>
        /// An item was removed from the tree
        /// </summary>
        ItemRemoved,

        /// <summary>
        /// All items in the tree may have changed
        /// </summary>
        Reset
    }
}
