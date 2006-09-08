using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.ImageViewer;
using ClearCanvas.Controls.WinForms.FileBrowser.ShellDll;
using ClearCanvas.Controls.WinForms;
using ClearCanvas.Common;

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
			try
			{
				using (new CursorManager(this, Cursors.WaitCursor))
				{
					ShellItem shellItem = _fileBrowser.SelectedItem;
					_component.Load(shellItem.Path);
				}
			}
			catch (OpenStudyException ex)
			{
				if (ex.StudyCouldNotBeLoaded)
				{
					Platform.ShowMessageBox(ClearCanvas.ImageViewer.SR.ErrorUnableToLoadStudy);
					return;
				}

				if (ex.AtLeastOneImageFailedToLoad)
				{
					Platform.ShowMessageBox(ClearCanvas.ImageViewer.SR.ErrorAtLeastOneImageFailedToLoad);
					return;
				}
			}
		}

	}
}
