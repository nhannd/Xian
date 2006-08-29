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
    public partial class NavigatorComponentContainerControl : UserControl
    {
        private NavigatorComponentContainer _component;
        private Dictionary<NavigatorPage, TreeNode> _nodeMap;

        public NavigatorComponentContainerControl(NavigatorComponentContainer component)
        {
            InitializeComponent();

            _nodeMap = new Dictionary<NavigatorPage, TreeNode>();

            _headerStrip.HeaderStyle = AreaHeaderStyle.Small;

            _component = component;
            _component.CurrentPageChanged += new EventHandler(_component_CurrentNodeChanged);

            _nextButton.DataBindings.Add("Enabled", _component, "ForwardEnabled");
            _backButton.DataBindings.Add("Enabled", _component, "BackEnabled");
            _okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled");

            foreach (NavigatorPage page in _component.Pages)
            {
                Control c = (Control)page.ComponentHost.ComponentView.GuiElement;
                c.Visible = false;
                c.Dock = DockStyle.Fill;
                _contentPanel.Controls.Add(c);

                AddTreeNode(page, _treeView.Nodes, 0);
            }

            // expand first-level of tree
            foreach (TreeNode treeNode in _treeView.Nodes)
            {
                treeNode.Expand();
            }

            SelectNode(_component.CurrentPage);
        }

        private void _component_CurrentNodeChanged(object sender, EventArgs e)
        {
            SelectNode(_component.CurrentPage);
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

        private void AddTreeNode(NavigatorPage page, TreeNodeCollection treeNodes, int depth)
        {
            PathSegment segment = page.Path.Segments[depth];

            TreeNode[] matches = treeNodes.Find(segment.LocalizedText, false);
            TreeNode treeNode = (matches.Length > 0) ? matches[0] :
                    treeNodes.Add(segment.LocalizedText, segment.LocalizedText);

            if (depth < page.Path.Segments.Length - 1)
            {
                // recur on next path segment
                AddTreeNode(page, treeNode.Nodes, depth + 1);
            }
            else
            {
                // this is the last path segment
                treeNode.Tag = page;
                _nodeMap.Add(page, treeNode);
            }
        }

        private void SelectNode(NavigatorPage page)
        {
            // set all pages to hidden except the selected page
            foreach (NavigatorPage p in _component.Pages)
            {
                if (p != page)
                {
                    Control c = (Control)p.ComponentHost.ComponentView.GuiElement;
                    c.Visible = false;
                }
            }

            Control toShow = (Control)page.ComponentHost.ComponentView.GuiElement;
            toShow.Visible = true;
            _title.Text = page.Path.LastSegment.LocalizedText;
            _treeView.SelectedNode = _nodeMap[page];
        }

        private void _treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _component.CurrentPage = (NavigatorPage)e.Node.Tag;
        }
    }
}
