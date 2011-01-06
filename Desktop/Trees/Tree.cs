#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// A useful generic implementation of <see cref="ITree"/>.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class Tree<TItem> : ITree
    {
        private ITreeItemBinding _binding;
        private ItemCollection<TItem> _items;

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="binding">The tree item binding.</param>
        public Tree(ITreeItemBinding binding)
            :this(binding, null)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="binding">The tree item binding.</param>
        /// <param name="items">The set of items that are initially contained in this tree.</param>
        public Tree(ITreeItemBinding binding, IEnumerable<TItem> items)
        {
            _binding = binding;
            _items = new ItemCollection<TItem>();
            if (items != null)
            {
                _items.AddRange(items);
            }
        }

        /// <summary>
		/// Gets the <see cref="IItemCollection{TItem}"/> associated with this tree.
        /// </summary>
        public ItemCollection<TItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets or sets the item binding associated with this tree.
        /// </summary>
        public ITreeItemBinding Binding
        {
            get { return _binding; }
            set { _binding = value; }
        }

        #region ITree Members

		/// <summary>
		/// Gets the <see cref="IItemCollection"/> associated with this tree.
		/// </summary>
		IItemCollection ITree.Items
        {
            get { return _items; }
        }

        #endregion
    }
}
