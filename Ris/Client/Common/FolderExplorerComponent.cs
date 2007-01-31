using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Common
{
    [MenuAction("show", "global-menus/Go/Technologist Home")]
    [ClickHandler("show", "Show")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class LaunchFolderExplorerTool : Tool<IDesktopToolContext>
    {
        public void Show()
        {
            FolderExplorerComponent folderComponent = new FolderExplorerComponent();
            AcquisitionWorkflowPreviewComponent previewComponent = new AcquisitionWorkflowPreviewComponent();

            folderComponent.SelectedItemsChanged += delegate(object sender, EventArgs args)
            {
                ModalityWorklistQueryResult item = folderComponent.SelectedItems.Item as ModalityWorklistQueryResult;
                previewComponent.WorklistItem = item;
            };

            SplitComponentContainer split = new SplitComponentContainer(
                new SplitPane("Folders", folderComponent, 1.0f),
                new SplitPane("Preview", previewComponent, 1.0f),
                SplitOrientation.Vertical);

            ApplicationComponent.LaunchAsWorkspace(
                this.Context.DesktopWindow,
                split,
                "Technologist Home",
                null);
        }
    }


    public interface IFolderExplorerToolContext : IToolContext
    {
        IDesktopWindow DesktopWindow { get; }
        void AddFolder(IFolder folder);
        void RemoveFolder(IFolder folder);
        IFolder SelectedFolder { get; set; }
        event EventHandler SelectedFolderChanged;

        ISelection SelectedItems { get; }
        event EventHandler SelectedItemsChanged;

        void AddActions(IActionSet actions);
    }

    [ExtensionPoint]
    public class FolderExplorerToolExtensionPoint : ExtensionPoint<ITool>
    {
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

            public void AddActions(IActionSet actions)
            {
                _component._itemActions = _component._itemActions.Union(actions);
            }

            #endregion
        }

        #endregion


        private Tree<IFolder> _folderTree;
        private IFolder _selectedFolder;
        private event EventHandler _selectedFolderChanged;

        private ISelection _selectedItems = Selection.Empty;
        private event EventHandler _selectedItemsChanged;

        private ToolSet _tools;

        private IActionSet _itemActions = new ActionSet();

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderExplorerComponent()
        {
            TreeItemBinding<IFolder> binding = new TreeItemBinding<IFolder>();
            binding.NodeTextProvider = delegate(IFolder folder) { return folder.Text; };
            binding.CanHaveSubTreeHandler = delegate(IFolder folder) { return false; };     // for now, assume only one level of folders
            binding.CanAcceptDropHandler = CanFolderAcceptDrop;
            binding.AcceptDropHandler = FolderAcceptDrop;

            _folderTree = new Tree<IFolder>(binding);
            _folderTree.Items.ItemsChanged += new EventHandler<ItemEventArgs>(RootFoldersChangedEventHandler);
        }

        #region Application Component overrides

        public override void Start()
        {
            base.Start();

            _tools = new ToolSet(new FolderExplorerToolExtensionPoint(), new FolderExplorerToolContext(this));
        }

        public override void Stop()
        {
            _tools.Dispose();

            base.Stop();
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

        public ActionModelRoot ItemsContextMenuModel
        {
            get
            {
                return ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-items-contextmenu", _itemActions);
            }
        }

        #endregion

        #region Private methods

        private void SelectFolder(IFolder folder)
        {
            if (_selectedFolder != folder)
            {
                _selectedFolder = folder;
                if (_selectedFolder != null)
                {
                    try
                    {
                        _selectedFolder.Refresh();
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
            }

            if (e.ChangeType == ItemChangeType.ItemRemoved)
            {
                IFolder folder = (IFolder)e.Item;
                folder.TextChanged -= FolderTextChangedEventHandler;
            }
        }

        private void FolderTextChangedEventHandler(object sender, EventArgs e)
        {
            _folderTree.Items.NotifyItemUpdated((IFolder)sender);
        }

        #endregion

    }
}
