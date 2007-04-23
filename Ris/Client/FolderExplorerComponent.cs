using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

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
        void AddFolder(IFolder folder);
        void RemoveFolder(IFolder folder);
        IFolder SelectedFolder { get; set; }
        event EventHandler SelectedFolderChanged;

        ISelection SelectedItems { get; }
        event EventHandler SelectedItemsChanged;

        void AddItemActions(IActionSet actions);
        void AddFolderActions(IActionSet actions);
    }

    /// <summary>
    /// Extension point for views onto <see cref="WorklistExplorerComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class FolderExplorerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistExplorerComponent class
    /// </summary>
    [AssociateView(typeof(FolderExplorerComponentViewExtensionPoint))]
    public class FolderExplorerComponent : ApplicationComponent
    {
        #region IFolderExplorerToolContext implementation

        class FolderExplorerToolContext : ToolContext, IFolderExplorerToolContext
        {
            private FolderExplorerComponent _component;

            public FolderExplorerToolContext(FolderExplorerComponent component)
            {
                _component = component;
            }

            #region IFolderExplorerToolContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            public void AddFolder(IFolder folder)
            {
                _component._folderTree.Items.Add(folder);
            }

            public void RemoveFolder(IFolder folder)
            {
                _component._folderTree.Items.Remove(folder);
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

            public void AddItemActions(IActionSet actions)
            {
                _component._itemActions = _component._itemActions.Union(actions);
            }

            public void AddFolderActions(IActionSet actions)
            {
                _component._folderActions = _component._folderActions.Union(actions);
            }

            #endregion
        }

        #endregion

        private IExtensionPoint _folderExplorerToolExtensionPoint;
        private Tree<IFolder> _folderTree;
        private IFolder _selectedFolder;
        private event EventHandler _selectedFolderChanged;
        private event EventHandler _folderIconChanged;

        private ISelection _selectedItems = Selection.Empty;
        private event EventHandler _selectedItemsChanged;

        private ToolSet _tools;

        private IActionSet _itemActions = new ActionSet();
        private IActionSet _folderActions = new ActionSet();

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderExplorerComponent(IExtensionPoint extensionPoint)
        {
            TreeItemBinding<IFolder> binding = new TreeItemBinding<IFolder>();
            binding.NodeTextProvider = delegate(IFolder folder) { return folder.Text; };
            binding.IconSetProvider = delegate(IFolder folder) { return folder.IconSet; };
            binding.ResourceResolverProvider = delegate(IFolder folder) { return folder.ResourceResolver; };
            binding.CanHaveSubTreeHandler = delegate(IFolder folder) { return false; };     // for now, assume only one level of folders
            binding.CanAcceptDropHandler = CanFolderAcceptDrop;
            binding.AcceptDropHandler = FolderAcceptDrop;

            _folderTree = new Tree<IFolder>(binding);
            _folderTree.Items.ItemsChanged += new EventHandler<ItemEventArgs>(RootFoldersChangedEventHandler);
            _folderExplorerToolExtensionPoint = extensionPoint;
        }

        #region Application Component overrides

        public override void Start()
        {
            base.Start();

            _tools = new ToolSet(_folderExplorerToolExtensionPoint, new FolderExplorerToolContext(this));
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

        public event EventHandler SelectedItemsChanged
        {
            add { _selectedItemsChanged += value; }
            remove { _selectedItemsChanged -= value; }
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

        #endregion

        #region Private methods

        private void SelectFolder(IFolder folder)
        {
            if (_selectedFolder != folder)
            {
                if (_selectedFolder != null)
                    _selectedFolder.CloseFolder();

                _selectedFolder = folder;
                if (_selectedFolder != null)
                {
                    try
                    {
                        _selectedFolder.OpenFolder();
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, "Folder refresh failed", this.Host.DesktopWindow);
                    }
                }
                EventsHelper.Fire(_selectedFolderChanged, this, EventArgs.Empty);
            }
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

        private void RootFoldersChangedEventHandler(object sender, ItemEventArgs e)
        {
            if (e.ChangeType == ItemChangeType.ItemAdded)
            {
                IFolder folder = (IFolder)e.Item;
                folder.TextChanged += FolderTextChangedEventHandler;
                folder.IconChanged += FolderIconChangedEventHandler;
            }

            if (e.ChangeType == ItemChangeType.ItemRemoved)
            {
                IFolder folder = (IFolder)e.Item;
                folder.TextChanged -= FolderTextChangedEventHandler;
                folder.IconChanged -= FolderIconChangedEventHandler;
            }
        }

        private void FolderTextChangedEventHandler(object sender, EventArgs e)
        {
            _folderTree.Items.NotifyItemUpdated((IFolder)sender);
        }

        private void FolderIconChangedEventHandler(object sender, EventArgs e)
        {
            _folderTree.Items.NotifyItemUpdated((IFolder)sender);
        }

        #endregion

    }
}
