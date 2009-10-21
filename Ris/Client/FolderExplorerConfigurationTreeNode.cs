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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Ris.Client
{
	public abstract class DraggableTreeNode
	{
		public class ContainerNode : DraggableTreeNode
		{
			private string _text;
			private readonly IResourceResolver _resourceResolver;

			public ContainerNode(string text)
				: base(false)
			{
				_text = text;
				_resourceResolver = new ResourceResolver(typeof(ContainerNode).Assembly);
			}

			#region DraggableTreeNode Overrides

			public override string Text
			{
				get { return _text; }
				set
				{
					if (_text == value)
						return;

					_text = value;
					this.Modified = true;
				}
			}

			public override string ToolTip
			{
				get { return _text; }
			}

			public override bool CanEdit
			{
				get { return true; }
			}

			public override bool CanDelete
			{
				get { return _subTree == null || _subTree.Items.Count == 0; }
			}

			public override IconSet IconSet
			{
				get { return new IconSet(IconScheme.Colour, "ContainerFolderOpenSmall.png", "ContainerFolderOpenSmall.png", "ContainerFolderOpenSmall.png"); }
			}

			public override IResourceResolver ResourceResolver
			{
				get { return _resourceResolver; }
			}

			#endregion
		}

		private bool _isChecked;
		private bool _isExpanded;
		private DraggableTreeNode _parent;
		private Tree<DraggableTreeNode> _subTree;

		private bool _modified;
		private bool _modifiedEnabled;
		private event EventHandler _modifiedChanged;

		protected DraggableTreeNode()
			: this(true)
		{
		}

		protected DraggableTreeNode(bool isChecked)
		{
			_isExpanded = true;
			_isChecked = isChecked;
		}

		#region Abstract and Virtual Properties

		/// <summary>
		/// Gets or sets the display text of this node.
		/// </summary>
		public abstract string Text { get; set; }

		/// <summary>
		/// Gets or sets the tooltip of the node.
		/// </summary>
		public abstract string ToolTip { get; }

		/// <summary>
		/// Gets whether this node can be edited.
		/// </summary>
		public abstract bool CanEdit { get; }

		/// <summary>
		/// Gets whether this node can be deleted.
		/// </summary>
		public abstract bool CanDelete { get; }

		/// <summary>
		/// Gets or sets whether the node is checked.
		/// </summary>
		public virtual bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				if (_isChecked == value)
					return;

				SetCheckStateInternal(value);

				if (!this.CheckStateChained)
					return;

				PropagateCheckStateUp(_parent);
				PropagateCheckStateDown(this);
			}
		}

		/// <summary>
		/// Gets or sets whether un/checking will affect check state of parent and children.  Default is true.
		/// </summary>
		/// <remarks>
		/// If true, the parent will be checked if this node is checked.  The descendents will be unchecked if this node is unchecked.
		/// If false, checking or unchecking this node will not affect the check state of parent or children.
		/// </remarks>
		public virtual bool CheckStateChained
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the iconset that should be displayed for the node.
		/// </summary>
		public virtual IconSet IconSet
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the resource resolver that is used to resolve the Icon
		/// </summary>
		public virtual IResourceResolver ResourceResolver
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the path of this node.
		/// </summary>
		public virtual Path Path
		{
			get
			{
				var thisPath = new PathSegment(this.Text);

				if (_parent == null || _parent.Path == null)
					return new Path(thisPath);

				return _parent.Path.Append(thisPath);
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets whether this node has been modified.
		/// </summary>
		public bool Modified
		{
			get { return _modified; }
			protected set
			{
				if (!_modifiedEnabled || value == _modified)
					return;

				_modified = value;

				if (_parent != null)
					_parent.Modified = true;

				EventsHelper.Fire(_modifiedChanged, this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets whether the Modified property is in effect or not.  It should be set to false when initializing the tree, where nodes
		/// are being inserted but the Modified property should remain false.
		/// </summary>
		public bool ModifiedEnabled
		{
			get { return _modifiedEnabled; }
			set
			{
				_modifiedEnabled = value;

				if (_subTree != null)
				{
					CollectionUtils.ForEach(_subTree.Items, child => child.ModifiedEnabled = value);
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="Modified"/> property has changed.
		/// </summary>
		public event EventHandler ModifiedChanged
		{
			add { _modifiedChanged += value; }
			remove { _modifiedChanged -= value; }
		}
		
		/// <summary>
		/// Gets or sets whether the subtree of this node is expanded.
		/// </summary>
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set { _isExpanded = value; }
		}

		/// <summary>
		/// Gets the previous sibling of this node, null if none.
		/// </summary>
		public DraggableTreeNode PreviousSibling
		{
			get
			{
				if (_parent == null)
					return null;

				// Find index of current node
				var index = _parent.SubTree.Items.IndexOf(this);

				// has older sibling
				return index <= 0 ? null : _parent.SubTree.Items[index - 1];
			}
		}

		/// <summary>
		/// Gets the next sibling of this node, null if none.
		/// </summary>
		public DraggableTreeNode NextSibling
		{
			get
			{
				if (_parent == null)
					return null;

				// Find index of current node
				var index = _parent.SubTree.Items.IndexOf(this);

				// has younger sibling
				return index == _parent.SubTree.Items.Count - 1 ? null : _parent.SubTree.Items[index + 1];
			}
		}

		/// <summary>
		/// Gets the parent of this node, null if none.
		/// </summary>
		public DraggableTreeNode Parent
		{
			get { return _parent; }
			private set { _parent = value; }
		}

		/// <summary>
		/// Gets a list of all descendent nodes, by in-order traversal.
		/// </summary>
		public List<DraggableTreeNode> Descendents
		{
			get
			{
				var descendents = new List<DraggableTreeNode>();

				if (_subTree != null)
				{
					CollectionUtils.ForEach(_subTree.Items,
						delegate(DraggableTreeNode child)
						{
							descendents.Add(child);
							descendents.AddRange(child.Descendents);
						});
				}

				return descendents;
				
			}
		}

		/// <summary>
		/// Gets the subtree of this node, null if none.
		/// </summary>
		public Tree<DraggableTreeNode> SubTree
		{
			get { return _subTree; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Clear all the children from this node.
		/// </summary>
		public void ClearSubTree()
		{
			if (_subTree == null)
				return;

			_subTree.Items.Clear();
			_subTree = null;
		}

		/// <summary>
		/// Add a node to the sub tree.
		/// </summary>
		public void AddChildNode(DraggableTreeNode node)
		{
			BuildSubTree();

			node.Parent = this;
			node.ModifiedEnabled = this.ModifiedEnabled;
			_subTree.Items.Add(node);
			this.Modified = true;

			// expand the tree right away
			this.ExpandSubTree();
		}

		/// <summary>
		/// Remove a child node and return the node's next sibling, previous sibling or the parent node.
		/// </summary>
		public DraggableTreeNode RemoveChildNode(DraggableTreeNode node)
		{
			var nextSelectedNode = node.NextSibling ?? node.PreviousSibling ?? this;
			this.SubTree.Items.Remove(node);
			this.Modified = true;
			return nextSelectedNode;
		}

		/// <summary>
		/// Replace a child node with a new node.
		/// </summary>
		public void ReplaceChildNode(DraggableTreeNode oldChildNode, DraggableTreeNode newChildNode)
		{
			// Move the subtree of the old node to the new node.
			foreach (var node in oldChildNode.SubTree.Items)
			{
				newChildNode.AddChildNode(node);
			}
			oldChildNode.ClearSubTree();
			newChildNode.Parent = oldChildNode.Parent;

			// replace the nodes
			var index = this.SubTree.Items.IndexOf(oldChildNode);
			this.SubTree.Items.Insert(index, newChildNode);
			this.SubTree.Items.Remove(oldChildNode);
			this.Modified = true;
		}

		/// <summary>
		/// Insert a node to the proper depth using the path.
		/// </summary>
		public void InsertNode(DraggableTreeNode node, Path path)
		{
			// There is no recommended path.  Add it immediately
			if (path == null || path.Segments.Count == 0)
			{
				AddChildNode(node);
				return;
			}

			var text  = CollectionUtils.FirstElement(path.Segments).LocalizedText;
			var childWithMatchingText = _subTree == null ? null : CollectionUtils.SelectFirst(_subTree.Items, child => child.Text == text);
			
			if (childWithMatchingText == null)
			{
				if (path.Segments.Count == 1)
				{
					// There are no more depth to the path, add child now.
					AddChildNode(node);
					PropagateCheckStateUp(node.Parent);
				}
				else
				{
					// create a container node and insert into the container node's subtree
					var containerNode = new ContainerNode(text);
					AddChildNode(containerNode);
					containerNode.InsertNode(node, path.SubPath(1, path.Segments.Count - 1));
				}
			}
			else
			{
				if (path.Segments.Count == 1)
				{
					// There are no more depth to the path, add child now.
					if (childWithMatchingText is ContainerNode)
						ReplaceChildNode(childWithMatchingText, node);
					else
						AddChildNode(node);

					PropagateCheckStateUp(node.Parent);
				}
				else
				{
					// insert this node child's subtree
					childWithMatchingText.InsertNode(node, path.SubPath(1, path.Segments.Count - 1));
				}
			}
		}

		/// <summary>
		/// Move the node up by swapping with the previous sibling.
		/// </summary>
		public void MoveUp()
		{
			var previousSibling = this.PreviousSibling;
			if (_parent == null || previousSibling == null)
				return;

			var index = _parent.SubTree.Items.IndexOf(this);

			// remove and re-insert this node at the prior index
			_parent.SubTree.Items.Remove(previousSibling);
			_parent.SubTree.Items.Insert(index, previousSibling);

			this.Modified = true;
		}

		/// <summary>
		/// Move the node down by swapping with the next sibling.
		/// </summary>
		public void MoveDown()
		{
			var nextSibling = this.NextSibling;
			if (_parent == null || nextSibling == null)
				return;

			// Find index of current node
			var index = _parent.SubTree.Items.IndexOf(this);

			// remove and re-insert this node at the next index
			_parent.SubTree.Items.Remove(nextSibling);
			_parent.SubTree.Items.Insert(index, nextSibling);

			this.Modified = true;
		}

		#endregion

		#region Drag & Drop supports

		public DragDropKind CanAcceptDrop(DraggableTreeNode dropData, DragDropKind kind)
		{
			if (dropData == null || this == dropData || this == dropData.Parent || this.IsDescendentOf(dropData))
				return DragDropKind.None;

			return DragDropKind.Move;
		}

		public DragDropKind AcceptDrop(DraggableTreeNode dropData, DragDropKind kind)
		{
			if (dropData.Parent != null)
				dropData.Parent.SubTree.Items.Remove(dropData);

			AddChildNode(dropData);
			PropagateCheckStateUp(this);

			this.Modified = true;

			return DragDropKind.Move;
		}

		#endregion

		#region Private helpers

		private void BuildSubTree()
		{
			if (_subTree != null)
				return;

			_subTree = BuildTree();

			ExpandSubTree();
		}

		private void ExpandSubTree()
		{
			this.IsExpanded = true;
			NotifyItemUpdated();
		}

		private void NotifyItemUpdated()
		{
			if (_parent == null)
				return;

			_parent.SubTree.Items.NotifyItemUpdated(this);
		}

		/// <summary>
		/// Determine whether this node is a descendent of the ancestorNode.
		/// </summary>
		/// <param name="ancestorNode">The unknown ancestorNode.</param>
		/// <returns>True if this node is the descendent of the ancestorNode.</returns>
		private bool IsDescendentOf(DraggableTreeNode ancestorNode)
		{
			// testNode has no children
			if (ancestorNode.SubTree == null)
				return false;

			var isDescendentOfAncestorNode = CollectionUtils.Contains(ancestorNode.SubTree.Items,
				childOfTestNode => this == childOfTestNode || this.IsDescendentOf(childOfTestNode));

			return isDescendentOfAncestorNode;
		}

		/// <summary>
		/// Sets the checked state without propagating.
		/// </summary>
		/// <param name="value"></param>
		private void SetCheckStateInternal(bool value)
		{
			if (_isChecked == value)
				return;

			_isChecked = value;
			this.Modified = true;
			NotifyItemUpdated();
		}

		/// <summary>
		/// Propagates check state up to parent.
		/// </summary>
		/// <param name="parent"></param>
		private static void PropagateCheckStateUp(DraggableTreeNode parent)
		{
			if (parent == null)
				return;

			var b = CollectionUtils.Contains(parent.SubTree.Items, n => n.IsChecked);
			parent.SetCheckStateInternal(b);

			PropagateCheckStateUp(parent.Parent);
		}

		/// <summary>
		/// Propagates check state down to children.
		/// </summary>
		/// <param name="parent"></param>
		private static void PropagateCheckStateDown(DraggableTreeNode parent)
		{
			if (parent == null || parent.SubTree == null)
				return;

			foreach (var child in parent.SubTree.Items)
			{
				child.SetCheckStateInternal(parent.IsChecked);
				PropagateCheckStateDown(child);
			}
		}

		#endregion

		public static Tree<DraggableTreeNode> BuildTree()
		{
			var binding = new TreeItemBinding<DraggableTreeNode>(node => node.Text, node => node.SubTree)
				{
					NodeTextSetter = (node, text) => node.Text = text,
					CanSetNodeTextHandler = node => node.CanEdit,
					CanHaveSubTreeHandler = node => node.SubTree != null,
					IsCheckedGetter = node => node.IsChecked,
					IsCheckedSetter = (node, isChecked) => node.IsChecked = isChecked,
					TooltipTextProvider = node => node.ToolTip,
					IsExpandedGetter = node => node.IsExpanded,
					IsExpandedSetter = (node, isExpanded) => node.IsExpanded = isExpanded,
					CanAcceptDropHandler = (node, dropData, kind) => node.CanAcceptDrop((dropData as DraggableTreeNode), kind),
					AcceptDropHandler = (node, dropData, kind) => node.AcceptDrop((dropData as DraggableTreeNode), kind),
					IconSetProvider = node => node.IconSet,
					ResourceResolverProvider = node => node.ResourceResolver
				};
			return new Tree<DraggableTreeNode>(binding);
		}
	}

	public class FolderSystemConfigurationNode : DraggableTreeNode
	{
		private readonly IFolderSystem _folderSystem;
		private readonly bool _readonly;

		public FolderSystemConfigurationNode(IFolderSystem folderSystem, bool isReadonly)
		{
			_folderSystem = folderSystem;
			_readonly = isReadonly;
		}

		public IFolderSystem FolderSystem
		{
			get { return _folderSystem; }
		}

		public bool Readonly
		{
			get { return _readonly; }
		}

		/// <summary>
		/// Gets a list of all folders for this folder system.
		/// </summary>
		public List<IFolder> Folders
		{
			get
			{
				var folders = new List<IFolder>();
				CollectionUtils.ForEach(this.Descendents,
					delegate(DraggableTreeNode node)
						{
							if (node is FolderConfigurationNode)
							{
								var folderNode = (FolderConfigurationNode)node;
								folders.Add(folderNode.Folder);
							}
						});

				return folders;
			}
		}

		/// <summary>
		/// Update IFolder with the current path of the folder node.
		/// </summary>
		public void UpdateFolderPath()
		{
			CollectionUtils.ForEach(this.Descendents,
				delegate(DraggableTreeNode node)
				{
					if (node is FolderConfigurationNode)
					{
						var folderNode = (FolderConfigurationNode) node;
						folderNode.Folder.FolderPath = folderNode.Path;
					}
				});
		}

		#region DraggableTreeNode Overrides

		public override string Text
		{
			get { return _folderSystem.Title; }
			set { }
		}

		public override string ToolTip
		{
			get { return _folderSystem.Title; }
		}

		public override bool CanEdit
		{
			get { return false; }
		}

		public override bool CanDelete
		{
			get { return false; }
		}

		public override bool CheckStateChained
		{
			get { return false; }
		}

		public override Path Path
		{
			get
			{
				// This is overwritten because we don't want folder system name to be part of the path
				return null;
			}
		}

		#endregion
	}

	public class FolderConfigurationNode : DraggableTreeNode
	{
		private readonly IFolder _folder;
		private string _text;
		private readonly IResourceResolver _resourceResolver;

		public FolderConfigurationNode(IFolder folder)
			: base(folder.Visible)
		{
			_folder = folder;
			_text = folder.Name;
			_resourceResolver = new ResourceResolver(typeof(ContainerNode).Assembly);
		}

		public IFolder Folder
		{
			get { return _folder; }
		}

		#region DraggableTreeNode Overrides

		public override string Text
		{
			get { return _text; }
			set
			{
				if (_text == value)
					return;

				_text = value;
				this.Modified = true;
			}
		}

		public override string ToolTip
		{
			get { return _folder.Tooltip; }
		}

		public override bool CanEdit
		{
			get { return true; }
		}

		public override bool CanDelete
		{
			get { return false; }
		}

		public override bool IsChecked
		{
			set
			{
				base.IsChecked = value;
				_folder.Visible = value;
			}
		}

		public override IconSet IconSet
		{
			get { return _folder.IconSet; }
		}

		public override IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
		}

		#endregion
	}
}
