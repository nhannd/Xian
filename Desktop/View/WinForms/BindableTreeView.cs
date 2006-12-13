using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class BindableTreeView : UserControl
    {
        /// <summary>
        /// Manages a single level of a tree view, listening for changes to the underlying model and updating the tree view
        /// as required
        /// </summary>
        class TreeLevelManager
        {
            private ITree _tree;
            private TreeNodeCollection _nodeCollection;

            public TreeLevelManager(ITree tree, TreeNodeCollection nodeCollection)
            {
                _tree = tree;
                _tree.ItemsChanged += new EventHandler<TreeItemEventArgs>(TreeItemsChangedEventHandler);
                _nodeCollection = nodeCollection;

                BuildLevel();
            }

            private void BuildLevel()
            {
                _nodeCollection.Clear();
                foreach (object item in _tree.Items)
                {
                    _nodeCollection.Add(new SmartTreeNode(_tree, item));
                }
            }

            private void TreeItemsChangedEventHandler(object sender, TreeItemEventArgs e)
            {
                switch (e.ChangeType)
                {
                    case TreeItemChangeType.ItemAdded:
                        _nodeCollection.Add(new SmartTreeNode(_tree, _tree.Items[e.ItemIndex]));
                        break;
                    case TreeItemChangeType.ItemChanged:
                        _nodeCollection[e.ItemIndex] = new SmartTreeNode(_tree, _tree.Items[e.ItemIndex]);
                        break;
                    case TreeItemChangeType.ItemRemoved:
                        _nodeCollection.RemoveAt(e.ItemIndex);
                        break;
                    case TreeItemChangeType.Reset:
                        BuildLevel();
                        break;
                }
            }
        }

        /// <summary>
        /// Tree node that knows how to build its subtree on demand from the underlying model
        /// </summary>
        class SmartTreeNode : TreeNode
        {
            private TreeLevelManager _subtreeManager;
            private object _item;
            private ITree _parentTree;

            public SmartTreeNode(ITree parentTree, object item)
                : base(parentTree.Binding.GetNodeText(item))
            {
                _item = item;
                _parentTree = parentTree;
                this.ToolTipText = _parentTree.Binding.GetTooltipText(item);
                this.Nodes.Add(new TreeNode("dummy"));
            }

            public object DataBoundItem
            {
                get { return _item; }
            }

            public bool IsSubTreeBuilt
            {
                get { return _subtreeManager != null; } 
            }

            public void BuildSubTree()
            {
                if (!IsSubTreeBuilt)
                {
                    this.Nodes.Clear(); // remove the dummy node

                    ITree subTree = _parentTree.Binding.GetSubTree(_item);
                    if (subTree != null)
                    {
                        _subtreeManager = new TreeLevelManager(subTree, this.Nodes);
                    }
                }
            }
        }



        private ITree _root;
        private TreeLevelManager _rootLevelManager;
        private event EventHandler _selectionChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public BindableTreeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the model that this view looks at
        /// </summary>
        public ITree Tree
        {
            get { return _root; }
            set
            {
                _root = value;
                if (_root != null)
                {
                    _rootLevelManager = new TreeLevelManager(_root, _treeCtrl.Nodes);
                }
            }
        }

        /// <summary>
        /// Gets or sets the current selection
        /// </summary>
        public ISelection Selection
        {
            get
            {
                return GetSelectionHelper();
            }
            set
            {
                // if someone tries to assign null, just convert it to an empty selection - this makes everything easier
                ISelection newSelection = (value == null) ? new Selection() : value;

                // get the existing selection
                ISelection existingSelection = GetSelectionHelper();

                if (!existingSelection.Equals(newSelection))
                {
                    if (newSelection.Item == null)
                    {
                        _treeCtrl.SelectedNode = null;
                    }
                    else
                    {
                        _treeCtrl.SelectedNode = FindNodeRecursive(_treeCtrl.Nodes, delegate(SmartTreeNode node) { return node.DataBoundItem == newSelection.Item; });
                    }
                    EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Notifies that the selection has changed
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add { _selectionChanged += value; }
            remove { _selectionChanged -= value; }
        }

        /// <summary>
        /// Expands the entire tree
        /// </summary>
        public void ExpandAll()
        {
            _treeCtrl.ExpandAll();
        }

        #region Design time properties

        [DefaultValue(true)]
        public bool ShowToolbar
        {
            get { return _toolStrip.Visible; }
            set { _toolStrip.Visible = value; }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Obtains the current selection
        /// </summary>
        /// <returns></returns>
        private ISelection GetSelectionHelper()
        {
            SmartTreeNode selNode = (SmartTreeNode)_treeCtrl.SelectedNode;
            return selNode == null ? new Selection() : new Selection(selNode.DataBoundItem);
        }

        /// <summary>
        /// Searches the tree depth-first for a node matching the specified criteria
        /// </summary>
        /// <param name="nodeCollection"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private SmartTreeNode FindNodeRecursive(TreeNodeCollection nodeCollection, Predicate<SmartTreeNode> criteria)
        {
            foreach (SmartTreeNode node in nodeCollection)
            {
                if (criteria(node))
                {
                    return node;
                }
                else
                {
                    return FindNodeRecursive(node.Nodes, criteria);
                }
            }
            return null;
        }


        /// <summary>
        /// When the user is about to expand a node, need to build the level beneath it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _treeCtrl_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            SmartTreeNode expandingNode = (SmartTreeNode)e.Node;
            if (!expandingNode.IsSubTreeBuilt)
            {
                expandingNode.BuildSubTree();
            }
        }

        private void _treeCtrl_AfterSelect(object sender, TreeViewEventArgs e)
        {
            EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
        }

        #endregion
    }
}
