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
    public partial class NavigatorComponentContainerControl : CustomUserControl
    {
        private NavigatorComponentContainer _component;
        private Dictionary<NavigatorPage, TreeNode> _nodeMap;
        private List<Control> _createdControls;

        public NavigatorComponentContainerControl(NavigatorComponentContainer component)
        {
            InitializeComponent();

            _nodeMap = new Dictionary<NavigatorPage, TreeNode>();
            _createdControls = new List<Control>();        

            _component = component;
            _component.CurrentPageChanged += new EventHandler(_component_CurrentNodeChanged);

            _nextButton.DataBindings.Add("Enabled", _component, "ForwardEnabled");
            _backButton.DataBindings.Add("Enabled", _component, "BackEnabled");
            _okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled");

            // add a node to the tree for each page
            foreach (NavigatorPage page in _component.Pages)
            {
                AddTreeNode(page, _treeView.Nodes, 0);
            }

            // expand first-level of tree
            foreach (TreeNode treeNode in _treeView.Nodes)
            {
                treeNode.Expand();
            }

            ShowPage(_component.CurrentPage);
        }

        private void _component_CurrentNodeChanged(object sender, EventArgs e)
        {
            ShowPage(_component.CurrentPage);
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Accept();
            }
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

        private void ShowPage(NavigatorPage page)
        {
            // get the control to show
            Control toShow = (Control)_component.GetPageView(page).GuiElement;

            // hide all others
            foreach (Control c in _contentPanel.Controls)
            {
                if (c != toShow)
                    c.Visible = false;
            }

            // if the control has not been added to the content panel, add it now
            if (!_contentPanel.Controls.Contains(toShow))
            {
                toShow.Dock = DockStyle.Fill;
                _contentPanel.Controls.Add(toShow);
            }

            toShow.Visible = true;

            // HACK: for some reason the error provider symbols don't show up the first time the control is shown
            // therefore we need to force it
            if (toShow is ApplicationComponentUserControl)
            {
                (toShow as ApplicationComponentUserControl).ErrorProvider.UpdateBinding();
            }

            // set the title and selected tree node
            _titleBar.Text = page.Path.LastSegment.LocalizedText;
            _treeView.SelectedNode = _nodeMap[page];
        }

        private void _treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _component.CurrentPage = (NavigatorPage)e.Node.Tag;
        }
    }
}
