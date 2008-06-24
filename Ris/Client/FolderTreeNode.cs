using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Trees;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Represents a node in a folder explorer tree.
	/// </summary>
	internal class FolderTreeNode
	{
		#region ContainerFolder

		/// <summary>
		/// A folder that acts strictly as a parent for other folders, and does not itself contain any items.
		/// </summary>
		internal class ContainerFolder : Folder
		{
			public ContainerFolder(string path, bool startExpanded)
				: base(path, startExpanded)
			{
			}

			protected override bool IsItemCountKnown
			{
				get { return true; }
			}

			public override string Text
			{
				get { return this.FolderPath.LastSegment.LocalizedText; }
			}

			public override void Update()
			{
			}

			public override void Invalidate()
			{
			}

			public override ITable ItemsTable
			{
				get { return null; }
			}

			public override DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
			{
				// can't drop items into a container folder, since it contains only other folders
				return DragDropKind.None;
			}

			public override DragDropKind AcceptDrop(object[] items, DragDropKind kind)
			{
				// can't drop items into a container folder, since it contains only other folders
				return DragDropKind.None;
			}

			protected override IconSet OpenIconSet
			{
				get { return new IconSet(IconScheme.Colour, "ContainerFolderOpenSmall.png", "ContainerFolderOpenMedium.png", "ContainerFolderOpenMedium.png"); }
			}

			protected override IconSet ClosedIconSet
			{
				get { return new IconSet(IconScheme.Colour, "ContainerFolderClosedSmall.png", "ContainerFolderClosedMedium.png", "ContainerFolderClosedMedium.png"); }
			}
		}

		#endregion

		private readonly FolderExplorerComponent _explorer;
		private readonly Tree<FolderTreeNode> _subTree;
		private readonly FolderTreeNode _parent;
		private bool _expanded;
		private IFolder _folder;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="explorer"></param>
		/// <param name="parent"></param>
		/// <param name="path"></param>
		public FolderTreeNode(FolderExplorerComponent explorer, FolderTreeNode parent, Path path)
		{
			_explorer = explorer;
			_parent = parent;
			_subTree = new Tree<FolderTreeNode>(GetBinding(_explorer));

			// always start with container folder
			SetFolder(new ContainerFolder(path.ToString(), true));
		}

		#region Public API

		/// <summary>
		/// Gets the folder at this node.
		/// </summary>
		public IFolder Folder
		{
			get { return _folder; }
		}

		/// <summary>
		/// Gets the subtree at this node.
		/// </summary>
		/// <returns></returns>
		public Tree<FolderTreeNode> GetSubTree()
		{
			return _subTree;
		}

		/// <summary>
		/// Finds a descendant node (not necessarily an immediate child) associated with the specified folder,
		/// or returns null if no such node exists.
		/// </summary>
		/// <param name="folder"></param>
		/// <returns></returns>
		public FolderTreeNode FindNode(IFolder folder)
		{
			if (_folder == folder)
				return this;
			else
			{
				foreach (FolderTreeNode child in _subTree.Items)
				{
					FolderTreeNode node = child.FindNode(folder);
					if (node != null)
						return node;
				}
			}
			return null;
		}

		/// <summary>
		/// Ensures that this node is up to date with respect to the folder count and/or contents.
		/// Applies recursively to all descendants of this node.
		/// </summary>
		public void Update()
		{
			// update this node
			_folder.Update();

			// only update the child nodes if this node is expanded
			if(_expanded)
			{
				foreach (FolderTreeNode child in _subTree.Items)
				{
					child.Update();
				}
			}
		}

		/// <summary>
		/// Invalidates this node and all child nodes.
		/// </summary>
		public bool Invalidate()
		{
			return Invalidate(delegate { return true; });
		}

		/// <summary>
		/// Invalidate this node and any child nodes where the folder is of the specified class (or a subclass of the specified class).
		/// </summary>
		/// <param name="folderClass"></param>
		/// <returns></returns>
		public bool Invalidate(Type folderClass)
		{
			return Invalidate(delegate(FolderTreeNode node) { return folderClass.IsAssignableFrom(node.Folder.GetType()); });
		}

		/// <summary>
		/// Invalidates the descendant node that refers to the specified folder.
		/// </summary>
		/// <param name="folder"></param>
		/// <returns></returns>
		public bool Invalidate(IFolder folder)
		{
			return Invalidate(delegate(FolderTreeNode node) { return folder == node.Folder; });
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Removes the specified node from the subtree of this node, assuming the specified node is
		/// a descendant (not necessarily an immediate child) of this node.  Also removes any empty
		/// parent container nodes of the specified node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		protected bool RemoveNode(FolderTreeNode node)
		{
			if (_subTree.Items.Contains(node))
			{
				// important to null out the folder, to unsubscribe from events, etc. before removing from the collection
				node.SetFolder(null);	
				_subTree.Items.Remove(node);
				return true;
			}
			else
			{
				foreach (FolderTreeNode child in _subTree.Items)
				{
					if (child.RemoveNode(node))
					{
						if (child.IsEmptyContainer())
							RemoveNode(child);
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Inserts the specified folder into the tree, based on its path, recursively creating
		/// container nodes where necessary.
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="depth"></param>
		/// <param name="alphabetical"></param>
		protected void InsertFolder(IFolder folder, int depth, bool alphabetical)
		{
			if(depth == folder.FolderPath.Segments.Length)
			{
				SetFolder(folder);
			}
			else
			{
				PathSegment segment = folder.FolderPath.Segments[depth];

				// find an existing node at this path point
				FolderTreeNode node = CollectionUtils.SelectFirst(_subTree.Items,
											delegate(FolderTreeNode n)
											{
												return Equals(n.Folder.FolderPath.Segments[depth], segment);
											});
				// if not, create the node
				if (node == null)
				{
					node = new FolderTreeNode(_explorer, this, folder.FolderPath.SubPath(depth));
					if(alphabetical)
						InsertChildAlphabetical(node, depth);
					else
						_subTree.Items.Add(node);
				}
				node.InsertFolder(folder, depth + 1, alphabetical);
			}
		}

		protected bool Expanded
		{
			get { return _expanded; }
			set
			{
				_expanded = value;
				if(_expanded)
				{
					Update();
				}
			}
		}

		#endregion

		#region Private Helpers

		/// <summary>
		/// Recursively invalidates nodes matching the specified condition,
		/// returning a value indicating whether any nodes were invalidated.
		/// </summary>
		/// <remarks>
		/// The method recursively invalidates nodes matching the condition, but invalidation
		/// of a child does not imply invalidation of its parent.  All nodes are treated
		/// independently.
		/// </remarks>
		/// <param name="condition"></param>
		/// <returns></returns>
		private bool Invalidate(Predicate<FolderTreeNode> condition)
		{
			bool invalidated = false;

			// invalidate children
			foreach (FolderTreeNode child in _subTree.Items)
			{
				invalidated |= child.Invalidate(condition);
			}

			// invalidate this node if the condition is satisfied
			if (condition(this))
			{
				_folder.Invalidate();

				invalidated = true;
			}
			return invalidated;
		}

		/// <summary>
		/// Sets the folder associatd with this node.
		/// </summary>
		/// <param name="folder"></param>
		private void SetFolder(IFolder folder)
		{
			if(_folder != null)
			{
				_folder.TextChanged -= FolderTextOrIconChangedEventHandler;
				_folder.IconChanged -= FolderTextOrIconChangedEventHandler;
			}

			_folder = folder;

			if (_folder != null)
			{
				_folder.TextChanged += FolderTextOrIconChangedEventHandler;
				_folder.IconChanged += FolderTextOrIconChangedEventHandler;
				_expanded = _folder.StartExpanded;

				// since the folder has changed, need to immediately notify the tree that this item is updated.
				NotifyItemUpdated();
			}
		}

		/// <summary>
		/// Gets a value indicating whether this node is an empty container.
		/// </summary>
		/// <returns></returns>
		private bool IsEmptyContainer()
		{
			return _folder is ContainerFolder && _subTree.Items.Count == 0;
		}

		/// <summary>
		/// Listens for changes from the folder. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FolderTextOrIconChangedEventHandler(object sender, EventArgs e)
		{
			NotifyItemUpdated();
		}

		/// <summary>
		/// Notifies the tree to update the display.
		/// </summary>
		private void NotifyItemUpdated()
		{
			// parent may be null iff this is a root node
			// parent item may not yet contain this node, if this is node has not yet been added to the parent's
			// item collection (a transient state that occurs while the tree is being built)
			if(_parent != null && _parent.GetSubTree().Items.Contains(this))
				_parent.GetSubTree().Items.NotifyItemUpdated(this);
		}

		/// <summary>
		/// Inserts the specified node alphabetically as a child of this node.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="pathDepth"></param>
		private void InsertChildAlphabetical(FolderTreeNode node, int pathDepth)
		{
			PathSegment segment = node.Folder.FolderPath.Segments[pathDepth];

			// find the insertion point - the first node greater/equalto the node to be inserted
			int insertPoint = _subTree.Items.FindIndex(
				delegate(FolderTreeNode n)
				{
					return n.Folder.FolderPath.Segments[pathDepth].LocalizedText.CompareTo(segment.LocalizedText) >= 0;
				});

			if (insertPoint > -1)
				_subTree.Items.Insert(insertPoint, node);
			else
				_subTree.Items.Add(node);
		}

		/// <summary>
		/// Constructs a tree item binding.
		/// </summary>
		/// <param name="explorer"></param>
		/// <returns></returns>
		private static TreeItemBinding<FolderTreeNode> GetBinding(FolderExplorerComponent explorer)
		{
			TreeItemBinding<FolderTreeNode> binding = new TreeItemBinding<FolderTreeNode>();

			binding.NodeTextProvider = delegate(FolderTreeNode node) { return node.Folder.Text; };
			binding.IconSetProvider = delegate(FolderTreeNode node) { return node.Folder.IconSet; };
			binding.TooltipTextProvider = delegate(FolderTreeNode node) { return node.Folder.Tooltip; };
			binding.ResourceResolverProvider = delegate(FolderTreeNode node) { return node.Folder.ResourceResolver; };

			binding.CanAcceptDropHandler = explorer.CanFolderAcceptDrop;
			binding.AcceptDropHandler = explorer.FolderAcceptDrop;

			binding.CanHaveSubTreeHandler = delegate(FolderTreeNode node) { return node.GetSubTree().Items.Count > 0; };
			binding.IsExpandedGetter = delegate(FolderTreeNode node) { return node.Expanded; };
			binding.IsExpandedSetter = delegate(FolderTreeNode node, bool expanded) { node.Expanded = expanded; };
			binding.SubTreeProvider = delegate(FolderTreeNode node) { return node.GetSubTree(); };

			return binding;
		}

		#endregion

	}

	/// <summary>
	/// Represents the root node in a folder explorer tree.
	/// </summary>
	internal class FolderTreeRoot : FolderTreeNode
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="explorer"></param>
		public FolderTreeRoot(FolderExplorerComponent explorer)
			: base(explorer, null, new Path("", null))
		{
		}

		/// <summary>
		/// Inserts the specified folders into the tree.
		/// </summary>
		/// <param name="folders"></param>
		/// <param name="alphabetical"></param>
		public void InsertFolders(IEnumerable<IFolder> folders, bool alphabetical)
		{
			foreach (IFolder folder in folders)
			{
				InsertFolder(folder, 0, alphabetical);
			}
		}


		/// <summary>
		/// Inserts the specified folder into the tree.
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="alphabetical"></param>
		public void InsertFolder(IFolder folder, bool alphabetical)
		{
			InsertFolder(folder, 0, alphabetical);
		}

		/// <summary>
		/// Removes the specified folder from the tree.
		/// </summary>
		/// <param name="folder"></param>
		public void RemoveFolder(IFolder folder)
		{
			FolderTreeNode node = FindNode(folder);
			if (node != null)
			{
				RemoveNode(node);
			}
		}
	}
}
