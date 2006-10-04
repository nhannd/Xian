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

			_fileBrowser.FolderOpened += new EventHandler(OnFolderViewItemOpened);
			_fileBrowser.FilesOpened += new EventHandler(OnFileViewItemOpened);

			_fileOpenedItem.Text = "Open";
			_folderOpenedItem.Text = "Open";
		}

		private void OnFolderViewItemOpened(object sender, EventArgs e)
		{
			ShellItem item = _fileBrowser.SelectedItem;
			if (item == null)
				return;

			string path = ShellItem.GetFullPath(item);

			OpenItems(new string[] { path });
		}

		private void OnFileViewItemOpened(object sender, EventArgs e)
		{
			IEnumerable<string> paths = _fileBrowser.PathsToSelectedItems;
			if (paths == null)
				return;

			OpenItems(paths);
		}

		private void OpenItems(IEnumerable<string> paths)
		{
			try
			{
				using (new CursorManager(this, Cursors.WaitCursor))
				{
					_component.Load(paths);
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
			catch (Exception ex)
			{
				// Just in case.  It's unlikely, but we could also catch:
				//    - DirectoryNotFoundException
				//    - ArgumentNullException
				//    - ArgumentException
				Platform.ShowMessageBox(ex.Message);
			}
		}
	}
}
