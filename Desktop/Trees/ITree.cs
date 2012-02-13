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
		/// <para>
		/// Note that this collection contains only the immediate items.  Each 
		/// item may provide a sub-tree, which can be obtained via the
		/// <see cref="ITreeItemBinding.GetSubTree"/> method.
		/// </para>
		/// <para>
		/// In general, it is advisable that the implementation of the root <see cref="ITree"/> should encapsulate
		/// a single ancestor root tree item, whose <see cref="IItemCollection"/> is returned in <see cref="Items"/>.
		/// Because only the root's children are returned in this interface, the tree view will still show them
		/// as "top-level" nodes, but they will still be related to each other through a common ancestor item.
		/// This is important, because a number of <see cref="ITree"/> features, such as check states, reordering,
		/// and view updates triggered from the model side depend on the existence of a parent node.
		/// </para>
		/// </remarks>
        IItemCollection Items { get; }
    }
}
