using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Trees
{
    public delegate string NodeTextProviderDelegate<T>(T item);
    public delegate ITree SubTreeProviderDelegate<T>(T item);

    /// <summary>
    /// A useful generic implementation of <see cref="ITreeItemBinding"/>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class TreeItemBinding<TItem> : ITreeItemBinding
    {
        private NodeTextProviderDelegate<TItem> _nodeTextProvider;
        private SubTreeProviderDelegate<TItem> _subTreeProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeTextProvider"></param>
        /// <param name="subTreeProvider"></param>
        public TreeItemBinding(NodeTextProviderDelegate<TItem> nodeTextProvider, SubTreeProviderDelegate<TItem> subTreeProvider)
        {
            _nodeTextProvider = nodeTextProvider;
            _subTreeProvider = subTreeProvider;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeTextProvider"></param>
        public TreeItemBinding(NodeTextProviderDelegate<TItem> nodeTextProvider)
            : this(nodeTextProvider, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TreeItemBinding()
            :this(null, null)
        {
        }

        /// <summary>
        /// Gets or sets the node text provider for this binding
        /// </summary>
        public NodeTextProviderDelegate<TItem> NodeTextProvider
        {
            get { return _nodeTextProvider; }
            set { _nodeTextProvider = value; }
        }

        /// <summary>
        /// Gets or sets the subtree provider for this binding
        /// </summary>
        public SubTreeProviderDelegate<TItem> SubTreeProvider
        {
            get { return _subTreeProvider; }
            set { _subTreeProvider = value; }
        }

        #region ITreeItemBinding Members

        public string GetNodeText(object item)
        {
            return _nodeTextProvider((TItem)item);
        }

        public ITree GetSubTree(object item)
        {
            return _subTreeProvider == null ? null : _subTreeProvider((TItem)item);
        }

        #endregion
    }
}
