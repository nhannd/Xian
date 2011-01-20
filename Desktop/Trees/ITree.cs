#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// Defines the interface to a tree, which provides a presentation model for viewing hierarchical data.
    /// </summary>
    public interface ITree
    {
        /// <summary>
        /// Obtains the <see cref="ITreeItemBinding"/> that defines how items in this tree are mapped to the view.
        /// </summary>
        ITreeItemBinding Binding { get; }

        /// <summary>
        /// Obtains a reference to the collection of items in this tree.
        /// </summary>
        /// <remarks>
		/// Note that this collection contains only the immediate items.  Each 
		/// item may provide a sub-tree, which can be obtained via the
		/// <see cref="ITreeItemBinding.GetSubTree"/> method.
		/// </remarks>
        IItemCollection Items { get; }
    }
}
