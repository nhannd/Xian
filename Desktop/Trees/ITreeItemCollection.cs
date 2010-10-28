#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// Defines the interface to the collection of items maintained by a tree.
    /// </summary>
    public interface ITreeItemCollection : System.Collections.IEnumerable
    {
        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
		/// Gets the item at the specified <paramref name="index"/>.
        /// </summary>
        object this[int index] { get; }
    }
}
