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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Tree-view that binds to an instance of an <see cref="ITree"/>, which acts as data-source.
    /// Also has built-in drag & drop support, delegating drop decisions to the underlying <see cref="ITree"/>.
    /// </summary>
    public partial class BindingTreeView : UserControl
    {
        private ITree _root;
        private BindingTreeLevelManager _rootLevelManager;
        private event EventHandler _selectionChanged;
        private event EventHandler _nodeMouseDoubleClicked;
		private event EventHandler _nodeMouseClicked;
		private event EventHandler<ItemDragEventArgs> _itemDrag;

        private BindingTreeNode _dropTargetNode;
        private DragDropEffects _dropEffect;

        private ActionModelNode _toolbarModel;
        private ActionModelNode _menuModel;
        private bool _selectionDisabled = false;
    	private Keys _labelEditShortcut;

        private bool _isLoaded = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public BindingTreeView()
        {
            InitializeComponent();
        }

        #region Public members

        /// <summary>
        /// Gets or sets the model that this view looks at
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITree Tree
        {
            get { return _root; }
            set
            {
				if(value != _root)
				{
					if (_rootLevelManager != null)
					{
						// be sure to dispose of _rootLevelManager, in order to unsubscribe events, etc.
						_rootLevelManager.Dispose();
						_rootLevelManager = null;
					}

					_root = value;

					if (_root != null)
					{
						_rootLevelManager = new BindingTreeLevelManager(_root, _treeCtrl.Nodes, _treeCtrl);
					}
				}
            }
        }

        /// <summary>
        /// Gets or sets the current selection
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISelection Selection
        {
            get
            {
                return GetSelectionHelper();
            }
            set
            {
                // if someone tries to assign null, just convert it to an empty selection - this makes everything easier
                ISelection newSelection = value ?? new Selection();

                // get the existing selection
                ISelection existingSelection = GetSelectionHelper();

                if (!existingSelection.Equals(newSelection))
                {
                    if (newSelection.Item == null)
                    {
                        _treeCtrl.SelectedNode = null;
						EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
                    }
                    else
                    {
                        _treeCtrl.SelectedNode = FindNodeRecursive(_treeCtrl.Nodes, delegate(BindingTreeNode node) { return node.DataBoundItem == newSelection.Item; });
                    }

                    // we don't need to fire SelectionChanged here, because setting _treeCtrl.SelectedNode will do that for us indirectly
					// except when _treeCtrl.SelectedNode is set to null
                }
            }
        }

		[Obsolete("Toolstrip item display style is controlled by ToolStripBuilder.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RightToLeft ToolStripRightToLeft
        {
            get { return RightToLeft.No; }
            set { }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
            set
            {
                _toolbarModel = value;
                if (_isLoaded) InitializeToolStrip();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;
				if (_isLoaded) InitializeMenu();
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
        /// Notifies that the selection is double clicked
        /// </summary>
        public event EventHandler NodeMouseDoubleClicked
        {
            add { _nodeMouseDoubleClicked += value; }
            remove { _nodeMouseDoubleClicked -= value; }
        }

		/// <summary>
		/// Notifies that the selection is clicked
		/// </summary>
		public event EventHandler NodeMouseClicked
		{
			add { _nodeMouseClicked += value; }
			remove { _nodeMouseClicked -= value; }
		}

		/// <summary>
		/// Start editing the selected node.
		/// </summary>
		public void EditSelectedNode()
		{
			if (!_treeCtrl.LabelEdit || _treeCtrl.SelectedNode == null)
				return;
			
			_treeCtrl.SelectedNode.BeginEdit();
		}

		/// <summary>
        /// Expands the entire tree
        /// </summary>
        public void ExpandAll()
        {
            _treeCtrl.ExpandAll();
        }

        #endregion

        #region Design time properties

        [DefaultValue(true)]
        public bool ShowToolbar
        {
            get { return _toolStrip.Visible; }
            set { _toolStrip.Visible = value; }
        }

        [DefaultValue(false)]
        public bool FullRowSelect
        {
            get { return _treeCtrl.FullRowSelect; }
            set { _treeCtrl.FullRowSelect = value; }
        }

        [DefaultValue(true)]
        public bool ShowLines
        {
            get { return _treeCtrl.ShowLines; }
            set { _treeCtrl.ShowLines = value; }
        }

        [DefaultValue(true)]
        public bool ShowPlusMinus
        {
            get { return _treeCtrl.ShowPlusMinus; }
            set { _treeCtrl.ShowPlusMinus = value; }
        }

        [DefaultValue(true)]
        public bool ShowRootLines
        {
            get { return _treeCtrl.ShowRootLines; }
            set { _treeCtrl.ShowRootLines = value;}
        }

        [DefaultValue(false)]
        public bool CheckBoxes
        {
            get { return _treeCtrl.CheckBoxes; }
            set { _treeCtrl.CheckBoxes = value; }
        }

        [DefaultValue(false)]
        public bool SelectionDisabled
        {
            get { return _selectionDisabled; }
            set
            {
                _selectionDisabled = value;
            }
        }

    	[DefaultValue(KnownColor.Window)]
		public Color TreeBackColor
    	{
			get { return _treeCtrl.BackColor; }	
			set { _treeCtrl.BackColor = value; }	
    	}

		[DefaultValue(KnownColor.WindowText)]
		public Color TreeForeColor
		{
			get { return _treeCtrl.ForeColor; }
			set { _treeCtrl.ForeColor = value; }
		}

		[DefaultValue(KnownColor.Black)]
		public Color TreeLineColor
		{
			get { return _treeCtrl.LineColor; }
			set { _treeCtrl.LineColor = value; }
		}

		[DefaultValue(Keys.None)]
    	public Keys LabelEditShortcut
    	{
			get { return _labelEditShortcut; }
			set { _labelEditShortcut = value; }
    	}

		[Obsolete("Toolstrip item display style is controlled by ToolStripBuilder.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolStripItemDisplayStyle ToolStripItemDisplayStyle
        {
			get { return ToolStripItemDisplayStyle.Image; }
			set {  }
        }

        public Size IconSize
        {
            get { return _imageList.ImageSize; }
            set { _imageList.ImageSize = value; }
        }

        public ColorDepth IconColorDepth
        {
            get { return _imageList.ColorDepth; }
            set { _imageList.ColorDepth = value; }
        }

        public event EventHandler<ItemDragEventArgs> ItemDrag
        {
            add { _itemDrag += value; }
            remove { _itemDrag -= value; }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Obtains the current selection
        /// </summary>
        /// <returns></returns>
        private ISelection GetSelectionHelper()
        {
            BindingTreeNode selNode = (BindingTreeNode)_treeCtrl.SelectedNode;
            return selNode == null ? new Selection() : new Selection(selNode.DataBoundItem);
        }

        /// <summary>
        /// Searches the tree depth-first for a node matching the specified criteria
        /// </summary>
        /// <param name="nodeCollection"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private BindingTreeNode FindNodeRecursive(TreeNodeCollection nodeCollection, Predicate<BindingTreeNode> criteria)
        {
            foreach (TreeNode node in nodeCollection)
            {
                //  Bug #871
                //  See BindingTreeNode.UpdateDisplay():  
                //  the Nodes property may contain a "dummy" TreeNode, so ensure each iterated TreeNode is actually a BindingTreeNode
                BindingTreeNode bindingTreeNode = node as BindingTreeNode;

                if (bindingTreeNode != null && criteria(bindingTreeNode))
                {
                    return bindingTreeNode;
                }
                else
                {
                    BindingTreeNode nodeFound = FindNodeRecursive(node.Nodes, criteria);
                    if (nodeFound != null)
                        return nodeFound;
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
            BindingTreeNode expandingNode = (BindingTreeNode)e.Node;
            if (!expandingNode.IsSubTreeBuilt)
            {
                expandingNode.BuildSubTree();
            }
        }

        /// <summary>
        /// Notify that the <see cref="Selection"/> property has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _treeCtrl_AfterSelect(object sender, TreeViewEventArgs e)
        {
			BindingTreeNode selNode = (BindingTreeNode)_treeCtrl.SelectedNode;
			_treeCtrl.LabelEdit = selNode != null && selNode.CanSetNodeText();
			
			EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
        }

        private void InitializeToolStrip()
        {
            ToolStripBuilder.Clear(_toolStrip.Items);
            if (_toolbarModel != null)
            {
                ToolStripBuilder.BuildToolbar(_toolStrip.Items, _toolbarModel.ChildNodes);
            }
        }
		private void InitializeMenu()
		{
			ToolStripBuilder.Clear(_contextMenu.Items);
			if (_menuModel != null)
			{
				ToolStripBuilder.BuildMenu(_contextMenu.Items, _menuModel.ChildNodes);
			}
		}

        private void _contextMenu_Opening(object sender, CancelEventArgs e)
        {
            // Find the node we're on
            Point pt = _treeCtrl.PointToClient(TreeView.MousePosition);
            BindingTreeNode node = (BindingTreeNode)_treeCtrl.GetNodeAt(pt.X, pt.Y);
            _treeCtrl.SelectedNode = node;
			if (node == null)
				EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
        }

        private void _contextMenu_Opened(object sender, EventArgs e)
        {

        }

        private void _contextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {

        }

        private void _contextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {

        }

        private void BindingTreeView_Load(object sender, EventArgs e)
        {
			InitializeMenu();
			InitializeToolStrip();
            _isLoaded = true;
        }

        private void _treeCtrl_AfterCheck(object sender, TreeViewEventArgs e)
        {
            BindingTreeNode node = e.Node as BindingTreeNode;
            if (node != null)
            {
                node.OnChecked();
            }
        }

        private void _treeCtrl_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (_selectionDisabled)
            {
                e.Cancel = true;
            }
        }

        private void _treeCtrl_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            EventsHelper.Fire(_nodeMouseDoubleClicked, this, e);
        }

		private void _treeCtrl_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			EventsHelper.Fire(_nodeMouseClicked, this, e);
		}

		#endregion

        #region Drag Drop support

        /// <summary>
        /// Called when an object is first dragged into this control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            // clear any record of a previous drop target node
            _dropTargetNode = null;
            _dropEffect = DragDropEffects.None;

            base.OnDragEnter(e);
        }

        /// <summary>
        /// Called repeatedly as the object is dragged within this control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragOver(DragEventArgs e)
        {
            // determine the node under the cursor
            BindingTreeNode node = (BindingTreeNode)_treeCtrl.GetNodeAt(_treeCtrl.PointToClient(new Point(e.X, e.Y)));
 
            // determine what effect the user is trying to accomplish
            DragDropEffects desiredEffect = GetDragDropDesiredEffect(e);

            // optimization: only care if different than the last known drop-target node, or different desired effect
            if (node != _dropTargetNode || desiredEffect != _dropEffect)
            {
                _treeCtrl.BeginUpdate();    // suspend drawing

                // un-highlight the last known drop-target node
                HighlightNode(_dropTargetNode, false);

                // set the drop target node to this node
                _dropTargetNode = node;
                _dropEffect = desiredEffect;


                // check if drop target node exists and what kind of operation it will accept
                DragDropKind acceptableKind = (_dropTargetNode == null) ?
                    DragDropKind.None : _dropTargetNode.CanAcceptDrop(GetDragDropData(e), GetDragDropKind(desiredEffect));

                // display the appropriate effect cue based on the result
                e.Effect = GetDragDropEffect(acceptableKind);

                // if the drop target is valid and willing to accept data, highlight it
                if (acceptableKind != DragDropKind.None)
                {
                    HighlightNode(_dropTargetNode, true);
                }

                _treeCtrl.EndUpdate(); // resume drawing
            }
            
            base.OnDragOver(e);
        }

        /// <summary>
        /// Called when an object is dropped onto this control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragDrop(DragEventArgs e)
        {
            // is there a current drop-target node?
            if (_dropTargetNode != null)
            {
                try
                {
					object dragDropData = GetDragDropData(e);
					
					// ask the node to accept the drop
					DragDropKind result = _dropTargetNode.AcceptDrop(dragDropData, GetDragDropKind(e.Effect));

                    // be sure to set the resulting effect in the event args, so that it gets communicated
                    // back to the initiator of the drag drop operation
                    e.Effect = GetDragDropEffect(result);

					this.Selection = new Selection(dragDropData);
				}
                catch (Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex);
                }

                // remove highlighting from drop target node
                HighlightNode(_dropTargetNode, false);
            }

            // clear the drop target node
            _dropTargetNode = null;
            _dropEffect = DragDropEffects.None;

            base.OnDragDrop(e);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            // is there a current drop-target node?
            if (_dropTargetNode != null)
            {
                // remove highlighting from drop target node
                HighlightNode(_dropTargetNode, false);
            }

            // clear the drop target node
            _dropTargetNode = null;
            _dropEffect = DragDropEffects.None;

            base.OnDragLeave(e);
        }

        /// <summary>
        /// Highlights or un-highlights the specified node, without altering the current selection
        /// </summary>
        /// <param name="node"></param>
        /// <param name="highlight"></param>
        private void HighlightNode(TreeNode node, bool highlight)
        {
            if (node != null)
            {
                node.BackColor = highlight ? SystemColors.Highlight : _treeCtrl.BackColor;
                node.ForeColor = highlight ? SystemColors.HighlightText : _treeCtrl.ForeColor;
            }
        }

        /// <summary>
        /// Returns the desired effect, chosen from the allowed effects in the DragEventArgs.
        /// The user indicates the desired effect (move/copy/link) by using modifier keys.
        /// Copied this logic from MSDN - seems to be standard windows convention.  
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private DragDropEffects GetDragDropDesiredEffect(DragEventArgs e)
        {
            // Set the effect based upon the KeyState.
            if ((e.KeyState & (8 + 32)) == (8 + 32) &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // KeyState 8 + 32 = CTL + ALT
                // Link drag-and-drop effect.
                return DragDropEffects.Link;
            }
            else if ((e.KeyState & 32) == 32 &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // ALT KeyState for link.
                return DragDropEffects.Link;
            }
            else if ((e.KeyState & 4) == 4 &&
              (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // SHIFT KeyState for move.
                return DragDropEffects.Move;
            }
            else if ((e.KeyState & 8) == 8 &&
              (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                // CTL KeyState for copy.
                return DragDropEffects.Copy;
            }
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // By default, the drop action should be move, if allowed.
                return DragDropEffects.Move;
            }

            return DragDropEffects.None;
        }

        /// <summary>
        /// Converts a <see cref="DragDropEffects"/>, which is WinForms specific, to a <see cref="DragDropKind"/>
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        private DragDropKind GetDragDropKind(DragDropEffects effect)
        {
            if ((effect & DragDropEffects.Copy) == DragDropEffects.Copy)
                return DragDropKind.Copy;
            if ((effect & DragDropEffects.Move) == DragDropEffects.Move)
                return DragDropKind.Move;

            // other effects are not currently supported by this control, so just return Move
            return DragDropKind.Move;
        }

        /// <summary>
        /// Converts a <see cref="DragDropKind"/> to the WinForms <see cref="DragDropEffects"/>
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        private DragDropEffects GetDragDropEffect(DragDropKind kind)
        {
            switch (kind)
            {
                case DragDropKind.Move:
                    return DragDropEffects.Move;
                case DragDropKind.Copy:
                    return DragDropEffects.Copy;
                default:
                    return DragDropEffects.None;
            }
        }

        /// <summary>
        /// Extracts the drag-drop data from the event args, assuming this is an in-process drop.
        /// Out-of-process drops are not currently supported by this control.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private object GetDragDropData(DragEventArgs e)
        {
            IDataObject dao = e.Data;
            string[] formats = dao.GetFormats();

            // use any available format, since we are assuming the data is in-process
            return formats.Length > 0 ? dao.GetData(formats[0]) : null;
        }

        #endregion

        private void _treeCtrl_ItemDrag(object sender, ItemDragEventArgs e)
        {
			// the item being dragged should be selected as well.
			BindingTreeNode node = (BindingTreeNode)e.Item;
			_treeCtrl.SelectedNode = node;

            ItemDragEventArgs args = new ItemDragEventArgs(e.Button, this.GetSelectionHelper());
            EventsHelper.Fire(_itemDrag, this, args);
        }

		private void _treeCtrl_AfterExpand(object sender, TreeViewEventArgs e)
		{
			BindingTreeNode node = e.Node as BindingTreeNode;
			if (node != null)
			{
				node.OnExpandCollapse();
			}
		}

		private void _treeCtrl_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			BindingTreeNode node = e.Node as BindingTreeNode;
			if (node != null)
			{
				node.OnExpandCollapse();
			}
		}

		private void _treeCtrl_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Label))
			{
				// user cancel the edit, stop editing
				e.CancelEdit = true;
			}
			else
			{
				BindingTreeNode node = e.Node as BindingTreeNode;
				if (node != null)
				{
					node.AfterLabelEdit(e.Label);
				}
			}
		}

		private void _treeCtrl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == _labelEditShortcut)
			{
				EditSelectedNode();
			}
		}
    }
}
