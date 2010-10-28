#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Defines the interface to the collection of items in a <see cref="ITable"/>.
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
        /// <param name="index">The index of the item to get.</param>
        /// <returns>The item in the collection at the specified index.</returns>
        object this[int index] { get; }
    }
}
