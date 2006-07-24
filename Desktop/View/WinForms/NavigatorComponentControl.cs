using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class NavigatorComponentControl : UserControl
    {
        private NavigatorComponent _component;
        private Dictionary<NavigatorNode, TreeNode> _nodeMap;

        public NavigatorComponentControl(NavigatorComponent component)
        {
            InitializeComponent();

            _nodeMap = new Dictionary<NavigatorNode, TreeNode>();

            _headerStrip.HeaderStyle = AreaHeaderStyle.Small;

            _component = component;
            _component.CurrentNodeChanged += new EventHandler(_component_CurrentNodeChanged);

            _nextButton.DataBindings.Add("Enabled", _component, "ForwardEnabled");
            _backButton.DataBindings.Add("Enabled", _component, "BackEnabled");

            foreach (NavigatorNode node in _component.Nodes)
            {
                Control c = (Control)node.ComponentView.GuiElement;
                c.Visible = false;
                c.Dock = DockStyle.Fill;
                _contentPanel.Controls.Add(c);

                AddTreeNode(node, _treeView.Nodes, 0);
            }

            // expand first-level of tree
            foreach (TreeNode treeNode in _treeView.Nodes)
            {
                treeNode.Expand();
            }

            SelectNode(_component.CurrentNode);
        }

        private void _component_CurrentNodeChanged(object sender, EventArgs e)
        {
            SelectNode(_component.CurrentNode);
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _nextButton_Click(object sender, EventArgs e)
        {
            _component.Forward();
        }

        private void _backButton_Click(object sender, EventArgs e)
        {
            _component.Back();
        }

        private void AddTreeNode(NavigatorNode node, TreeNodeCollection treeNodes, int depth)
        {
            PathSegment segment = node.NodePath.Segments[depth];

            TreeNode[] matches = treeNodes.Find(segment.LocalizedText, false);
            TreeNode treeNode = (matches.Length > 0) ? matches[0] :
                    treeNodes.Add(segment.LocalizedText, segment.LocalizedText);

            if (depth < node.NodePath.Segments.Length - 1)
            {
                // recur on next path segment
                AddTreeNode(node, treeNode.Nodes, depth + 1);
            }
            else
            {
                // this is the last path segment
                treeNode.Tag = node;
                _nodeMap.Add(node, treeNode);
            }
        }

        private void SelectNode(NavigatorNode node)
        {
            foreach (NavigatorNode n in _component.Nodes)
            {
                Control c = (Control)n.ComponentView.GuiElement;
                c.Visible = false;
            }
            Control toShow = (Control)node.ComponentView.GuiElement;
            toShow.Visible = true;
            _title.Text = node.NodePath.LastSegment.LocalizedText;
            _treeView.SelectedNode = _nodeMap[node];
        }

        private void _treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _component.CurrentNode = (NavigatorNode)e.Node.Tag;
        }
    }
}
