#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="FolderExplorerComponent"/>
    /// </summary>
    public partial class FolderExplorerComponentControl : CustomUserControl
    {
        private readonly FolderExplorerComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderExplorerComponentControl(FolderExplorerComponent component)
        {
            InitializeComponent();
            _component = component;

            _folderTreeView.Tree = _component.FolderTree;
            _folderTreeView.DataBindings.Add("Selection", _component, "SelectedFolderTreeNode", true, DataSourceUpdateMode.OnPropertyChanged);
            _folderTreeView.MenuModel = _component.FoldersContextMenuModel;

            _component.SelectedFolderChanged += _component_SelectedFolderChanged;
        }

        private void _component_SelectedFolderChanged(object sender, EventArgs e)
        {
			if (_folderTreeView.Selection != _component.SelectedFolderTreeNode)
			{
				_folderTreeView.Selection = _component.SelectedFolderTreeNode;

				// Update action model based on the folder selected
				_folderTreeView.MenuModel = _component.FoldersContextMenuModel;
			}
        }
    }
}
