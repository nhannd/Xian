#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="FolderContentsComponent"/>
    /// </summary>
    public partial class FolderContentsComponentControl : CustomUserControl
    {
        private readonly FolderContentsComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderContentsComponentControl(FolderContentsComponent component)
        {
            InitializeComponent();
            _component = component;

            _folderContentsTableView.MultiSelect = _component.MultiSelect;

            _folderContentsTableView.Table = _component.FolderContentsTable;
            _folderContentsTableView.MenuModel = _component.ItemsContextMenuModel;
            _folderContentsTableView.ToolbarModel = _component.ItemsToolbarModel;

            _component.TableChanged += _component_TableChanged;
            _component.FolderSystemChanged += _component_FolderSystemChanged;
            
            _folderContentsTableView.DataBindings.Add("Selection", _component, "SelectedItems", true, DataSourceUpdateMode.OnPropertyChanged);
			_folderContentsTableView.DataBindings.Add("SuppressSelectionChangedEvent", _component, "SuppressFolderContentSelectionChanges",
				true, DataSourceUpdateMode.OnPropertyChanged);

			_statusText.Text = _component.StatusMessage;
			_component.PropertyChanged += _component_PropertyChanged;

//			_folderContentsTableView.DataBindings.Add("StatusText", _component, "StatusMessage", true, DataSourceUpdateMode.Never);
		}

		private void _component_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "StatusMessage")
			{
				_statusText.Text = _component.StatusMessage;
			}

			if(e.PropertyName == "IsUpdating")
			{
				_progressBar.Visible = _component.IsUpdating;
				_progressBar.Style = _component.IsUpdating ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
			}
		}

        private void _component_TableChanged(object sender, EventArgs e)
        {
            _folderContentsTableView.Table = _component.FolderContentsTable;
        }

        private void _component_FolderSystemChanged(object sender, EventArgs e)
        {
            // Must set selection to null before setting tool models
            // This is because the type of item may be different between the old and new folder systems
            _folderContentsTableView.Selection = null;

            _folderContentsTableView.MenuModel = _component.ItemsContextMenuModel;
            _folderContentsTableView.ToolbarModel = _component.ItemsToolbarModel;
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

        private void _folderContentsTableView_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.DoubleClickSelectedItem();
        }
    }
}
