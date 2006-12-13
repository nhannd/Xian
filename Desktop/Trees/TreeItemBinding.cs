using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Trees
{
    public delegate string TextProviderDelegate<T>(T item);
    public delegate ITree TreeProviderDelegate<T>(T item);

    /// <summary>
    /// A useful generic implementation of <see cref="ITreeItemBinding"/>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class TreeItemBinding<TItem> : TreeItemBindingBase
    {
        private TextProviderDelegate<TItem> _nodeTextProvider;
        private TextProviderDelegate<TItem> _tooltipTextProvider;
        private TreeProviderDelegate<TItem> _subTreeProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeTextProvider"></param>
        /// <param name="subTreeProvider"></param>
        public TreeItemBinding(TextProviderDelegate<TItem> nodeTextProvider, TreeProviderDelegate<TItem> subTreeProvider)
        {
            _nodeTextProvider = nodeTextProvider;
            _subTreeProvider = subTreeProvider;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeTextProvider"></param>
        public TreeItemBinding(TextProviderDelegate<TItem> nodeTextProvider)
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
        public TextProviderDelegate<TItem> NodeTextProvider
        {
            get { return _nodeTextProvider; }
            set { _nodeTextProvider = value; }
        }

        /// <summary>
        /// Gets or sets the tooltip text provider for this binding
        /// </summary>
        public TextProviderDelegate<TItem> TooltipTextProvider
        {
            get { return _tooltipTextProvider; }
            set { _tooltipTextProvider = value; }
        }


        /// <summary>
        /// Gets or sets the subtree provider for this binding
        /// </summary>
        public TreeProviderDelegate<TItem> SubTreeProvider
        {
            get { return _subTreeProvider; }
            set { _subTreeProvider = value; }
        }

        public override string GetNodeText(object item)
        {
            return _nodeTextProvider((TItem)item);
        }

        public override ITree GetSubTree(object item)
        {
            return _subTreeProvider == null ? null : _subTreeProvider((TItem)item);
        }

        public override string GetTooltipText(object item)
        {
            return _tooltipTextProvider == null ? null : _tooltipTextProvider((TItem)item);
        }
    }
}
