#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client
{
    public interface IFolderExplorerToolContext : IToolContext
    {
        IDesktopWindow DesktopWindow { get; }
        void AddFolderSystem(IFolderSystem folderSystem);
        IFolder SelectedFolder { get; set; }
        event EventHandler SelectedFolderChanged;

        ISelection SelectedItems { get; }
        event EventHandler SelectedItemsChanged;
        event EventHandler SelectedItemDoubleClicked;

        void AddItemActions(IActionSet actions);
        void AddFolderActions(IActionSet actions);

        void RegisterSearchDataHandler(ISearchDataHandler handler);
    }

    /// <summary>
    /// Extension point for views onto <see cref="FolderExplorerComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class FolderExplorerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistExplorerComponent class
    /// </summary>
    [AssociateView(typeof(FolderExplorerComponentViewExtensionPoint))]
    public class FolderExplorerComponent : ApplicationComponent, ISearchDataHandler
    {
        #region IFolderExplorerToolContext implementation

        class FolderExplorerToolContext : ToolContext, IFolderExplorerToolContext
        {
            private readonly FolderExplorerComponent _component;

            public FolderExplorerToolContext(FolderExplorerComponent component)
            {
                _component = component;
            }

            #region IFolderExplorerToolContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            public void AddFolderSystem(IFolderSystem folderSystem)
            {
                _component.AddFolderSystem(folderSystem);
            }

            public IFolder SelectedFolder
            {
                get { return _component._selectedFolder; }
                set { _component.SelectFolder(value); }
            }

            public event EventHandler SelectedFolderChanged
            {
                add { _component.SelectedFolderChanged += value; }
                remove { _component.SelectedFolderChanged -= value; }
            }

            public ISelection SelectedItems
            {
                get { return _component.SelectedItems; }
            }

            public event EventHandler SelectedItemsChanged
            {
                add { _component.SelectedItemsChanged += value; }
                remove { _component.SelectedItemsChanged -= value; }
            }

            public event EventHandler SelectedItemDoubleClicked
            {
                add { _component.SelectedItemDoubleClicked += value; }
                remove { _component.SelectedItemDoubleClicked -= value; }
            }

            public void AddItemActions(IActionSet actions)
            {
                _component._itemActions = _component._itemActions.Union(actions);
            }

            public void AddFolderActions(IActionSet actions)
            {
                _component._folderActions = _component._folderActions.Union(actions);
            }

            public void RegisterSearchDataHandler(ISearchDataHandler handler)
            {
                _component.RegisterSearchDataHandler(handler);
            }

            #endregion
        }

        #endregion

        #region Search related

        private ISearchDataHandler _searchDataHandler;

        public void RegisterSearchDataHandler(ISearchDataHandler handler)
        {
            _searchDataHandler = handler;
        }

        public SearchData SearchData
        {
            set
            {
                if (_searchDataHandler != null)
                    _searchDataHandler.SearchData = value;
            }
        }

        #endregion
           
        private IExtensionPoint _folderExplorerToolExtensionPoint;
        private Tree<IFolder> _folderTree;
        private IDictionary<IFolder, ITree> _containers;
        private IFolder _selectedFolder;
        private event EventHandler _selectedFolderChanged;
        private event EventHandler _folderIconChanged;

        private bool _multiSelect;
        private ISelection _selectedItems = Selection.Empty;
        private ISelection _selectedItemsBeforeRefresh = Selection.Empty;
        private event EventHandler _selectedItemDoubleClicked;
        private event EventHandler _selectedItemsChanged;
        private event EventHandler _suppressSelectionChangedEvent;

        private ToolSet _tools;

        private IActionSet _itemActions = new ActionSet();
        private IActionSet _folderActions = new ActionSet();

        private readonly IList<IFolderSystem> _folderSystems;
        private readonly IList<IFolder> _folders;

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderExplorerComponent(IExtensionPoint extensionPoint)
        {
            _containers = new Dictionary<IFolder, ITree>();
            _folderTree = new Tree<IFolder>(GetBinding());
            _folderSystems = new List<IFolderSystem>();
            _folders = new List<IFolder>();

            _folderExplorerToolExtensionPoint = extensionPoint;
        }

        private TreeItemBinding<IFolder> GetBinding()
        {
            TreeItemBinding<IFolder> binding = new TreeItemBinding<IFolder>();

            binding.NodeTextProvider = delegate(IFolder folder) { return folder.Text; };
            binding.IconSetProvider = delegate(IFolder folder) { return folder.IconSet; };
            binding.TooltipTextProvider = delegate(IFolder folder) { return folder.Tooltip; };
            binding.ResourceResolverProvider = delegate(IFolder folder) { return folder.ResourceResolver; };

            binding.CanAcceptDropHandler = CanFolderAcceptDrop;
            binding.AcceptDropHandler = FolderAcceptDrop;

            binding.CanHaveSubTreeHandler = delegate(IFolder folder) { return folder.Subfolders.Count > 0; };
            binding.ShouldInitiallyExpandSubTreeHandler = delegate(IFolder folder) { return folder.StartExpanded; };
            binding.SubTreeProvider =
                delegate(IFolder folder)
                {
                    if (folder.Subfolders.Count > 0)
                    {
                        // Sub trees need to be cached so that delegates assigned to its ItemsChanged event are not orphaned 
                        // on successive GetSubTree calls
                        if (_containers.ContainsKey(folder) == false)
                        {
                            _containers.Add(folder, new Tree<IFolder>(GetBinding(), folder.Subfolders));
                        }
                        return _containers[folder];
                    }
                    else
                    {
                        return null;
                    }
                };

            return binding;
        }

        #region Application Component overrides

        public override void Start()
        {
            base.Start();

            _tools = new ToolSet(_folderExplorerToolExtensionPoint, new FolderExplorerToolContext(this));

            FolderExplorerComponentSettings.Default.BuildAndSynchronize(_folderSystems, InsertFolderUsingPath);

            RefreshCounts(_folderTree);
        }

        private void RefreshCounts(ITree tree)
        {
            if (tree == null) return;

            foreach (IFolder folder in tree.Items)
            {
                RefreshCounts(tree.Binding.GetSubTree(folder));
                folder.RefreshCount();
            }
        }

        public override void Stop()
        {
            _tools.Dispose();

            base.Stop();
        }

        public override IActionSet ExportedActions
        {
            get { return _itemActions.Union(_folderActions); }
        }

        #endregion

        #region Presentation Model

        public ITree FolderTree
        {
            get
            {
                return _folderTree;
            }
        }

        public ISelection SelectedFolder
        {
            get
            {
                return new Selection(_selectedFolder);
            }
            set
            {
                IFolder folderToSelect = (IFolder)value.Item;
                SelectFolder(folderToSelect);
            }
        }

        public ITable FolderContentsTable
        {
            get { return _selectedFolder == null ? null : _selectedFolder.ItemsTable; }
        }

        public event EventHandler SelectedFolderChanged
        {
            add { _selectedFolderChanged += value; }
            remove { _selectedFolderChanged -= value; }
        }

        public bool MultiSelect
        {
            get { return _multiSelect; }
            set { _multiSelect = value; }
        }

        public ISelection SelectedItems
        {
            get
            {
                return _selectedItems;
            }
            set 
            {
                if (!_selectedItems.Equals(value))
                {
                    _selectedItems = value;
                    EventsHelper.Fire(_selectedItemsChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectedItemDoubleClicked
        {
            add { _selectedItemDoubleClicked += value; }
            remove { _selectedItemDoubleClicked -= value; }
        }

        public event EventHandler SelectedItemsChanged
        {
            add { _selectedItemsChanged += value; }
            remove { _selectedItemsChanged -= value; }
        }

        public event EventHandler SuppressSelectionChanged
        {
            add { _suppressSelectionChangedEvent += value; }
            remove { _suppressSelectionChangedEvent -= value; }
        }

        public event EventHandler FolderIconChanged
        {
            add { _folderIconChanged += value; }
            remove { _folderIconChanged -= value; }
        }

        public ActionModelRoot ItemsContextMenuModel
        {
            get
            {
                return ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-items-contextmenu", _itemActions);
            }
        }

        public ActionModelNode ItemsToolbarModel
        {
            get
            {
                return ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-items-toolbar", _itemActions);
            }
        }

        public ActionModelRoot FoldersContextMenuModel
        {
            get
            {
                ActionModelRoot amr = ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-folders-contextmenu", _folderActions);
                if (_selectedFolder != null && _selectedFolder.MenuModel != null)
                    amr.Merge(_selectedFolder.MenuModel);
                return amr;
            }
        }

        public ActionModelNode FoldersToolbarModel
        {
            get
            {
                ActionModelRoot amr = ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-folders-toolbar", _folderActions);
                if (_selectedFolder != null && _selectedFolder.MenuModel != null)
                    amr.Merge(_selectedFolder.MenuModel);
                return amr;
            }
        }

        public void OnSelectedItemDoubleClick()
        {
            EventsHelper.Fire(_selectedItemDoubleClicked, this, EventArgs.Empty);
        }

        #endregion

        #region Private methods

        private void SelectFolder(IFolder folder)
        {
            if (_selectedFolder != folder)
            {
                if (_selectedFolder != null)
                {
                    _selectedFolder.RefreshBegin -= OnSelectedFolderRefreshBegin;
                    _selectedFolder.RefreshFinish -= OnSelectedFolderRefreshFinish;
                    _selectedFolder.CloseFolder();
                }

                _selectedFolder = folder;
                if (_selectedFolder != null)
                {
                    _selectedFolder.RefreshBegin += OnSelectedFolderRefreshBegin;
                    _selectedFolder.RefreshFinish += OnSelectedFolderRefreshFinish;
                    _selectedFolder.OpenFolder();
                }
                EventsHelper.Fire(_selectedFolderChanged, this, EventArgs.Empty);
            }
        }

        void OnSelectedFolderRefreshBegin(object sender, EventArgs e)
        {
            EventsHelper.Fire(_suppressSelectionChangedEvent, this, new ItemEventArgs<bool>(true));

            _selectedItemsBeforeRefresh = _selectedItems;
        }

        void OnSelectedFolderRefreshFinish(object sender, EventArgs e)
        {
            EventsHelper.Fire(_suppressSelectionChangedEvent, this, new ItemEventArgs<bool>(false));

            object sameObjFound = CollectionUtils.SelectFirst<object>(_selectedFolder.ItemsTable.Items,
                delegate(object obj)
                {
                    return obj.Equals(_selectedItemsBeforeRefresh.Item);
                });

            Selection newSelection = Selection.Empty;
            if (sameObjFound != null)
            {
                newSelection = new Selection(sameObjFound);
            }
            else if (_selectedFolder.ItemsTable.Items.Count > 0)
            {
                newSelection = new Selection(_selectedFolder.ItemsTable.Items[0]);
            }

            // Normally we check if _selectedItems is the same as new selection, but we must force a selected items changed event 
            // because the detail of the selection may have changed after a refresh
            _selectedItems = newSelection;
            EventsHelper.Fire(_selectedItemsChanged, this, EventArgs.Empty);
        }

        private DragDropKind CanFolderAcceptDrop(IFolder folder, object dropData, DragDropKind kind)
        {
            if (folder != _selectedFolder && dropData is ISelection)
            {
                return folder.CanAcceptDrop((dropData as ISelection).Items, kind);
            }
            return DragDropKind.None;
        }

        private DragDropKind FolderAcceptDrop(IFolder folder, object dropData, DragDropKind kind)
        {
            if (folder != _selectedFolder && dropData is ISelection)
            {
                // inform the target folder to accept the drop
                DragDropKind result = folder.AcceptDrop((dropData as ISelection).Items, kind);

                // inform the source folder that a drag was completed
                _selectedFolder.DragComplete((dropData as ISelection).Items, result);
            }
            return DragDropKind.None;
        }

        private void AddFolderSystem(IFolderSystem folderSystem)
        {
            if (!_folderSystems.Contains(folderSystem))
                _folderSystems.Add(folderSystem);
        }

        /// <summary>
        /// Insert a folder into the folder system.  This will use the folder.FolderPath property to 
        /// insert the folder into the right structure.  Container folders are created whenever necessary
        /// </summary>
        /// <param name="folder"></param>
        private void InsertFolderUsingPath(IFolder folder)
        {
            int lastIndex = folder.FolderPath.Segments.Length - 1;
            IFolder parentFolder = null;

            for (int index = 0; index < folder.FolderPath.Segments.Length; index++)
            {
                string currentPath = folder.FolderPath.SubPath(index);
                IFolder folderWithCurrentPath = CollectionUtils.SelectFirst(_folders,
                    delegate(IFolder f) { return Equals(currentPath, f.FolderPath.ToString()); });

                if (folderWithCurrentPath != null)
                {
                    if (folderWithCurrentPath is ContainerFolder && index == lastIndex)
                    {
                        ReplaceFolder(folderWithCurrentPath, folder);
                        _folders.Remove(folderWithCurrentPath);
                    }

                    parentFolder = folderWithCurrentPath;
                    continue;
                }

                IFolder currentFolder;
                if (index == lastIndex)
                {
                    currentFolder = folder;
                }
                else
                {
                    currentFolder = new ContainerFolder(currentPath, folder.StartExpanded);
                    _folders.Add(currentFolder);
                }

                if (parentFolder == null)
                    AddFolder(currentFolder);
                else
                    AddFolder(currentFolder, parentFolder);

                parentFolder = currentFolder;
            }

            _folders.Add(folder);
        }

        private void AddFolder(IFolder folder)
        {
            _folderTree.Items.Add(folder);
            folder.TextChanged += FolderChangedEventHandler;
            folder.IconChanged += FolderChangedEventHandler;
        }

        private void RemoveFolder(IFolder folder)
        {
            _folderTree.Items.Remove(folder);
            folder.TextChanged -= FolderChangedEventHandler;
            folder.IconChanged -= FolderChangedEventHandler;
        }

        private void AddFolder(IFolder folder, IFolder parentFolder)
        {
            parentFolder.AddFolder(folder);
            folder.TextChanged += FolderChangedEventHandler;
            folder.IconChanged += FolderChangedEventHandler;
        }

        private void RemoveFolder(IFolder folder, IFolder parentFolder)
        {
            parentFolder.RemoveFolder(folder);
            folder.TextChanged -= FolderChangedEventHandler;
            folder.IconChanged -= FolderChangedEventHandler;
        }

        private void ReplaceFolder(IFolder oldFolder, IFolder newFolder)
        {
            foreach (IFolder subFolder in oldFolder.Subfolders)
            {
                // we don't use this.AddFolder(...) so the eventHandler will stay intact
                newFolder.AddFolder(subFolder);
            }
            
            oldFolder.Subfolders.Clear();

            IFolder parentFolder = CollectionUtils.SelectFirst(_folderTree.Items,
                delegate(IFolder f) { return f.Subfolders.Contains(oldFolder); });

            RemoveFolder(oldFolder, parentFolder);
            AddFolder(newFolder, parentFolder);
        }

        // Tells the item collection holding the specified folder that the folder has been changed
        private void FolderChangedEventHandler(object sender, EventArgs e)
        {
            IFolder folder = (IFolder)sender;
            ITree tree = GetSubtreeContainingFolder(_folderTree, folder);
            if (tree != null)
            {
                ((Tree<IFolder>)tree).Items.NotifyItemUpdated(folder);
            }
        }

        // Recursively finds the correct [sub]tree which holds the specified folder
        private ITree GetSubtreeContainingFolder(ITree tree, IFolder folder)
        {
            if (tree == null) return null;

            if (tree.Items.Contains(folder))
            {
                return tree;
            }
            else
            {
                foreach (IFolder treeFolder in tree.Items)
                {
                    ITree subTree = GetSubtreeContainingFolder(tree.Binding.GetSubTree(treeFolder), folder);
                    if (subTree != null) return subTree;
                }
            }

            return null;
        }

        #endregion
    }
}
