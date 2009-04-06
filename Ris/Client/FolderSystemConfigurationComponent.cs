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
	public class FolderSystemConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		/// <summary>
		/// Gets all the <see cref="IConfigurationPage"/>s for this provider.
		/// </summary>
		public IEnumerable<IConfigurationPage> GetPages()
		{
			List<IConfigurationPage> listPages = new List<IConfigurationPage>();

			listPages.Add(new ConfigurationPage<FolderSystemConfigurationComponent>(SR.TitleFolderSystems));

			return listPages.AsReadOnly();
		}

		#endregion
	}

	/// <summary>
	/// Extension point for views onto <see cref="FolderSystemConfigurationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class FolderSystemConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// FolderSystemConfigurationComponent class.
	/// </summary>
	[AssociateView(typeof(FolderSystemConfigurationComponentViewExtensionPoint))]
	public class FolderSystemConfigurationComponent : ConfigurationApplicationComponent
	{
		public class FolderNode
		{
			private readonly IResourceResolver _resourceResolver;
			private readonly string _id;
			private Path _path;
			private Path _defaultPath;
			private bool _isVisible;
			private bool _isStale;
			private Tree<FolderNode> _subTree;

			public FolderNode(string id, string path, IResourceResolver resourceResolver)
				: this(id, path, path, true, false, resourceResolver)
			{
			}

			public FolderNode(string id, string path, string defaultPath, bool isVisible, bool isStale, IResourceResolver resourceResolver)
			{
				_resourceResolver = resourceResolver;
				_id = id;
				_path = new Path(path, _resourceResolver);
				_defaultPath = new Path(defaultPath, _resourceResolver);
				_isVisible = isVisible;
				_isStale = isStale;
			}

			#region Public Properties

			public string ID
			{
				get { return _id; }
			}

			public string DisplayName
			{
				get { return _path.LastSegment.LocalizedText; }
			}

			public string Path
			{
				get { return _path.ToString(); }
				set { _path = new Path(value, _resourceResolver); }
			}

			public string DefaultPath
			{
				get { return _defaultPath.ToString(); }
				set { _defaultPath = new Path(value, _resourceResolver); }
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

			public Tree<FolderNode> SubTree
			{
				get { return _subTree; }
			}

			#endregion

			public void AddChildNode(FolderNode node)
			{
				if (_subTree == null)
					_subTree = CreateTree();

				_subTree.Items.Add(node);
			}

			public static Tree<FolderNode> CreateTree()
			{
				TreeItemBinding<FolderNode> binding = new TreeItemBinding<FolderNode>(
						delegate(FolderNode node) { return node.DisplayName; },
						delegate(FolderNode node) { return node.SubTree; });

				binding.CanHaveSubTreeHandler = delegate(FolderNode node) { return node.SubTree != null; };

				return new Tree<FolderNode>(binding);
			}
		}

		private readonly IResourceResolver _resourceResolver;

		private BindingList<IFolderSystem> _folderSystems;
		private int _selectedFolderSystemIndex;
		private SimpleActionModel _folderSystemsActionModel;

		private readonly string _moveFolderSystemUpKey = "MoveFolderSystemUp";
		private readonly string _moveFolderSystemDownKey = "MoveFolderSystemDown";

		private readonly Tree<FolderNode> _folderTree;
		private FolderNode _selectedFolderNode;
		private SimpleActionModel _foldersActionModel;
		private readonly string _moveFolderUpKey = "MoveFolderUp";
		private readonly string _moveFolderDownKey = "MoveFolderDown";
		private readonly string _newFolderKey = "NewFolder";

		public FolderSystemConfigurationComponent()
		{
			_folderTree = FolderNode.CreateTree();

			// establish default resource resolver on this assembly (not the assembly of the derived class)
			_resourceResolver = new ResourceResolver(typeof(FolderNode).Assembly);
		}

		public override void Start()
		{
			_folderSystemsActionModel = new SimpleActionModel(_resourceResolver);
			_folderSystemsActionModel.AddAction(_moveFolderSystemUpKey, SR.TitleMoveUp, "Icons.UpToolSmall.png", SR.TitleMoveUp, MoveFolderSystemUp);
			_folderSystemsActionModel.AddAction(_moveFolderSystemDownKey, SR.TitleMoveDown, "Icons.DownToolSmall.png", SR.TitleMoveDown, MoveFolderSystemDown);
			_folderSystemsActionModel[_moveFolderSystemUpKey].Enabled = false;
			_folderSystemsActionModel[_moveFolderSystemDownKey].Enabled = false;

			_foldersActionModel = new SimpleActionModel(_resourceResolver);
			_foldersActionModel.AddAction(_newFolderKey, SR.TitleAddFolder, "Icons.AddToolSmall.png", SR.TitleAddFolder, AddFolderCode);
			_foldersActionModel.AddAction(_moveFolderUpKey, SR.TitleMoveUp, "Icons.UpToolSmall.png", SR.TitleMoveUp, MoveFolderUp);
			_foldersActionModel.AddAction(_moveFolderDownKey, SR.TitleMoveDown, "Icons.DownToolSmall.png", SR.TitleMoveDown, MoveFolderDown);
			_foldersActionModel[_newFolderKey].Enabled = true;
			_foldersActionModel[_moveFolderUpKey].Enabled = true;
			_foldersActionModel[_moveFolderDownKey].Enabled = false;

			_folderSystems = new BindingList<IFolderSystem>(CollectionUtils.Cast<IFolderSystem>(new FolderSystemExtensionPoint().CreateExtensions()));
			this.SelectedFolderSystemIndex = _folderSystems.Count > 0 ? 0 : -1;

			base.Start();
		}

		#region Folder System Presentation Model

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
				NotifyPropertyChanged("SelectedFolderSystemIndex");

				// Update action model enablement
				_folderSystemsActionModel[_moveFolderSystemUpKey].Enabled = _selectedFolderSystemIndex > 0;
				_folderSystemsActionModel[_moveFolderSystemDownKey].Enabled = _selectedFolderSystemIndex < _folderSystems.Count - 1;

				UpdateFolderTree();
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

		public ISelection SelectedFolderNode
		{
			get { return new Selection(_selectedFolderNode); }
			set
			{
				FolderNode node = (FolderNode)value.Item;
				if (!Equals(node, _selectedFolderNode))
				{
					_selectedFolderNode = node;
					NotifyPropertyChanged("SelectedFolderNode");
				}
			}
		}

		/// <summary>
		/// Gets the folders action model.
		/// </summary>
		public ActionModelNode FoldersActionModel
		{
			get { return _foldersActionModel; }
		}

		#endregion

		#region IConfigurationApplicationComponent Members

		public override void Save()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		private void UpdateFolderTree()
		{
			_folderTree.Items.Clear();
			PopulateDummyTreeNode(_folderTree);
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

		private void AddFolderCode()
		{
		}

		private void MoveFolderUp()
		{
		}

		private void MoveFolderDown()
		{
		}

		
		private void PopulateDummyTreeNode(Tree<FolderNode> tree)
		{
			FolderNode rootNode = new FolderNode("Root", "Root", _resourceResolver);
			FolderNode node1 = new FolderNode("node1", "Root/Node1", _resourceResolver);
			FolderNode node2 = new FolderNode("node2", "Root/Node2", _resourceResolver);
			FolderNode node21 = new FolderNode("node3", "Root/Node21", _resourceResolver);

			node2.AddChildNode(node21);
			rootNode.AddChildNode(node1);
			rootNode.AddChildNode(node2);
			tree.Items.Add(rootNode);
		}
	}
}
