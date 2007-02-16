using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Manages a single level of a tree view, listening for changes to the underlying model and updating the tree view
    /// as required.  This class is used internally and is not intended to be used directly by application code.
    /// </summary>
    internal class BindingTreeLevelManager
    {
        private ITree _tree;
        private TreeNodeCollection _nodeCollection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="nodeCollection"></param>
        public BindingTreeLevelManager(ITree tree, TreeNodeCollection nodeCollection)
        {
            _tree = tree;
            _tree.Items.ItemsChanged += TreeItemsChangedEventHandler;
            _nodeCollection = nodeCollection;

            BuildLevel();
        }

        /// <summary>
        /// Handles changes to the tree's items collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeItemsChangedEventHandler(object sender, ItemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case ItemChangeType.ItemAdded:
                    AddNode(_tree.Items[e.ItemIndex]);
                    break;
                case ItemChangeType.ItemChanged:
                    UpdateNode(e.ItemIndex, _tree.Items[e.ItemIndex]);
                    break;
                case ItemChangeType.ItemRemoved:
                    RemoveNode(e.ItemIndex);
                    break;
                case ItemChangeType.Reset:
                    BuildLevel();
                    break;
            }
        }

        /// <summary>
        /// Adds a node for the specified item
        /// </summary>
        /// <param name="item"></param>
        private void AddNode(object item)
        {
            _nodeCollection.Add(new BindingTreeNode(_tree, item));
        }

        /// <summary>
        /// Updates the node at the specified index, with the specified item
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        private void UpdateNode(int index, object item)
        {
            BindingTreeNode node = (BindingTreeNode)_nodeCollection[index];
            node.DataBoundItem = item;
            node.UpdateDisplay();   // force update, even if it is the same item, because its properties may have changed
        }

        /// <summary>
        /// Removes the node at the specified index
        /// </summary>
        /// <param name="index"></param>
        private void RemoveNode(int index)
        {
            _nodeCollection.RemoveAt(index);
        }

        /// <summary>
        /// Builds or rebuilds the entire level
        /// </summary>
        private void BuildLevel()
        {
            _nodeCollection.Clear();
            foreach (object item in _tree.Items)
            {
                BindingTreeNode node = new BindingTreeNode(_tree, item);
                _nodeCollection.Add(node);
                node.UpdateDisplay();
            }
        }

    }
}
