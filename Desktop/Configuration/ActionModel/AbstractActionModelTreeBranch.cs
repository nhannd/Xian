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

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	public class AbstractActionModelTreeBranch : AbstractActionModelTreeNode
	{
		private readonly Tree<AbstractActionModelTreeNode> _subtree = new Tree<AbstractActionModelTreeNode>(new Binding());
		private readonly List<AbstractActionModelTreeNode> _syncList = new List<AbstractActionModelTreeNode>();

		public AbstractActionModelTreeBranch(string groupName)
			: this(new PathSegment(groupName, groupName)) {}

		public AbstractActionModelTreeBranch(PathSegment pathSegment)
			: base(pathSegment)
		{
			_subtree.Items.ItemsChanged += OnSubtreeItemsChanged;

			base.IconSet = new IconSet(IconScheme.Colour, "Icons.ActionModelGroupSmall.png", "Icons.ActionModelGroupSmall.png", "Icons.ActionModelGroupSmall.png");
			base.ResourceResolver = new ResourceResolver(this.GetType().Assembly);
		}

		public IList<AbstractActionModelTreeNode> Children
		{
			get { return _subtree.Items; }
		}

		public bool HasNoActions
		{
			get
			{
				foreach (AbstractActionModelTreeNode actionModelTreeNode in this.Children)
				{
					if (actionModelTreeNode is AbstractActionModelTreeLeafAction)
						return false;
					else if (actionModelTreeNode is AbstractActionModelTreeBranch && !((AbstractActionModelTreeBranch) actionModelTreeNode).HasNoActions)
						return false;
				}
				return true;
			}
		}

		protected IEnumerable<AbstractActionModelTreeNode> EnumerateDescendants()
		{
			List<AbstractActionModelTreeNode> list = new List<AbstractActionModelTreeNode>();
			foreach (AbstractActionModelTreeNode actionModelTreeNode in this.Children)
			{
				list.Add(actionModelTreeNode);
				if (actionModelTreeNode is AbstractActionModelTreeBranch)
					list.AddRange(((AbstractActionModelTreeBranch) actionModelTreeNode).EnumerateDescendants());
			}
			return list;
		}

		/// <summary>
		/// Use only for performing initial population of the node.
		/// </summary>
		internal void AppendChild(AbstractActionModelTreeNode child)
		{
			_subtree.Items.Add(child);
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
					actionModelRoot.InsertAction(((AbstractActionModelTreeLeafAction) child).BuildAction());
				}
				else if (child is AbstractActionModelTreeLeafSeparator)
				{
					actionModelRoot.InsertSeparator(((AbstractActionModelTreeLeafSeparator) child).GetSeparatorPath());
				}
			}
		}

		internal virtual bool RequestValidation(AbstractActionModelTreeNode node, string propertyName, object value)
		{
			if (!ReferenceEquals(this.Parent, null))
			{
				return this.Parent.RequestValidation(node, propertyName, value);
			}
			return true;
		}

		internal virtual void NotifyValidated(AbstractActionModelTreeNode node, string propertyName, object value)
		{
			if (!ReferenceEquals(this.Parent, null))
			{
				this.Parent.NotifyValidated(node, propertyName, value);
			}
		}

		private void OnSubtreeItemsChanged(object sender, ItemChangedEventArgs e)
		{
			SynchronizeParent((AbstractActionModelTreeNode) e.Item, e.ChangeType);

			this.CheckState = ComputeCheckState();
		}

		private void SynchronizeParent(AbstractActionModelTreeNode node, ItemChangeType changeType)
		{
			switch (changeType)
			{
				case ItemChangeType.ItemAdded:
				case ItemChangeType.ItemInserted:
					// set the parent on the added node
					node.Parent = this;

					// sync the secondary list
					_syncList.Add(node);

					// force the node to expand
					this.IsExpanded = true;
					break;
				case ItemChangeType.ItemRemoved:
					// nullify the parent on the node if and only if it is us (just in case something silly happens and the parent is already updated)
					if (ReferenceEquals(this, node.Parent))
						node.Parent = null;

					// sync the secondary list
					_syncList.Remove(node);
					break;
				case ItemChangeType.ItemChanged:
					// replacing an item in the list can cause ItemChanged, thus we need to resync the entire list to update the replaced item
				case ItemChangeType.Reset:
				default:
					// resync the parent nodes using the secondary list as a reference for what was in the tree before the change
					foreach (AbstractActionModelTreeNode child in _syncList)
					{
						if (!_subtree.Items.Contains(child))
						{
							if (ReferenceEquals(this, node.Parent))
								node.Parent = null;
						}
					}
					foreach (AbstractActionModelTreeNode child in _subtree.Items)
					{
						if (!_syncList.Contains(child))
							child.Parent = this;
					}

					// update the secondary list
					_syncList.Clear();
					_syncList.AddRange(_subtree.Items);
					break;
			}
		}

		private CheckState ComputeCheckState()
		{
			if (this.Children.Count <= 0)
				return CheckState.Unchecked;

			CheckState checkState = this.Children[0].CheckState;
			if (checkState == CheckState.Partial)
				return checkState;
			foreach (AbstractActionModelTreeNode child in this.Children)
				if (child.CheckState != checkState)
					return CheckState.Partial;
			return checkState;
		}

		protected override void OnCheckStateChanged()
		{
			base.OnCheckStateChanged();

			CheckState checkState = this.CheckState;
			if (checkState == CheckState.Partial)
				return;
			foreach (AbstractActionModelTreeNode child in this.Children)
				child.CheckState = checkState;
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

				this.IsHighlightedProvider = (node => node.IsHighlighted);
			}

			public override CheckState GetCheckState(object item)
			{
				AbstractActionModelTreeNode node = item as AbstractActionModelTreeNode;
				if (node == null)
					return CheckState.Unchecked;
				return node.CheckState;
			}

			public override CheckState ToggleCheckState(object item)
			{
				AbstractActionModelTreeNode node = item as AbstractActionModelTreeNode;
				if (node == null)
					return CheckState.Unchecked;
				if (node is AbstractActionModelTreeLeafSeparator)
					return Trees.CheckState.Checked;
				switch (node.CheckState)
				{
					case CheckState.Checked:
						node.CheckState = CheckState.Unchecked;
						break;
					case CheckState.Partial:
					case CheckState.Unchecked:
					default:
						node.CheckState = CheckState.Checked;
						break;
				}
				return node.CheckState;
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