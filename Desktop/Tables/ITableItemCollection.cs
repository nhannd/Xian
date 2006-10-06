using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Defines the interface to the collection of items in a <see cref="ITable"/>
    /// </summary>
    public interface ITableItemCollection : System.Collections.IEnumerable
    {
        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the item in the collection at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get</param>
        /// <returns>The item in the collection at the specified index</returns>
        object this[int index] { get; }
    }
}
