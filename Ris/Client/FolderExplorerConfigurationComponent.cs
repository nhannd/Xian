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
using System.ComponentModel;
using System.Threading;
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
			var listPages = new List<IConfigurationPage>();

			if (Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.HomePage.View) &&
				Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Desktop.FolderOrganization))
			{
				listPages.Add(new ConfigurationPage<FolderExplorerConfigurationComponent>(SR.FolderExplorerConfigurationPagePath));
			}

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
		private BindingList<FolderSystemConfigurationNode> _folderSystems;
		private FolderSystemConfigurationNode _selectedFolderSystemNode;
		private SimpleActionModel _folderSystemsActionModel;
		private readonly Tree<DraggableTreeNode> _folderTree;
		private DraggableTreeNode _selectedFolderNode;
		private SimpleActionModel _foldersActionModel;
		private readonly IList<IFolderSystem> _folderSystemsToReset;

		private const string _moveFolderSystemUpKey = "MoveFolderSystemUp";
		private const string _moveFolderSystemDownKey = "MoveFolderSystemDown";
		private const string _resetFolderSystemKey = "ResetFolderSystem";
		private const string _addFolderKey = "AddFolder";
		private const string _editFolderKey = "EditFolder";
		private const string _deleteFolderKey = "DeleteFolder";
		private const string _moveFolderUpKey = "MoveFolderUp";
		private const string _moveFolderDownKey = "MoveFolderDown";

		private event EventHandler _onEditFolder;

		public FolderExplorerConfigurationComponent()
		{
			_folderTree = DraggableTreeNode.BuildTree();
			_folderSystemsToReset = new List<IFolderSystem>();
		}

		public override void Start()
		{
			// establish default resource resolver on this assembly (not the assembly of the derived class)
			IResourceResolver resourceResolver = new ResourceResolver(typeof(DraggableTreeNode).Assembly);

			_folderSystemsActionModel = new SimpleActionModel(resourceResolver);
			_folderSystemsActionModel.AddAction(_moveFolderSystemUpKey, SR.TitleMoveUp, "Icons.UpToolSmall.png", SR.TitleMoveUp, MoveFolderSystemUp);
			_folderSystemsActionModel.AddAction(_moveFolderSystemDownKey, SR.TitleMoveDown, "Icons.DownToolSmall.png", SR.TitleMoveDown, MoveFolderSystemDown);
			_folderSystemsActionModel.AddAction(_resetFolderSystemKey, SR.TitleReset, "Icons.ResetToolSmall.png", SR.TitleReset, ResetFolderSystem);
			_folderSystemsActionModel[_moveFolderSystemUpKey].Enabled = false;
			_folderSystemsActionModel[_moveFolderSystemDownKey].Enabled = false;
			_folderSystemsActionModel[_resetFolderSystemKey].Enabled = false;

			_foldersActionModel = new SimpleActionModel(resourceResolver);
			_foldersActionModel.AddAction(_addFolderKey, SR.TitleAddContainerFolder, "Icons.AddToolSmall.png", SR.TitleAddContainerFolder, AddFolder);
			var editFolderAction = _foldersActionModel.AddAction(_editFolderKey, SR.TitleRenameFolder, "Icons.EditToolSmall.png", SR.TitleRenameFolder, EditFolder);
			_foldersActionModel.AddAction(_deleteFolderKey, SR.TitleDeleteContainerFolder, "Icons.DeleteToolSmall.png", SR.TitleDeleteContainerFolder, DeleteFolder);
			_foldersActionModel.AddAction(_moveFolderUpKey, SR.TitleMoveUp, "Icons.UpToolSmall.png", SR.TitleMoveUp, MoveFolderUp);
			_foldersActionModel.AddAction(_moveFolderDownKey, SR.TitleMoveDown, "Icons.DownToolSmall.png", SR.TitleMoveDown, MoveFolderDown);
			_foldersActionModel[_addFolderKey].Enabled = false;
			_foldersActionModel[_editFolderKey].Enabled = false;
			_foldersActionModel[_deleteFolderKey].Enabled = false;
			_foldersActionModel[_moveFolderUpKey].Enabled = false;
			_foldersActionModel[_moveFolderDownKey].Enabled = false;

			editFolderAction.KeyStroke = XKeys.F2;

			LoadFolderSystems();

			base.Start();
		}

		#region IConfigurationApplicationComponent Members

		public override void Save()
		{
			FolderExplorerComponentSettings.Default.UpdateUserConfiguration(userConfiguration =>
				{
					foreach(var folderSystem in _folderSystemsToReset)
					{
						userConfiguration.RemoveUserFoldersCustomizations(folderSystem);
					}

					// Save the ordering of the folder systems
					userConfiguration.SaveUserFolderSystemsOrder(
						CollectionUtils.Map<FolderSystemConfigurationNode, IFolderSystem>(
							_folderSystems,
							node => node.FolderSystem));

					// and then save each folder systems' folder customizations
					CollectionUtils.ForEach(_folderSystems, node =>
					{
						if (!node.Modified) return;

						node.UpdateFolderPath();
						userConfiguration.SaveUserFoldersCustomizations(
							node.FolderSystem, node.Folders);
					});
				});
		}

		#endregion

		#region Folder Systems Presentation Model

		public IBindingList FolderSystems
		{
			get { return _folderSystems; }
		}

		public string FormatFolderSystem(object item)
		{
			return ((FolderSystemConfigurationNode)item).Text;
		}

		public int SelectedFolderSystemIndex
		{
			get { return _selectedFolderSystemNode == null ? -1 : _folderSystems.IndexOf(_selectedFolderSystemNode); }
			set
			{
				var previousSelection = _selectedFolderSystemNode;
				_selectedFolderSystemNode = value < 0 ? null : _folderSystems[value];

				UpdateFolderSystemActionModel();
				UpdateFolderActionModel(this.FolderEditorEnabled);

				NotifyPropertyChanged("SelectedFolderSystemIndex");

				BuildFolderTreeIfNotExist(_selectedFolderSystemNode);

				if (previousSelection != _selectedFolderSystemNode)
				{
					_folderTree.Items.Clear();
					_folderTree.Items.Add(_selectedFolderSystemNode);
					this.SelectedFolderNode = new Selection(_selectedFolderSystemNode);
				}
			}
		}

		public bool FolderEditorEnabled
		{
			get { return _selectedFolderSystemNode != null ? !_selectedFolderSystemNode.Readonly : false; }
		}

		public bool CanMoveFolderSystemUp
		{
			get { return this.SelectedFolderSystemIndex > 0; }
		}

		public bool CanMoveFolderSystemDown
		{
			get { return this.SelectedFolderSystemIndex < _folderSystems.Count - 1; }
		}

		public void MoveSelectedFolderSystem(int index, int newIndex)
		{
			// invalid, should never happen.
			if (index < 0)
				return;

			// If a node is dragged to an empty space (not on a node), put it at the end of the list
			if (newIndex < 0)
				newIndex = _folderSystems.Count - 1;

			// Instead of remove/insert node at the source/target index, we opt to move folder system node up and down
			// one at a time.  So the selected node never leave the tree.  The folder tree won't flicker.
			if (newIndex > index)
			{
				while (newIndex > index)
				{
					MoveFolderSystemDown();
					index++;
				}
			}
			else
			{
				while (newIndex < index)
				{
					MoveFolderSystemUp();
					index--;
				}
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
				var node = (DraggableTreeNode)value.Item;

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

		public void OnItemDropped(object droppedItem, DragDropKind kind)
		{
			if (droppedItem is DraggableTreeNode && kind == DragDropKind.Move)
			{
				var droppedNode = (DraggableTreeNode)droppedItem;
				this.SelectedFolderNode = new Selection(droppedNode);
			}
		}

		#endregion

		#region Folder Systems Helper

		private void LoadFolderSystems()
		{
			var folderSystems = FolderExplorerComponentSettings.Default.ApplyFolderSystemsOrder(
				CollectionUtils.Cast<IFolderSystem>(new FolderSystemExtensionPoint().CreateExtensions()));

			var fsNodes = CollectionUtils.Map<IFolderSystem, FolderSystemConfigurationNode>(
				folderSystems,
				fs => new FolderSystemConfigurationNode(fs, FolderExplorerComponentSettings.Default.IsFolderSystemReadOnly(fs)));

			CollectionUtils.ForEach(
				fsNodes, 
				node => node.ModifiedChanged += ((sender, args) => this.Modified = true));

			_folderSystems = new BindingList<FolderSystemConfigurationNode>(fsNodes);

			// Set the initial selected folder system
			if (_folderSystems.Count > 0)
				this.SelectedFolderSystemIndex = 0;
		}

		private void MoveFolderSystemUp()
		{
			if (!this.CanMoveFolderSystemUp)
				return;

			NudgeSelectedFolderSystemUpOrDownOnePosition(false);
		}

		private void MoveFolderSystemDown()
		{
			if (!this.CanMoveFolderSystemDown)
				return;

			NudgeSelectedFolderSystemUpOrDownOnePosition(true);
		}

		private void ResetFolderSystem()
		{
			if (_selectedFolderSystemNode == null)
				return;

			_folderSystemsToReset.Add(_selectedFolderSystemNode.FolderSystem);
			RefreshSelectedFolderSystemFolders(false);
			this.Modified = true;
		}

		private void RefreshSelectedFolderSystemFolders(bool includeUserCustomizations)
		{
			if(_selectedFolderSystemNode == null)
				return;

			_selectedFolderSystemNode.ClearSubTree();
			BuildFolderTreeIfNotExist(_selectedFolderSystemNode, includeUserCustomizations);

			_folderTree.Items.Clear();
			_folderTree.Items.Add(_selectedFolderSystemNode);
			this.SelectedFolderNode = new Selection(_selectedFolderSystemNode);
		}

		private void NudgeSelectedFolderSystemUpOrDownOnePosition(bool up)
		{
			var offset = up ? +1 : -1;

			// We don't want to remove/insert the selected node, but rather the node before, so the folder tree does not flicker
			var index = this.SelectedFolderSystemIndex;
			var fs = _folderSystems[index + offset];
			_folderSystems.Remove(fs);
			_folderSystems.Insert(index, fs);

			this.SelectedFolderSystemIndex = index + offset;
			this.Modified = true;
		}

		private void UpdateFolderSystemActionModel()
		{
			_folderSystemsActionModel[_moveFolderSystemUpKey].Enabled = this.CanMoveFolderSystemUp;
			_folderSystemsActionModel[_moveFolderSystemDownKey].Enabled = this.CanMoveFolderSystemDown;
			_folderSystemsActionModel[_resetFolderSystemKey].Enabled = _selectedFolderSystemNode != null;
		}

		#endregion

		#region Folders Helper

		private void BuildFolderTreeIfNotExist(FolderSystemConfigurationNode folderSystemNode, bool includeUserCustomizations)
		{
			if (folderSystemNode.SubTree != null)
				return;

			// Initialize the list of Folders
			folderSystemNode.FolderSystem.Initialize();

			var folders = FolderExplorerComponentSettings.Default.ApplyFolderCustomizations(folderSystemNode.FolderSystem, includeUserCustomizations);

			// add each ordered folder to the tree
			folderSystemNode.ModifiedEnabled = false;
			folderSystemNode.ClearSubTree();
			CollectionUtils.ForEach(folders, folder => folderSystemNode.InsertNode(new FolderConfigurationNode(folder), folder.FolderPath));

			folderSystemNode.ModifiedEnabled = true;
		}

		private void BuildFolderTreeIfNotExist(FolderSystemConfigurationNode folderSystemNode)
		{
			BuildFolderTreeIfNotExist(folderSystemNode, true);
		}

		private void AddFolder()
		{
			var newFolderNode = new DraggableTreeNode.ContainerNode("New Folder");
			_selectedFolderNode.AddChildNode(newFolderNode);
			this.SelectedFolderNode = new Selection(newFolderNode);
			EventsHelper.Fire(_onEditFolder, this, EventArgs.Empty);
		}

		private void EditFolder()
		{
			EventsHelper.Fire(_onEditFolder, this, EventArgs.Empty);
		}

		private void DeleteFolder()
		{
			var parentNode = _selectedFolderNode.Parent;
			var nextSelectedNode = parentNode.RemoveChildNode(_selectedFolderNode);
			this.SelectedFolderNode = new Selection(nextSelectedNode);
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
			UpdateFolderActionModel(true);
		}

		private void UpdateFolderActionModel(bool canEditFolderSystem)
		{
			var editsEnabled = canEditFolderSystem && _selectedFolderNode != null;

			_foldersActionModel[_addFolderKey].Enabled = editsEnabled;
			_foldersActionModel[_editFolderKey].Enabled = editsEnabled && _selectedFolderNode.CanEdit;
			_foldersActionModel[_deleteFolderKey].Enabled = editsEnabled && _selectedFolderNode.CanDelete;
			_foldersActionModel[_moveFolderUpKey].Enabled = editsEnabled && _selectedFolderNode.PreviousSibling != null;
			_foldersActionModel[_moveFolderDownKey].Enabled = editsEnabled && _selectedFolderNode.NextSibling != null;
		}

		#endregion
	}
}
