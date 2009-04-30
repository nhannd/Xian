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
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Local.View.WinForms
{
	public partial class LocalImageExplorerControl : UserControl
	{
		private LocalImageExplorerComponent _component;

		public LocalImageExplorerControl(LocalImageExplorerComponent component)
		{
			_component = component;

			InitializeComponent();

			_fileBrowser.FolderOpened += new EventHandler(OnItemOpened);
			_fileBrowser.FilesOpened += new EventHandler(OnItemOpened);

			//we can use the same context menu for both here.
			_fileBrowser.CustomFileContextMenuDelegate = GetContextMenu;
			_fileBrowser.CustomFolderContextMenuDelegate = GetContextMenu;

			//Tell the component how to get the paths to use.
			component.GetSelectedPathsDelegate = GetSelectedPaths;
		}

		private ContextMenuStrip GetContextMenu()
		{
			if (_contextMenu != null)
				return _contextMenu;

			_contextMenu = new ContextMenuStrip();
			ToolStripBuilder.Clear(_contextMenu.Items);
			ToolStripBuilder.BuildMenu(_contextMenu.Items, _component.ContextMenuModel.ChildNodes);

			return _contextMenu;
		}

		private IEnumerable<string> GetSelectedPaths()
		{
			return _fileBrowser.PathsToSelectedItems;
		}

		private void OnItemOpened(object sender, EventArgs e)
		{
			if (_component.DefaultActionHandler != null)
			{
				_component.DefaultActionHandler();
			}
		}
	}
}
