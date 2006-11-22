using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class BindableTreeView : UserControl
    {
        class SmartTreeNode : TreeNode
        {
            private bool _subtreeBuilt;
            private ITreeNode _dataNode;

            public SmartTreeNode(ITreeNode dataNode)
                :base(dataNode.NodeText)
            {
                _dataNode = dataNode;
                _subtreeBuilt = false;
                this.Nodes.Add(new TreeNode("dummy"));
            }

            public bool IsSubTreeBuilt { get { return _subtreeBuilt; } }

            public void BuildSubTree()
            {
                if (!_subtreeBuilt)
                {
                    this.Nodes.Clear(); // remove the dummy node

                    if (_dataNode.ChildNodes != null)
                    {
                        foreach (ITreeNode dataChild in _dataNode.ChildNodes)
                        {
                            SmartTreeNode treeChild = new SmartTreeNode(dataChild);
                            this.Nodes.Add(treeChild);
                        }
                        _subtreeBuilt = true;
                    }
                }
            }
        }



        private ITreeNode _rootNode;
        private ITreeNodeCollection _rootNodes;

        public BindableTreeView()
        {
            InitializeComponent();
        }

        public ITreeNodeCollection RootNodes
        {
            get { return _rootNodes; }
            set
            {
                _rootNodes = value;
                _treeCtrl.Nodes.Clear();

                if (_rootNodes != null)
                {
                    foreach (ITreeNode dataNode in _rootNodes)
                    {
                        _treeCtrl.Nodes.Add(new SmartTreeNode(dataNode));
                    }
                }
            }
        }

        public ITreeNode RootNode
        {
            get { return _rootNode; }
            set
            {
                _rootNode = value;
                _treeCtrl.Nodes.Clear();

                if (_rootNode != null)
                {
                    _treeCtrl.Nodes.Add(new SmartTreeNode(_rootNode));
                }
            }
        }

        public void ExpandAll()
        {
            _treeCtrl.ExpandAll();
        }

        [DefaultValue(true)]
        public bool ShowToolbar
        {
            get { return _toolStrip.Visible; }
            set { _toolStrip.Visible = value; }
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
    }
}
