using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single or multiple selection
    /// </summary>
    public interface ISelection
    {
        /// <summary>
        /// Returns the set of items that are currently selected
        /// </summary>
        object[] Items { get; }

        /// <summary>
        /// Convenience method to obtain the currently selected item in a single-select scenario.
        /// If no rows are selected, the method returns null.  If more than one row is selected,
        /// it is undefined which item will be returned.
        /// </summary>
        object Item { get; }

        ISelection Union(ISelection other);
        ISelection Intersect(ISelection other);
        ISelection Subtract(ISelection other);
        bool Contains(object item);
    }
}
