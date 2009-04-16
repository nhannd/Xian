using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Ris.Client
{
	public abstract class DraggableTreeNode
	{
		private bool _isChecked;
		private bool _isExpanded;
		private DraggableTreeNode _parent;
		private Tree<DraggableTreeNode> _subTree;

		private bool _modified;
		private event EventHandler _modifiedChanged;

		public DraggableTreeNode()
		{
			_isExpanded = true;
			_isChecked = true;
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
				if (_isChecked != value)
				{
					_isChecked = value;
					this.Modified = true;
				}
			}
		}

		/// <summary>
		/// Gets the path of this node.
		/// </summary>
		public virtual Path Path
		{
			get
			{
				Path thisPath = new Path(this.Text);

				if (_parent == null || _parent.Path == null)
					return thisPath;
				else
					return _parent.Path.Append(thisPath);
			}
		}

		#endregion

		#region Public Properties

		public bool Modified
		{
			get { return _modified; }
			protected set
			{
				if (value != _modified)
				{
					_modified = value;

					if (_parent != null)
						_parent.Modified = true;

					EventsHelper.Fire(_modifiedChanged, this, EventArgs.Empty);
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
				int index = _parent.SubTree.Items.IndexOf(this);

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
				int index = _parent.SubTree.Items.IndexOf(this);

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
		/// Gets a list of all descendent nodes, starting with the immediate children.
		/// </summary>
		public List<DraggableTreeNode> Descendents
		{
			get
			{
				List<DraggableTreeNode> descendents = new List<DraggableTreeNode>();

				if (_subTree != null)
				{
					descendents.AddRange(_subTree.Items);

					CollectionUtils.ForEach(_subTree.Items,
						delegate(DraggableTreeNode child)
						{
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
			_subTree.Items.Add(node);

			// expand the tree right away
			this.ExpandSubTree();
		}

		/// <summary>
		/// Insert a node to the proper depth using the path.
		/// </summary>
		public void InsertNode(DraggableTreeNode node, Path path)
		{
			// This node is the first child, or there is no recommended path.  Add it immediately
			if (_subTree == null || _subTree.Items.Count == 0 || path == null || path.Segments.Count == 0)
			{
				AddChildNode(node);
				return;
			}

			PathSegment firstSegment = CollectionUtils.FirstElement(path.Segments);
			DraggableTreeNode childWithMatchingText = CollectionUtils.SelectFirst(_subTree.Items,
				delegate(DraggableTreeNode child) { return child.Text == firstSegment.LocalizedText; });
			
			if (childWithMatchingText == null)
			{
				AddChildNode(node);
			}
			else
			{
				// insert this node child's subtree
				childWithMatchingText.InsertNode(node, path.SubPath(1, path.Segments.Count - 1));
			}
		}

		/// <summary>
		/// Move the node up by swapping with the previous sibling.
		/// </summary>
		public void MoveUp()
		{
			DraggableTreeNode previousSibling = this.PreviousSibling;
			if (_parent == null || previousSibling == null)
				return;

			int index = _parent.SubTree.Items.IndexOf(this);

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
			DraggableTreeNode nextSibling = this.NextSibling;
			if (_parent == null || nextSibling == null)
				return;

			// Find index of current node
			int index = _parent.SubTree.Items.IndexOf(this);

			// remove and re-insert this node at the next index
			_parent.SubTree.Items.Remove(nextSibling);
			_parent.SubTree.Items.Insert(index, nextSibling);

			this.Modified = true;
		}

		#endregion

		#region Drag & Drop supports

		public DragDropKind CanAcceptDrop(DraggableTreeNode dropData, DragDropKind kind)
		{
			if (this == dropData || this == dropData.Parent || this.IsDescendentOf(dropData))
				return DragDropKind.None;

			return DragDropKind.Move;
		}

		public DragDropKind AcceptDrop(DraggableTreeNode dropData, DragDropKind kind)
		{
			if (dropData.Parent != null)
				dropData.Parent.SubTree.Items.Remove(dropData);

			this.AddChildNode(dropData);

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
			if (_parent == null)
				return;

			this.IsExpanded = true;
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

			bool isDescendentOfAncestorNode = CollectionUtils.Contains(ancestorNode.SubTree.Items,
				delegate(DraggableTreeNode childOfTestNode) { return this == childOfTestNode || this.IsDescendentOf(childOfTestNode); });

			return isDescendentOfAncestorNode;
		}

		#endregion

		public static Tree<DraggableTreeNode> BuildTree()
		{
			TreeItemBinding<DraggableTreeNode> binding = new TreeItemBinding<DraggableTreeNode>(
					delegate(DraggableTreeNode node) { return node.Text; },
					delegate(DraggableTreeNode node) { return node.SubTree; });
			binding.NodeTextSetter = delegate(DraggableTreeNode node, string text) { node.Text = text; };
			binding.CanSetNodeTextHandler = delegate(DraggableTreeNode node) { return node.CanEdit; };
			binding.CanHaveSubTreeHandler = delegate(DraggableTreeNode node) { return node.SubTree != null; };
			binding.IsCheckedGetter = delegate(DraggableTreeNode node) { return node.IsChecked; };
			binding.IsCheckedSetter = delegate(DraggableTreeNode node, bool isChecked) { node.IsChecked = isChecked; };
			binding.TooltipTextProvider = delegate(DraggableTreeNode node) { return node.ToolTip; };
			binding.IsExpandedGetter = delegate(DraggableTreeNode node) { return node.IsExpanded; };
			binding.IsExpandedSetter = delegate(DraggableTreeNode node, bool isExpanded) { node.IsExpanded = isExpanded; };
			binding.CanAcceptDropHandler = delegate(DraggableTreeNode node, object dropData, DragDropKind kind) { return node.CanAcceptDrop((dropData as DraggableTreeNode), kind); };
			binding.AcceptDropHandler = delegate(DraggableTreeNode node, object dropData, DragDropKind kind) { return node.AcceptDrop((dropData as DraggableTreeNode), kind); };
			return new Tree<DraggableTreeNode>(binding);
		}
	}

	public class FolderSystemConfigurationNode : DraggableTreeNode
	{
		private readonly IFolderSystem _folderSystem;

		public FolderSystemConfigurationNode(IFolderSystem folderSystem)
		{
			_folderSystem = folderSystem;
		}

		public IFolderSystem FolderSystem
		{
			get { return _folderSystem; }
		}

		/// <summary>
		/// Gets a list of all folders for this folder system.
		/// </summary>
		public List<IFolder> Folders
		{
			get
			{
				return CollectionUtils.Map<DraggableTreeNode, IFolder>(this.Descendents,
					delegate(DraggableTreeNode node)
						{
							FolderConfigurationNode folderNode = (FolderConfigurationNode) node;
							return folderNode.Folder;
						});
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
					FolderConfigurationNode folderNode = (FolderConfigurationNode)node;
					folderNode.Folder.FolderPath = folderNode.Path;
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
			get { return _folderSystem.Id; }
		}

		public override bool CanEdit
		{
			get { return false; }
		}

		public override bool CanDelete
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

		/// <summary>
		/// Constructor for a new node not associated with an IFolder
		/// </summary>
		public FolderConfigurationNode(string text)
		{
			_text = text;
		}
		
		/// <summary>
		/// Constructor for a node that is associated with an IFolder.
		/// </summary>
		/// <param name="folder"></param>
		public FolderConfigurationNode(IFolder folder)
			: this(folder.Text)
		{
			_folder = folder;

			// TODO: visibility of IFolder
			//this.IsChecked = _folder.IsVisible;
			this.IsChecked = true;
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
				if (_text != value)
				{
					_text = value;
					this.Modified = true;
				}
			}
		}

		public override string ToolTip
		{
			get { return _folder == null ? null : _folder.Tooltip; }
		}

		public override bool CanEdit
		{
			get { return this.Parent != null; }
		}

		public override bool CanDelete
		{
			get { return _folder == null && (this.SubTree == null || this.SubTree.Items.Count == 0); }
		}

		public override bool IsChecked
		{
			set
			{
				base.IsChecked = value;

				// TODO: enable folder visiblility
				//if (_folder != null)
				//    _folder.Visible = value;
			}
		}

		#endregion
	}
}
