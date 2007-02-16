using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.Trees;
using System.Drawing;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Tree node that knows how to build its subtree on demand from an <see cref="ITree"/> model.  This class
    /// is used internally and is not intended to be used directly by application code.
    /// </summary>
    internal class BindingTreeNode : TreeNode
    {
        private BindingTreeLevelManager _subtreeManager;
        private object _item;
        private ITree _parentTree;
        private bool _isSubTreeBuilt;

        public BindingTreeNode(ITree parentTree, object item)
            : base(parentTree.Binding.GetNodeText(item))
        {
            _item = item;
            _parentTree = parentTree;

            UpdateDisplay();
        }

        /// <summary>
        /// The item that this node represents
        /// </summary>
        public object DataBoundItem
        {
            get { return _item; }
            set
            {
                if (value != _item)
                {
                    _item = value;
                    UpdateDisplay();
                }
            }
        }

        /// <summary>
        /// Updates the displayable properties of this node, based on the underlying model
        /// </summary>
        public void UpdateDisplay()
        {
            if(this.TreeView != null)
                this.TreeView.BeginUpdate();

            // update all displayable attributes from the binding
            this.Text = _parentTree.Binding.GetNodeText(_item);
            this.ToolTipText = _parentTree.Binding.GetTooltipText(_item);

            if (this.TreeView != null)
            {
                IResourceResolver resolver = _parentTree.Binding.GetResourceResolver(_item);
                IconSet iconSet = _parentTree.Binding.GetIconSet(_item);
                ImageList.ImageCollection imageCollection = this.TreeView.ImageList.Images;
                if (iconSet == null)
                {
                    this.ImageIndex = -1;
                }
                else if (imageCollection.ContainsKey(iconSet.MediumIcon))
                {
                    this.ImageIndex = imageCollection.IndexOfKey(iconSet.MediumIcon);
                }
                else
                {
                    try
                    {
                        imageCollection.Add(iconSet.MediumIcon, IconFactory.CreateIcon(iconSet.MediumIcon, resolver));
                        this.ImageIndex = imageCollection.IndexOfKey(iconSet.MediumIcon);
                    }
                    catch (Exception e)
                    {
                        Platform.Log(e, LogLevel.Warn);
                        this.ImageIndex = -1;
                    }
                }

                this.SelectedImageIndex = this.ImageIndex;
            }

            // if the subtree was already built, we need to rebuild it because it may no longer be valid
            if (_isSubTreeBuilt)
            {
                RebuildSubTree();
            }
            else
            {
                // add a dummy child so that we get a "plus" sign next to the node
                if (_parentTree.Binding.CanHaveSubTree(_item) && this.Nodes.Count == 0)
                {
                    this.Nodes.Add(new TreeNode("dummy"));
                }
            }

            if (this.TreeView != null)
                this.TreeView.EndUpdate();
        }

        /// <summary>
        /// Returns true if the sub-tree of this node has been built
        /// </summary>
        public bool IsSubTreeBuilt
        {
            get { return _isSubTreeBuilt; }
        }

        /// <summary>
        /// Forces the sub-tree to be built
        /// </summary>
        public void BuildSubTree()
        {
            if (!_isSubTreeBuilt)
            {
                _isSubTreeBuilt = true;
                RebuildSubTree();
            }
        }

        /// <summary>
        /// Asks the item if it can accept the specifid drop
        /// </summary>
        /// <param name="dropData"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public DragDropKind CanAcceptDrop(object dropData, DragDropKind kind)
        {
            return _parentTree.Binding.CanAcceptDrop(_item, dropData, kind);
        }

        /// <summary>
        /// Asks the item to accept the specified drop
        /// </summary>
        /// <param name="dropData"></param>
        /// <param name="kind"></param>
        public DragDropKind AcceptDrop(object dropData, DragDropKind kind)
        {
            return _parentTree.Binding.AcceptDrop(_item, dropData, kind);
        }

        /// <summary>
        /// Rebuilds the sub-tree
        /// </summary>
        private void RebuildSubTree()
        {
            this.Nodes.Clear(); // remove any existing nodes

            ITree subTree = _parentTree.Binding.GetSubTree(_item);
            if (subTree != null)
            {
                _subtreeManager = new BindingTreeLevelManager(subTree, this.Nodes);
            }
        }
    }
}
