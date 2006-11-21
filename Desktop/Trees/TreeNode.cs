using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Trees
{
    public delegate string NodeTextProviderDelegate<T>(T item);
    public delegate ITreeNode TreeNodeProviderDelegate<T>(T item);
    public delegate ITreeNodeCollection ChildNodeCollectionProviderDelegate<T>(T item);

    public class TreeNode<TItem> : ITreeNode
    {
        private TItem _item;
        private NodeTextProviderDelegate<TItem> _nodeTextProvider;
        private ChildNodeCollectionProviderDelegate<TItem> _childNodesProvider;

        public TreeNode(TItem item, NodeTextProviderDelegate<TItem> nodeTextProvider, ChildNodeCollectionProviderDelegate<TItem> childNodesProvider)
        {
            _item = item;
            _nodeTextProvider = nodeTextProvider;
            _childNodesProvider = childNodesProvider;
        }

        public string NodeText
        {
            get { return _nodeTextProvider(_item); }
        }

        public ITreeNodeCollection ChildNodes
        {
            get { return _childNodesProvider == null ? null : _childNodesProvider(_item); }
        }

    }
}
