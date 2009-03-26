#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public interface INavigatorApplyHandler
	{
		void Apply();
		bool ApplyEnabled { get; }
		event EventHandler ApplyEnabledChanged;
	}

    public partial class NavigatorComponentContainerControl : CustomUserControl
    {
        private NavigatorComponentContainer _component;
        private Dictionary<NavigatorPage, TreeNode> _nodeMap;
		private INavigatorApplyHandler _applyHandler;

		public NavigatorComponentContainerControl(NavigatorComponentContainer component)
			: this(component, null)
		{
		}

    	public NavigatorComponentContainerControl(NavigatorComponentContainer component, INavigatorApplyHandler applyHandler)
        {
            InitializeComponent();

			_applyHandler = applyHandler;
			ClearCanvasStyle.SetTitleBarStyle(_titleBar);

            _nodeMap = new Dictionary<NavigatorPage, TreeNode>();

            _component = component;
            _component.CurrentPageChanged += new EventHandler(_component_CurrentNodeChanged);

			if (applyHandler == null)
			{
				_applyButton.Dispose();
				_applyButton = null;
			}
			else
			{
				_applyButton.Click += delegate { _applyHandler.Apply(); };
				_applyButton.DataBindings.Add("Enabled", _applyHandler, "ApplyEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			}

    		base.AcceptButton = this._okButton;
			base.CancelButton = this._cancelButton;

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

            if (depth < page.Path.Segments.Count - 1)
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

        private void _treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            NavigatorPage page = (NavigatorPage)e.Node.Tag;
            if (page == null)
            {
                // no page associated with this node, select another node?
                e.Cancel = true;
            }
        }
    }
}
