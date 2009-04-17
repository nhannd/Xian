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

using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="FolderExplorerConfigurationComponent"/>.
    /// </summary>
    public partial class FolderExplorerConfigurationComponentControl : ApplicationComponentUserControl
    {
        private readonly FolderExplorerConfigurationComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public FolderExplorerConfigurationComponentControl(FolderExplorerConfigurationComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			_folderSystems.DataBindings.Add("SelectedIndex", _component, "SelectedFolderSystemIndex", true, DataSourceUpdateMode.OnPropertyChanged);
			_folderSystems.Format += _folderSystems_Format;
			_folderSystems.ItemDropped += _folderSystems_ItemDropped;
			_folderSystems.MenuModel = _component.FolderSystemsActionModel;
			_folderSystems.ToolbarModel = _component.FolderSystemsActionModel;
			_folderSystems.DataSource = _component.FolderSystems;

			_folders.DataBindings.Add("Selection", _component, "SelectedFolderNode", true, DataSourceUpdateMode.OnPropertyChanged);
			_folders.MenuModel = _component.FoldersActionModel;
			_folders.ToolbarModel = _component.FoldersActionModel;
			_folders.Tree = _component.FolderTree;

        	_component.OnEditFolder += delegate { _folders.EditSelectedNode(); };
		}

		private void _folderSystems_Format(object sender, ListControlConvertEventArgs e)
		{
			e.Value = _component.FormatFolderSystem(e.ListItem);
		}

		private void _folderSystems_ItemDropped(object sender, ListBoxItemDroppedEventArgs e)
		{
			_component.MoveSelectedFolderSystem(e.DraggedIndex, e.DroppedIndex);
		}

		private void _folders_ItemDrag(object sender, ItemDragEventArgs e)
		{
			// allow dragging of nodes
			ISelection selection = (ISelection)e.Item;

			// send the node
			if (selection.Item != null)
				_folders.DoDragDrop(selection.Item, DragDropEffects.All);
		}

	}
}
