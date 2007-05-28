using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="WorklistExplorerComponent"/>
    /// </summary>
    public partial class FolderExplorerComponentControl : CustomUserControl
    {
        private FolderExplorerComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderExplorerComponentControl(FolderExplorerComponent component)
        {
            InitializeComponent();
            _component = component;

            _folderTreeView.Tree = _component.FolderTree;
            _folderTreeView.DataBindings.Add("Selection", _component, "SelectedFolder", true, DataSourceUpdateMode.OnPropertyChanged);
            _folderTreeView.MenuModel = _component.FoldersContextMenuModel;
            _folderTreeView.ToolbarModel = _component.FoldersToolbarModel;

            _folderContentsTableView.Table = _component.FolderContentsTable;
            _component.SelectedFolderChanged += new EventHandler(_component_SelectedFolderChanged);
            _component.SuppressSelectionChanged += new EventHandler(_component_SuppressSelectionChanged);

            //_folderContentsTableView.DataBindings.Add("Table", _component, "FolderContentsTable", true, DataSourceUpdateMode.Never);
            _folderContentsTableView.DataBindings.Add("Selection", _component, "SelectedItems", true, DataSourceUpdateMode.OnPropertyChanged);

            _folderContentsTableView.MenuModel = _component.ItemsContextMenuModel;
            _folderContentsTableView.ToolbarModel = _component.ItemsToolbarModel;
        }

        void _component_SuppressSelectionChanged(object sender, EventArgs e)
        {
            ItemEventArgs<bool> ea = (ItemEventArgs<bool>)e;
            _folderContentsTableView.SuppressSelectionChangedEvent = (bool) ea.Item;
        }

        private void _component_SelectedFolderChanged(object sender, EventArgs e)
        {
            _folderTreeView.Selection = _component.SelectedFolder;
            _folderContentsTableView.Table = _component.FolderContentsTable;

            // Update action model based on the folder selected
            _folderTreeView.MenuModel = _component.FoldersContextMenuModel;
            _folderTreeView.ToolbarModel = _component.FoldersToolbarModel;
        }

        /// <summary>
        /// Handle the item drag event from table view, to begin a drag-drop operation when a user
        /// starts dragging items from the table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _folderContentsTableView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            _folderContentsTableView.DoDragDrop(e.Item, DragDropEffects.All);
        }
    }
}
