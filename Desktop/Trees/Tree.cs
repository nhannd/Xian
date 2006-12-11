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
        private TreeItemBinding<TItem> _binding;
        private TreeItemCollection<TItem> _items;

        private event EventHandler<TreeItemEventArgs> _itemsChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="binding"></param>
        public Tree(TreeItemBinding<TItem> binding)
            :this(binding, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="items">The set of items that are initially contained in this tree</param>
        public Tree(TreeItemBinding<TItem> binding, IEnumerable items)
        {
            _binding = binding;
            _items = new TreeItemCollection<TItem>(this, items);
        }

        /// <summary>
        /// Gets the item collection associated with this tree
        /// </summary>
        public TreeItemCollection<TItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets the item binding associated with this tree
        /// </summary>
        public TreeItemBinding<TItem> Binding
        {
            get { return _binding; }
        }

        #region ITree Members

        public event EventHandler<TreeItemEventArgs> ItemsChanged
        {
            add { _itemsChanged += value; }
            remove { _itemsChanged -= value; }
        }

        ITreeItemBinding ITree.Binding
        {
            get { return _binding; }
        }

        ITreeItemCollection ITree.Items
        {
            get { return _items; }
        }

        #endregion

        internal void NotifyItemsChanged(TreeItemChangeType changeType, int index)
        {
            EventsHelper.Fire(_itemsChanged, this, new TreeItemEventArgs(changeType, index));
        }
    }
}
