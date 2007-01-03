using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// A useful generic implementation of <see cref="ITree"/>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class Tree<TItem> : ITree
    {
        private ITreeItemBinding _binding;
        private ItemCollection<TItem> _items;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="binding"></param>
        public Tree(ITreeItemBinding binding)
            :this(binding, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="items">The set of items that are initially contained in this tree</param>
        public Tree(ITreeItemBinding binding, IEnumerable items)
        {
            _binding = binding;
            _items = new ItemCollection<TItem>();
            if (items != null)
            {
                _items.AddRange(items);
            }
        }

        /// <summary>
        /// Gets the item collection associated with this tree
        /// </summary>
        public ItemCollection<TItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets or sets the item binding associated with this tree
        /// </summary>
        public ITreeItemBinding Binding
        {
            get { return _binding; }
            set { _binding = value; }
        }

        #region ITree Members

        IItemCollection ITree.Items
        {
            get { return _items; }
        }

        #endregion
    }
}
