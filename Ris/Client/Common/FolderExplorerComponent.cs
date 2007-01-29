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
    [MenuAction("show", "global-menus/Folders/Show")]
    [ClickHandler("show", "Show")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class FolderExplorerTool : Tool<IDesktopToolContext>
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
                "Folders",
                null);
        }
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
        private Tree<IFolder> _folderTree;
        private IFolder _selectedFolder;
        private event EventHandler _selectedFolderChanged;

        private ISelection _selectedItems = Selection.Empty;
        private event EventHandler _selectedItemsChanged;



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

        public override void Start()
        {
            // add folders
            /*
            _folderTree.Items.Add(new TestStorageFolders.MyDogsFolder());
            _folderTree.Items.Add(new TestStorageFolders.LostDogsFolder());
            _folderTree.Items.Add(new TestStorageFolders.FoundDogsFolder());
            _folderTree.Items.Add(new TestStorageFolders.CryingCatsFolder());
            _folderTree.Items.Add(new TestStorageFolders.FriendlyCatsFolder());
            _folderTree.Items.Add(new TestStorageFolders.StrayCatsFolder());
             */
            _folderTree.Items.Add(new TestWorkflowFolders.ScheduledItemsFolder());
            _folderTree.Items.Add(new TestWorkflowFolders.InProgressItemsFolder());
            _folderTree.Items.Add(new TestWorkflowFolders.CompletedItemsFolder());
            _folderTree.Items.Add(new TestWorkflowFolders.CancelledItemsFolder());

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

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
                return _selectedFolder == null ? Selection.Empty : new Selection(_selectedFolder);
            }
            set
            {
                IFolder folderToSelect = (IFolder)value.Item;
                if (_selectedFolder != folderToSelect)
                {
                    _selectedFolder = folderToSelect;
                    try
                    {
                        _selectedFolder.Refresh();
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.Report(e, "Folder refresh failed", this.Host.DesktopWindow);
                    }

                    EventsHelper.Fire(_selectedFolderChanged, this, EventArgs.Empty);
                }
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

        #endregion

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

    }
}
