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

        private BindingTreeNode _dropTargetNode;
        private DragDropEffects _dropEffect;

        private ActionModelNode _toolbarModel;
        private ActionModelNode _menuModel;
        private ToolStripItemDisplayStyle _toolStripItemDisplayStyle = ToolStripItemDisplayStyle.Image;
        private ToolStripItemAlignment _toolStripItemAlignment = ToolStripItemAlignment.Right;
        private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;

        private bool _isLoaded = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public BindingTreeView()
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
                    _rootLevelManager = new BindingTreeLevelManager(_root, _treeCtrl.Nodes);
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
                ISelection newSelection = value ?? new Selection();

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
                        _treeCtrl.SelectedNode = FindNodeRecursive(_treeCtrl.Nodes, delegate(BindingTreeNode node) { return node.DataBoundItem == newSelection.Item; });
                    }

                    // we don't need to fire SelectionChanged here, because setting _treeCtrl.SelectedNode will do that for us indirectly
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

        public ImageList ImageList
        {
            get { return _treeCtrl.ImageList; }
            set { _treeCtrl.ImageList = value; }
        }

        [Obsolete("Do not use.  Toolstrip item alignment is now controlled by application setting")]
        public RightToLeft ToolStripRightToLeft
        {
            get { return RightToLeft.No; }
            set { }
        }

        public ToolStripItemDisplayStyle ToolStripItemDisplayStyle
        {
            get { return _toolStripItemDisplayStyle; }
            set { _toolStripItemDisplayStyle = value; }
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
                //  Bug #
                //  See BindingTreeNode.UpdateDisplay():  the Nodes property may contain a "dummy" TreeNode, so ensure each iterated TreeNode is actually a BindingTreeNode
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
            EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
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
                    // ask the node to accept the drop
                    DragDropKind result = _dropTargetNode.AcceptDrop(GetDragDropData(e), GetDragDropKind(e.Effect));

                    // be sure to set the resulting effect in the event args, so that it gets communicated
                    // back to the initiator of the drag drop operation
                    e.Effect = GetDragDropEffect(result);
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

        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
            set
            {
                _toolbarModel = value;
                if(_isLoaded) InitializeToolStrip();
            }
        }

        private void InitializeToolStrip()
        {
            ToolStripBuilder.Clear(_toolStrip.Items);
            if (_toolbarModel != null)
            {
                if (_toolStripItemAlignment == ToolStripItemAlignment.Right)
                {
                    _toolbarModel.ChildNodes.Reverse();
                }

                ToolStripBuilder.BuildToolbar(
                    _toolStrip.Items,
                    _toolbarModel.ChildNodes,
                    new ToolStripBuilder.ToolStripBuilderStyle(_toolStripItemDisplayStyle, _toolStripItemAlignment, _textImageRelation));
            }
        }

        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;
                ToolStripBuilder.Clear(_contextMenu.Items);
                if (_menuModel != null)
                {
                    ToolStripBuilder.BuildMenu(_contextMenu.Items, _menuModel.ChildNodes);
                }
            }
        }

        private void _contextMenu_Opening(object sender, CancelEventArgs e)
        {
            // Find the node we're on
            Point pt = _treeCtrl.PointToClient(TreeView.MousePosition);
            BindingTreeNode node = (BindingTreeNode)_treeCtrl.GetNodeAt(pt.X, pt.Y);
            _treeCtrl.SelectedNode = node;
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
            if (this.DesignMode == false)
            {
                _toolStripItemAlignment = DesktopViewSettings.Default.LocalToolStripItemAlignment;
                _textImageRelation = DesktopViewSettings.Default.LocalToolStripItemTextImageRelation;
            }
            else
            {
                _toolStripItemAlignment = ToolStripItemAlignment.Left;
                _textImageRelation = TextImageRelation.ImageBeforeText;
            }

            InitializeToolStrip();
            _isLoaded = true;
        }
    }
}
