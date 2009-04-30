#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.ExtensionBrowser.View.WinForms
{
    /// <summary>
    /// User-interface for the <see cref="ExtensionBrowserComponent"/>
    /// </summary>
    public partial class ExtensionBrowserControl : UserControl
    {
        private ExtensionBrowserComponent _browser;

        /// <summary>
        /// Default constructor.  Must be initialized with the instance of <see cref="ExtensionBrowserComponent"/>
        /// that it will interact with.
        /// </summary>
        /// <param name="browser"></param>
        public ExtensionBrowserControl(ExtensionBrowserComponent browser)
        {
            InitializeComponent();

            _browser = browser;

            _pluginTreeView.BeforeExpand += new TreeViewCancelEventHandler(TreeView_BeforeExpand);
            _extPointTreeView.BeforeExpand += new TreeViewCancelEventHandler(TreeView_BeforeExpand);

            BuildTreeView(_pluginTreeView, _browser.PluginTree);
            BuildTreeView(_extPointTreeView, _browser.ExtensionPointTree);

        }

        /// <summary>
        /// When the user is about to expand a node, need to build the level beneath it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Builds the root and first-level of the tree
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="dataRoot"></param>
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

        /// <summary>
        /// Called to build subsequent levels of the tree as they are expanded
        /// </summary>
        /// <param name="treeNode"></param>
        private void BuildNextTreeLevel(TreeNode treeNode)
        {
            IBrowserNode dataNode = (IBrowserNode)treeNode.Tag;
            foreach (IBrowserNode dataChild in dataNode.ChildNodes)
            {
                TreeNode treeChild = new TreeNode(dataChild.DisplayName);
                treeChild.Tag = dataChild;
                treeChild.ForeColor = dataChild.Enabled ? treeNode.TreeView.ForeColor : Color.DimGray;
                treeNode.Nodes.Add(treeChild);
            }
        }

    }
}
