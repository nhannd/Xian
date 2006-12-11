using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// Defines the interface to the collection of items maintained by a tree.
    /// </summary>
    public interface ITreeItemCollection : System.Collections.IEnumerable
    {
        /// <summary>
        /// Gets the number of items in the collection
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the item at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object this[int index] { get; }
    }
}
