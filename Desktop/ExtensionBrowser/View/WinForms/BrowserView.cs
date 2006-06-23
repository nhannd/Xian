using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Workstation.View.WinForms;
using ClearCanvas.Workstation.ExtensionBrowser;
using ClearCanvas.Common.Application.Tools;

namespace ClearCanvas.Workstation.ExtensionBrowser.View.WinForms
{
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Workstation.ExtensionBrowser.ExtensionBrowserViewExtensionPoint))]
    public class BrowserView : WinFormsView, IToolView
    {
        private ExtensionBrowserTool _browser;
        private BrowserControl _browserControl;
       
        public BrowserView()
        {
        }

        #region IToolView Members

        public void SetTool(ITool tool)
        {
            _browser = (ExtensionBrowserTool)tool;
            BuildTreeView(Control.PluginTree, _browser.PluginTree);
            BuildTreeView(Control.ExtPointTree, _browser.ExtensionPointTree);
        }

        #endregion

        protected BrowserControl Control
        {
            get
            {
                if (_browserControl == null)
                {
                    _browserControl = new BrowserControl();
                    _browserControl.PluginTree.BeforeExpand += new TreeViewCancelEventHandler(TreeView_BeforeExpand);
                    _browserControl.ExtPointTree.BeforeExpand += new TreeViewCancelEventHandler(TreeView_BeforeExpand);
                }
                return _browserControl;
            }
        }

        private void TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode expandingNode = e.Node;
            foreach (TreeNode subNode in expandingNode.Nodes)
            {
                // only build the next level if it hasn't been built previously
                if (subNode.Nodes.Count == 0)
                {
                    BuildNextTreeLevel(subNode);
                }
            }
        }

        private void BuildTreeView(TreeView treeView, IBrowserNode dataRoot)
        {
            treeView.Nodes.Clear();
            if (dataRoot != null)
            {
                TreeNode treeRoot = new TreeNode(dataRoot.DisplayName);
                treeRoot.Tag = dataRoot;
                treeView.Nodes.Add(treeRoot);
                BuildNextTreeLevel(treeRoot);
            }
        }

        private void BuildNextTreeLevel(TreeNode treeNode)
        {
            IBrowserNode dataNode = (IBrowserNode)treeNode.Tag;
            foreach (IBrowserNode dataChild in dataNode.ChildNodes)
            {
                TreeNode treeChild = new TreeNode(dataChild.DisplayName);
                treeChild.Tag = dataChild;
                treeNode.Nodes.Add(treeChild);
            }
        }

        public override object GuiElement
        {
            get { return Control; }
        }
    }
}
