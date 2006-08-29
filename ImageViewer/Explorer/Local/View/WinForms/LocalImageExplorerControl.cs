using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Controls.WinForms.FileBrowser.ShellDll;
using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Local.View.WinForms
{
	public partial class LocalImageExplorerControl : UserControl
	{

		private LocalImageExplorerComponent _component;

		public LocalImageExplorerControl(LocalImageExplorerComponent component)
		{
			_component = component;
			InitializeComponent();

			_fileBrowser.ItemOpened += new EventHandler(OnItemOpened);
			_fileOpenedItem.Text = "Open";
			_folderOpenedItem.Text = "Open";
		}

		void OnItemOpened(object sender, EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				ShellItem shellItem = _fileBrowser.SelectedItem;
				_component.Load(shellItem.Path);
			}
		}

	}
}
