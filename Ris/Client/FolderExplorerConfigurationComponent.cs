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
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class FolderExplorerConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		/// <summary>
		/// Gets all the <see cref="IConfigurationPage"/>s for this provider.
		/// </summary>
		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

			listPages.Add(new ConfigurationPage<FolderExplorerConfigurationComponent>(SR.TitleFolderExplorer));

			return listPages.AsReadOnly();
		}

		#endregion
	}

	/// <summary>
	/// Extension point for views onto <see cref="FolderExplorerConfigurationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class FolderExplorerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// FolderExplorerConfigurationComponent class.
	/// </summary>
	[AssociateView(typeof(FolderExplorerConfigurationComponentViewExtensionPoint))]
	public class FolderExplorerConfigurationComponent : ConfigurationApplicationComponent
	{
		public class FolderNode
		{
			private const string SEPARATOR = "/";
			private readonly int _lastSeparatorIndex;

			private readonly string _id;
			private readonly string _defaultPath;
			private bool _isVisible;
			private bool _isStale;

			private string _text;
			private bool _isExpanded;
			private FolderNode _parent;
			private Tree<FolderNode> _subTree;

			public FolderNode(string id, string path)
				: this(id, path, true, false)
			{
			}

			public FolderNode(string id, string defaultPath, bool isVisible, bool isStale)
			{
				_id = id;
				_defaultPath = defaultPath;
				_isVisible = isVisible;
				_isStale = isStale;

				_isExpanded = true;
				_lastSeparatorIndex = _defaultPath.LastIndexOf(SEPARATOR);
				_text = _defaultPath.Substring(_lastSeparatorIndex + 1);
			}

			#region Public Data Members

			public string ID
			{
				get { return _id; }
			}

			public string DefaultPath
			{
				get { return _defaultPath; }
			}

			public bool IsVisible
			{
				get { return _isVisible; }
				set { _isVisible = value; }
			}

			public bool IsStale
			{
				get { return _isStale; }
			}

			#endregion

			#region Presentation Model Properties

			/// <summary>
			/// Gets or sets the display text of this node.
			/// </summary>
			public string Text
			{
				get { return _text; }
				set { _text = value; }
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
			/// Gets whether this node can be edited.
			/// </summary>
			public bool CanEdit
			{
				get { return this.Parent != null; }
			}

			/// <summary>
			/// Gets whether this node can be deleted.
			/// </summary>
			public bool CanDelete
			{
				get { return string.IsNullOrEmpty(_id) && (_subTree == null || _subTree.Items.Count == 0); }
			}

			/// <summary>
			/// Gets the previous sibling of this node, null if none.
			/// </summary>
			public FolderNode PreviousSibling
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
			public FolderNode NextSibling
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
			public FolderNode Parent
			{
				get { return _parent; }
				private set { _parent = value; }
			}

			/// <summary>
			/// Gets the subtree of this node, null if none.
			/// </summary>
			public Tree<FolderNode> SubTree
			{
				get { return _subTree; }
			}

			#endregion

			#region Public methods

			public void AddChildNode(FolderNode node)
			{
				BuildSubTree();

				node.Parent = this;
				_subTree.Items.Add(node);

				// expand the tree right away
				this.ExpandSubTree();
			}

			public void MoveUp()
			{
				FolderNode previousSibling = this.PreviousSibling;
				if (_parent == null || previousSibling == null)
					return;

				int index = _parent.SubTree.Items.IndexOf(this);

				// remove and re-insert this node at the prior index
				_parent.SubTree.Items.Remove(previousSibling);
				_parent.SubTree.Items.Insert(index, previousSibling);
			}

			public void MoveDown()
			{
				FolderNode nextSibling = this.NextSibling;
				if (_parent == null || nextSibling == null)
					return;

				// Find index of current node
				int index = _parent.SubTree.Items.IndexOf(this);

				// remove and re-insert this node at the next index
				_parent.SubTree.Items.Remove(nextSibling);
				_parent.SubTree.Items.Insert(index, nextSibling);
			}

			#endregion

			#region Drag & Drop supports

			public DragDropKind CanAcceptDrop(FolderNode dropData, DragDropKind kind)
			{
				if (this == dropData || this == dropData.Parent || this.IsDescendentOf(dropData))
					return DragDropKind.None;

				return DragDropKind.Move;
			}

			public DragDropKind AcceptDrop(FolderNode dropData, DragDropKind kind)
			{
				if (dropData.Parent != null)
					dropData.Parent.SubTree.Items.Remove(dropData);

				this.AddChildNode(dropData);

				return DragDropKind.Move;
			}

			#endregion

			#region Private helpers

			private void BuildSubTree()
			{
				if (_subTree != null)
					return;

				_subTree = BuildTree();
				if (_parent != null)
					_parent.SubTree.Items.NotifyItemUpdated(this);
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
			private bool IsDescendentOf(FolderNode ancestorNode)
			{
				// testNode has no children
				if (ancestorNode.SubTree == null)
					return false;

				bool isDescendentOfAncestorNode = CollectionUtils.Contains(ancestorNode.SubTree.Items,
					delegate(FolderNode childOfTestNode) { return this == childOfTestNode || this.IsDescendentOf(childOfTestNode); });

				return isDescendentOfAncestorNode;
			}

			#endregion

			public static Tree<FolderNode> BuildTree()
			{
				TreeItemBinding<FolderNode> binding = new TreeItemBinding<FolderNode>(
						delegate(FolderNode node) { return node.Text; },
						delegate(FolderNode node) { return node.SubTree; });
				binding.NodeTextSetter = delegate(FolderNode node, string text) { node.Text = text; };
				binding.CanSetNodeTextHandler = delegate(FolderNode node) { return node.CanEdit; };
				binding.CanHaveSubTreeHandler = delegate(FolderNode node) { return node.SubTree != null; };
				binding.IsCheckedGetter = delegate(FolderNode node) { return node.IsVisible; };
				binding.IsCheckedSetter = delegate(FolderNode node, bool isChecked) { node.IsVisible = isChecked; };
				binding.TooltipTextProvider = delegate(FolderNode node) { return node.DefaultPath; };
				binding.IsExpandedGetter = delegate(FolderNode node) { return node.IsExpanded; };
				binding.IsExpandedSetter = delegate(FolderNode node, bool isExpanded) { node.IsExpanded = isExpanded; };
				binding.CanAcceptDropHandler = delegate(FolderNode node, object dropData, DragDropKind kind) { return node.CanAcceptDrop((dropData as FolderNode), kind); };
				binding.AcceptDropHandler = delegate(FolderNode node, object dropData, DragDropKind kind) { return node.AcceptDrop((dropData as FolderNode), kind); };
				return new Tree<FolderNode>(binding);
			}
		}

		private BindingList<IFolderSystem> _folderSystems;
		private int _selectedFolderSystemIndex;
		private SimpleActionModel _folderSystemsActionModel;
		private readonly Tree<FolderNode> _folderTree;
		private FolderNode _selectedFolderNode;
		private SimpleActionModel _foldersActionModel;

		private readonly string _moveFolderSystemUpKey = "MoveFolderSystemUp";
		private readonly string _moveFolderSystemDownKey = "MoveFolderSystemDown";
		private readonly string _addFolderKey = "AddFolder";
		private readonly string _editFolderKey = "EditFolder";
		private readonly string _deleteFolderKey = "DeleteFolder";
		private readonly string _moveFolderUpKey = "MoveFolderUp";
		private readonly string _moveFolderDownKey = "MoveFolderDown";

		private event EventHandler _onEditFolder;

		public FolderExplorerConfigurationComponent()
		{
			_folderTree = FolderNode.BuildTree();
		}

		public override void Start()
		{
			// establish default resource resolver on this assembly (not the assembly of the derived class)
			IResourceResolver resourceResolver = new ResourceResolver(typeof(FolderNode).Assembly);

			_folderSystemsActionModel = new SimpleActionModel(resourceResolver);
			_folderSystemsActionModel.AddAction(_moveFolderSystemUpKey, SR.TitleMoveUp, "Icons.UpToolSmall.png", SR.TitleMoveUp, MoveFolderSystemUp);
			_folderSystemsActionModel.AddAction(_moveFolderSystemDownKey, SR.TitleMoveDown, "Icons.DownToolSmall.png", SR.TitleMoveDown, MoveFolderSystemDown);
			_folderSystemsActionModel[_moveFolderSystemUpKey].Enabled = false;
			_folderSystemsActionModel[_moveFolderSystemDownKey].Enabled = false;

			_foldersActionModel = new SimpleActionModel(resourceResolver);
			_foldersActionModel.AddAction(_addFolderKey, SR.TitleAdd, "Icons.AddToolSmall.png", SR.TitleAdd, AddFolder);
			_foldersActionModel.AddAction(_editFolderKey, SR.TitleEdit, "Icons.EditToolSmall.png", SR.TitleEdit, EditFolder);
			_foldersActionModel.AddAction(_deleteFolderKey, SR.TitleDelete, "Icons.DeleteToolSmall.png", SR.TitleDelete, DeleteFolder);
			_foldersActionModel.AddAction(_moveFolderUpKey, SR.TitleMoveUp, "Icons.UpToolSmall.png", SR.TitleMoveUp, MoveFolderUp);
			_foldersActionModel.AddAction(_moveFolderDownKey, SR.TitleMoveDown, "Icons.DownToolSmall.png", SR.TitleMoveDown, MoveFolderDown);
			_foldersActionModel[_addFolderKey].Enabled = false;
			_foldersActionModel[_editFolderKey].Enabled = false;
			_foldersActionModel[_deleteFolderKey].Enabled = false;
			_foldersActionModel[_moveFolderUpKey].Enabled = false;
			_foldersActionModel[_moveFolderDownKey].Enabled = false;

			LoadFolderSystems();

			base.Start();
		}

		#region IConfigurationApplicationComponent Members

		public override void Save()
		{
			//TODO: save configuration to Xml
			throw new Exception("The method or operation is not implemented.");
		}
		
		#endregion

		#region Folder Systems Presentation Model

		public IBindingList FolderSystems
		{
			get { return _folderSystems; }
		}

		public int SelectedFolderSystemIndex
		{
			get { return _selectedFolderSystemIndex; }
			set
			{
				_selectedFolderSystemIndex = value;
				UpdateFolderSystemActionModel();
				NotifyPropertyChanged("SelectedFolderSystemIndex");

				LoadFolderTree(_folderSystems[_selectedFolderSystemIndex]);
			}
		}

		public void MoveFolderSystem(int index, int newIndex)
		{
			if (index >= 0 && newIndex >= 0 && index != newIndex)
			{
				IFolderSystem fs = _folderSystems[index];
				_folderSystems.RemoveAt(index);
				_folderSystems.Insert(newIndex, fs);
			}
		}

		/// <summary>
		/// Gets the folder systems action model.
		/// </summary>
		public ActionModelNode FolderSystemsActionModel
		{
			get { return _folderSystemsActionModel; }
		}

		#endregion

		#region Folders Presentation Model

		public ITree FolderTree
		{
			get { return _folderTree; }
		}

		public string SelectedFolderNodeText
		{
			get { return _selectedFolderNode.Text; }
			set { _selectedFolderNode.Text = value; }
		}

		public ISelection SelectedFolderNode
		{
			get { return new Selection(_selectedFolderNode); }
			set
			{
				FolderNode node = (FolderNode)value.Item;

				_selectedFolderNode = node;
				UpdateFolderActionModel();
				NotifyPropertyChanged("SelectedFolderNode");
			}
		}

		/// <summary>
		/// Gets the folders action model.
		/// </summary>
		public ActionModelNode FoldersActionModel
		{
			get { return _foldersActionModel; }
		}

		public event EventHandler OnEditFolder
		{
			add { _onEditFolder += value; }
			remove { _onEditFolder -= value; }
		}

		#endregion

		#region Folder Systems Helper

		private void LoadFolderSystems()
		{
			// TODO: Load Folder Systems in the same order as in the FolderExplorer
			_folderSystems = new BindingList<IFolderSystem>(CollectionUtils.Cast<IFolderSystem>(new FolderSystemExtensionPoint().CreateExtensions()));
			this.SelectedFolderSystemIndex = _folderSystems.Count > 0 ? 0 : -1;
		}

		private void MoveFolderSystemUp()
		{
			if (_selectedFolderSystemIndex > 0)
			{
				int newIndex = _selectedFolderSystemIndex - 1;
				MoveFolderSystem(_selectedFolderSystemIndex, newIndex);
				this.SelectedFolderSystemIndex = newIndex;
			}
		}

		private void MoveFolderSystemDown()
		{
			if (_selectedFolderSystemIndex < _folderSystems.Count - 1)
            {
				int newIndex = _selectedFolderSystemIndex + 1;
				MoveFolderSystem(_selectedFolderSystemIndex, newIndex);
				this.SelectedFolderSystemIndex = newIndex;
            }
		}

		private void UpdateFolderSystemActionModel()
		{
			_folderSystemsActionModel[_moveFolderSystemUpKey].Enabled = _selectedFolderSystemIndex > 0;
			_folderSystemsActionModel[_moveFolderSystemDownKey].Enabled = _selectedFolderSystemIndex < _folderSystems.Count - 1;
		}

		#endregion

		#region Folders Helper

		private void LoadFolderTree(IFolderSystem folderSystem)
		{
			_folderTree.Items.Clear();

			// TODO: Load Folder tree in the same order as in the FolderExplorer
			FolderNode rootNode = new FolderNode(folderSystem.Id, folderSystem.Title);
			FolderNode node1 = new FolderNode("node1", "Root/Node1");
			FolderNode node2 = new FolderNode("node2", "Root/Node2");
			FolderNode node21 = new FolderNode("node3", "Root/Node21");

			node2.AddChildNode(node21);
			rootNode.AddChildNode(node1);
			rootNode.AddChildNode(node2);
			_folderTree.Items.Add(rootNode);
		}

		private void AddFolder()
		{
			FolderNode newFolderNode = new FolderNode(null, "New Node");
			_selectedFolderNode.AddChildNode(newFolderNode);

			// Must update action model because the node index may have changed after adding node.
			UpdateFolderActionModel();
		}

		private void EditFolder()
		{
			EventsHelper.Fire(_onEditFolder, this, EventArgs.Empty);
		}

		private void DeleteFolder()
		{
			FolderNode parentNode = _selectedFolderNode.Parent;
			FolderNode previousSibling = _selectedFolderNode.PreviousSibling;
			parentNode.SubTree.Items.Remove(_selectedFolderNode);

			if (previousSibling != null)
				this.SelectedFolderNode = new Selection(previousSibling);
			else
				this.SelectedFolderNode = new Selection(parentNode);
		}

		private void MoveFolderUp()
		{
			_selectedFolderNode.MoveUp();

			// Must update action model because the node index may have changed after moving node.
			UpdateFolderActionModel();
		}

		private void MoveFolderDown()
		{
			_selectedFolderNode.MoveDown();

			// Must update action model because the node index may have changed after moving node.
			UpdateFolderActionModel();
		}
		
		private void UpdateFolderActionModel()
		{
			_foldersActionModel[_addFolderKey].Enabled = _selectedFolderNode != null;
			_foldersActionModel[_editFolderKey].Enabled = _selectedFolderNode != null && _selectedFolderNode.CanEdit;
			_foldersActionModel[_deleteFolderKey].Enabled = _selectedFolderNode != null && _selectedFolderNode.CanDelete;
			_foldersActionModel[_moveFolderUpKey].Enabled = _selectedFolderNode != null && _selectedFolderNode.PreviousSibling != null;
			_foldersActionModel[_moveFolderDownKey].Enabled = _selectedFolderNode != null && _selectedFolderNode.NextSibling != null;
		}

		#endregion
	}
}
