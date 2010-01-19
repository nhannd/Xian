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
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Tree node that knows how to build its subtree on demand from an <see cref="ITree"/> model.  This class
    /// is used internally and is not intended to be used directly by application code.
    /// </summary>
    internal class BindingTreeNode : TreeNode, IDisposable
    {
		/// <summary>
		/// need to cache our own reference to the treeView, because during construction the base-class TreeView property is null
		/// </summary>
		private readonly TreeView _treeView;
		private readonly ITree _parentTree;
		private BindingTreeLevelManager _subtreeManager;
        private object _item;
        private bool _isSubTreeBuilt;

		public BindingTreeNode(ITree parentTree, object item, TreeView treeView)
            : base(parentTree.Binding.GetNodeText(item))
        {
            _item = item;
            _parentTree = parentTree;
			_treeView = treeView;

			UpdateDisplay(_treeView.ImageList);
        }

        /// <summary>
        /// The item that this node represents
        /// </summary>
        public object DataBoundItem
        {
            get { return _item; }
            set
            {
                if (value != _item)
                {
                    _item = value;
                    UpdateDisplay();
                }
            }
        }

		/// <summary>
		/// Updates the displayable properties of this node, based on the underlying model
		/// </summary>
		public void UpdateDisplay()
        {
			UpdateDisplay(_treeView.ImageList);
        }

        /// <summary>
        /// Returns true if the sub-tree of this node has been built
        /// </summary>
        public bool IsSubTreeBuilt
        {
            get { return _isSubTreeBuilt; }
        }

        /// <summary>
        /// Forces the sub-tree to be built
        /// </summary>
        public void BuildSubTree()
        {
            if (!_isSubTreeBuilt)
            {
                _isSubTreeBuilt = true;
                RebuildSubTree();
            }
        }

        /// <summary>
        /// Asks the item if it can accept the specifid drop
        /// </summary>
        /// <param name="dropData"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public DragDropKind CanAcceptDrop(object dropData, DragDropKind kind)
        {
            return _parentTree.Binding.CanAcceptDrop(_item, dropData, kind);
        }

		/// <summary>
		/// Asks if the item text can be changed.
		/// </summary>
		/// <returns></returns>
		public bool CanSetNodeText()
		{
			return _parentTree.Binding.CanSetNodeText(_item);
		}

        /// <summary>
        /// Asks the item to accept the specified drop
        /// </summary>
        /// <param name="dropData"></param>
        /// <param name="kind"></param>
        public DragDropKind AcceptDrop(object dropData, DragDropKind kind)
        {
            return _parentTree.Binding.AcceptDrop(_item, dropData, kind);
        }

		private void UpdateDisplay(ImageList treeViewImageList)
		{
			if (this.TreeView != null)
				this.TreeView.BeginUpdate();

			// update all displayable attributes from the binding
			this.Text = _parentTree.Binding.GetNodeText(_item);
			this.ToolTipText = _parentTree.Binding.GetTooltipText(_item);

			this.Checked = _parentTree.Binding.GetIsChecked(_item);

			if (_parentTree.Binding.GetIsHighlighted(_item))
				this.BackColor = System.Drawing.Color.FromArgb(124, 177, 221);

			if (treeViewImageList != null)
			{
				var resolver = _parentTree.Binding.GetResourceResolver(_item);
				var iconSet = _parentTree.Binding.GetIconSet(_item);
				var imageCollection = treeViewImageList.Images;
				if (iconSet == null)
				{
					this.ImageIndex = -1;
				}
				else if (imageCollection.ContainsKey(iconSet.MediumIcon))
				{
					this.ImageIndex = imageCollection.IndexOfKey(iconSet.MediumIcon);
				}
				else
				{
					try
					{
						imageCollection.Add(iconSet.MediumIcon, IconFactory.CreateIcon(iconSet.MediumIcon, resolver));
						this.ImageIndex = imageCollection.IndexOfKey(iconSet.MediumIcon);
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
						this.ImageIndex = -1;
					}
				}

				this.SelectedImageIndex = this.ImageIndex;
			}

			// if the subtree was already built, we may need to rebuild it if it has changed
			if (_isSubTreeBuilt)
			{
				// rebuild the subtree only if the binding returns a different reference
				if (_parentTree.Binding.GetSubTree(_item) != _subtreeManager.Tree)
					RebuildSubTree();
			}
			else
			{
				// add a dummy child so that we get a "plus" sign next to the node
				if (_parentTree.Binding.CanHaveSubTree(_item) && this.Nodes.Count == 0)
				{
					this.Nodes.Add(new TreeNode(SR.MessageErrorRetrievingSubtree));
				}
			}

			if (this.TreeView != null)
				this.TreeView.EndUpdate();

			// check whether to start in expanded or collapsed state
			if (_parentTree.Binding.GetExpanded(_item))
				this.Expand();
			else
				this.Collapse();
		}

		/// <summary>
        /// Rebuilds the sub-tree
        /// </summary>
        private void RebuildSubTree()
        {
			// dispose of the old sub-tree manager
			if(_subtreeManager != null)
			{
				_subtreeManager.Dispose();
				_subtreeManager = null;
			}
			
			// get the sub-tree from the binding
			// note: there is no guarantee that successive calls will return the same ITree instance,
			// which is why we create a new BindingTreeLevelManager
            var subTree = _parentTree.Binding.GetSubTree(_item);
            if (subTree != null)
            {
				_subtreeManager = new BindingTreeLevelManager(subTree, this.Nodes, _treeView);
            }
        }

		public void AfterLabelEdit(string text)
		{
			_parentTree.Binding.SetNodeText(_item, text);
		}

        public void OnChecked()
        {
            _parentTree.Binding.SetIsChecked(_item, this.Checked);
        }

		public void OnExpandCollapse()
		{
			_parentTree.Binding.SetExpanded(_item, this.IsExpanded);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if(_subtreeManager != null)
			{
				_subtreeManager.Dispose();
				_subtreeManager = null;
			}
		}

		#endregion
	}
}
