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
