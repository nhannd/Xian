#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	public class AbstractActionModelTreeBranch : AbstractActionModelTreeNode
	{
		private readonly Tree<AbstractActionModelTreeNode> _subtree = new Tree<AbstractActionModelTreeNode>(new Binding());
		private readonly List<AbstractActionModelTreeNode> _children = new List<AbstractActionModelTreeNode>();

		private bool _suspendSynchronizeParent = false;

		public AbstractActionModelTreeBranch(string groupName)
			: base(new PathSegment(groupName, groupName))
		{
			Initialize();
		}

		public AbstractActionModelTreeBranch(PathSegment pathSegment)
			: base(pathSegment)
		{
			Initialize();
		}

		private void Initialize()
		{
			_subtree.Items.ItemsChanged += OnSubtreeItemsChanged;
		}

		public IList<AbstractActionModelTreeNode> Children
		{
			get { return _subtree.Items; }
		}

		public bool IsEmpty
		{
			get
			{
				foreach (AbstractActionModelTreeNode actionModelTreeNode in this.Children)
				{
					if (actionModelTreeNode is AbstractActionModelTreeLeafAction)
						return false;
					else if (actionModelTreeNode is AbstractActionModelTreeBranch && !((AbstractActionModelTreeBranch) actionModelTreeNode).IsEmpty)
						return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Use only for performing initial population of the node.
		/// </summary>
		internal void AppendChild(AbstractActionModelTreeNode child)
		{
			_suspendSynchronizeParent = true;
			try
			{
				_subtree.Items.Add(child);
				_children.Add(child);
				child.Parent = this;
			}
			finally
			{
				_suspendSynchronizeParent = false;
			}
		}

		internal void NotifyChildChanged(AbstractActionModelTreeNode child)
		{
			_subtree.Items.NotifyItemUpdated(child);
		}

		protected ITree Subtree
		{
			get { return _subtree; }
		}

		protected void CreateActionModelRoot(ActionModelRoot actionModelRoot)
		{
			foreach (AbstractActionModelTreeNode child in this.Subtree.Items)
			{
				if (child is AbstractActionModelTreeBranch)
				{
					((AbstractActionModelTreeBranch) child).CreateActionModelRoot(actionModelRoot);
				}
				else if (child is AbstractActionModelTreeLeafAction)
				{
					actionModelRoot.InsertAction(((AbstractActionModelTreeLeafAction) child).GetAction());
				}
				else if (child is AbstractActionModelTreeLeafSeparator)
				{
					actionModelRoot.InsertSeparator(((AbstractActionModelTreeLeafSeparator) child).GetSeparatorPath());
				}
			}
		}

		private void OnSubtreeItemsChanged(object sender, ItemChangedEventArgs e)
		{
			SynchronizeParent((AbstractActionModelTreeNode) e.Item, e.ChangeType);
		}

		private void SynchronizeParent(AbstractActionModelTreeNode node, ItemChangeType changeType)
		{
			if (_suspendSynchronizeParent)
				return;

			switch (changeType)
			{
				case ItemChangeType.ItemAdded:
				case ItemChangeType.ItemInserted:
					// set the parent on the added node
					node.Parent = this;

					// sync the secondary list
					_children.Add(node);

					// force the node to expand
					this.IsExpanded = true;
					break;
				case ItemChangeType.ItemRemoved:
					// nullify the parent on the node if and only if it is us (just in case something silly happens and the parent is already updated)
					if (ReferenceEquals(this, node.Parent))
						node.Parent = null;

					// sync the secondary list
					_children.Remove(node);
					break;
				case ItemChangeType.ItemChanged:
					// replacing an item in the list can cause ItemChanged, thus we need to resync the entire list to update the replaced item
				case ItemChangeType.Reset:
				default:
					// resync the parent nodes using the secondary list as a reference for what was in the tree before the change
					foreach (AbstractActionModelTreeNode child in _children)
					{
						if (!_subtree.Items.Contains(child))
						{
							if (ReferenceEquals(this, node.Parent))
								node.Parent = null;
						}
					}
					foreach (AbstractActionModelTreeNode child in _subtree.Items)
					{
						if (!_children.Contains(child))
							child.Parent = this;
					}

					// update the secondary list
					_children.Clear();
					_children.AddRange(_subtree.Items);
					break;
			}
		}

		public override DragDropKind CanAcceptDrop(object dropData, DragDropKind dragDropKind, DragDropPosition dragDropPosition)
		{
			if (dragDropPosition == DragDropPosition.Default)
			{
				if (dropData is AbstractActionModelTreeNode)
				{
					AbstractActionModelTreeNode droppedNode = (AbstractActionModelTreeNode) dropData;
					if (dragDropKind == DragDropKind.Move)
					{
						// to drag-move, we can't be dragging onto ourself, onto our parent, or onto one of our descendants
						if (!ReferenceEquals(this, droppedNode)
						    && !ReferenceEquals(this, droppedNode.Parent)
						    && !this.IsDescendantOf(droppedNode as AbstractActionModelTreeBranch))
							return dragDropKind;
					}
				}
			}
			else if (dragDropPosition == DragDropPosition.After)
			{
				// prevent dropping something after an expanded branch node with children,
				// because the user is physically dropping between the branch and its children
				// but the child logically appears as a sibling of the branch *after* both the branch and its children.
				if (this.Children.Count > 0 && this.IsExpanded)
					return DragDropKind.None;
			}
			return base.CanAcceptDrop(dropData, dragDropKind, dragDropPosition);
		}

		public override DragDropKind AcceptDrop(object dropData, DragDropKind dragDropKind, DragDropPosition dragDropPosition)
		{
			if (dragDropPosition == DragDropPosition.Default)
			{
				if (dropData is AbstractActionModelTreeNode)
				{
					AbstractActionModelTreeNode droppedNode = (AbstractActionModelTreeNode) dropData;
					if (dragDropKind == DragDropKind.Move)
					{
						// to drag-move, we can't be dragging onto ourself, onto our parent, or onto one of our descendants
						if (!ReferenceEquals(this, droppedNode)
						    && !ReferenceEquals(this, droppedNode.Parent)
						    && !this.IsDescendantOf(droppedNode as AbstractActionModelTreeBranch))
						{
							if (droppedNode.Parent != null)
								droppedNode.Parent.Children.Remove(droppedNode);
							this.Children.Add(droppedNode);
							return dragDropKind;
						}
					}
				}
			}
			else if (dragDropPosition == DragDropPosition.After)
			{
				// prevent dropping something after an expanded branch node with children,
				// because the user is physically dropping between the branch and its children
				// but the child logically appears as a sibling of the branch *after* both the branch and its children.
				if (this.Children.Count > 0 && this.IsExpanded)
					return DragDropKind.None;
			}
			return base.AcceptDrop(dropData, dragDropKind, dragDropPosition);
		}

		private class Binding : TreeItemBinding<AbstractActionModelTreeNode>
		{
			public Binding()
			{
				this.NodeTextProvider = (node => node.CanonicalLabel);
				this.CanSetNodeTextHandler = (node => false);

				this.CanHaveSubTreeHandler = (node => node is AbstractActionModelTreeBranch);
				this.SubTreeProvider = (node => ((AbstractActionModelTreeBranch) node)._subtree);

				this.IconSetProvider = (node => node.IconSet);
				this.ResourceResolverProvider = (node => node.ResourceResolver);

				this.TooltipTextProvider = (node => node.Tooltip);

				this.IsExpandedGetter = (node => node.IsExpanded);
				this.IsExpandedSetter = ((node, v) => node.IsExpanded = v);

				this.IsCheckedGetter = (node => node.IsChecked);
				this.IsCheckedSetter = ((node, v) => node.IsChecked = v);

				this.IsHighlightedProvider = (node => node.IsHighlighted);
			}

			public override DragDropKind CanAcceptDrop(object item, object dropData, DragDropKind kind, DragDropPosition position)
			{
				AbstractActionModelTreeNode node = item as AbstractActionModelTreeNode;
				if (node == null)
					return DragDropKind.None;
				return node.CanAcceptDrop(dropData, kind, position);
			}

			public override DragDropKind AcceptDrop(object item, object dropData, DragDropKind kind, DragDropPosition position)
			{
				AbstractActionModelTreeNode node = item as AbstractActionModelTreeNode;
				if (node == null)
					return DragDropKind.None;
				return node.AcceptDrop(dropData, kind, position);
			}
		}
	}
}