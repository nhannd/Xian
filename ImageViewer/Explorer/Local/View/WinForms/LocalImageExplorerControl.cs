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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Controls.WinForms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Local.View.WinForms
{
	public partial class LocalImageExplorerControl : UserControl
	{
		private LocalImageExplorerComponent _component;
		private bool _lastClickOnFolderView = false;

		public LocalImageExplorerControl(LocalImageExplorerComponent component)
		{
			_component = component;

			InitializeComponent();
			InitializeHistoryMenu();
			InitializeIcons();

			_folderView.ExceptionRaised += delegate(object sender, ItemEventArgs<Exception> e) { this.ReportException(e.Item); };
			_folderView.ExceptionRaised += delegate(object sender, ItemEventArgs<Exception> e) { this.ReportException(e.Item); };

			//Tell the component how to get the paths to use.
			component.GetSelectedPathsDelegate = GetSelectedPaths;
		}

		private void ReportException(Exception ex)
		{
			if (ex is PathNotFoundException)
			{
				ExceptionHandler.Report(ex, string.Format(SR.ErrorPathUnavailable, ((PathNotFoundException) ex).Path), _component.DesktopWindow);
			}
			else
			{
				ExceptionHandler.Report(ex, ex.Message, _component.DesktopWindow);
			}
		}

		private IEnumerable<string> GetSelectedPaths()
		{
			if (_lastClickOnFolderView)
			{
				foreach (FolderView.FolderViewItem item in _folderView.SelectedItems)
				{
					if (!string.IsNullOrEmpty(item.Path))
						yield return item.Path;
				}
			}
			else
			{
				yield return _folderTree.SelectedItem.Path;
			}
		}

		private void OnItemOpened(object sender, EventArgs e)
		{
			if (_component.DefaultActionHandler != null)
			{
				_component.DefaultActionHandler();
			}
		}

		#region Explorer Control

		private const int ShowHistoryCount = 10;
		private string _lastValidLocation = string.Empty;

		private static void InitializeImageList(ImageList imageList, string sizeString)
		{
			Type type = typeof (LocalImageExplorerControl);

			string[] icons = {"Back", "Next", "Up", "Refresh", "Home", "ShowFolders", "View", "Go"};
			foreach (string iconName in icons)
			{
				using (Stream ioStream = type.Assembly.GetManifestResourceStream(string.Format("{0}.Icons.{1}Tool{2}.png", type.Namespace, iconName, sizeString)))
				{
					if (ioStream == null)
						continue;
					imageList.Images.Add(iconName, Image.FromStream(ioStream));
				}
			}
		}

		private void InitializeIcons()
		{
			InitializeImageList(_largeIconImageList, "Large");
			InitializeImageList(_mediumIconImageList, "Medium");
			InitializeImageList(_smallIconImageList, "Small");

			_toolStrip.ImageList = _mediumIconImageList;
			_btnBack.ImageKey = "Back";
			_btnForward.ImageKey = "Next";
			_btnUp.ImageKey = "Up";
			_btnRefresh.ImageKey = "Refresh";
			_btnHome.ImageKey = "Home";
			_btnShowFolders.ImageKey = "ShowFolders";
			_btnViews.ImageKey = "View";

			_addressStrip.ImageList = _smallIconImageList;
			_btnGo.ImageKey = "Go";
		}

		private void InitializeHistoryMenu()
		{
			for (int n = 0; n < ShowHistoryCount; n++)
			{
				ToolStripMenuItem menuBack = new ToolStripMenuItem();
				menuBack.Click += _mnuHistoryItem_Click;
				menuBack.Tag = -(n + 1);
				menuBack.Visible = false;
				_btnBack.DropDownItems.Add(menuBack);

				ToolStripMenuItem menuForward = new ToolStripMenuItem();
				menuForward.Click += _mnuHistoryItem_Click;
				menuForward.Tag = n + 1;
				menuForward.Visible = false;
				_btnForward.DropDownItems.Add(menuForward);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			_folderCoordinator.BrowseToHome();
			base.OnLoad(e);
		}

		private void _addressStrip_SizeChanged(object sender, EventArgs e)
		{
			_txtAddress.Size = new Size(
				_toolStripContainer.Width - _txtAddress.Margin.Horizontal
				- (_btnGo.Width + _btnGo.Margin.Horizontal)
				- (_lblAddress.Width + _lblAddress.Margin.Horizontal)
				- _addressStrip.GripRectangle.Width - 10
				, _txtAddress.Height);
		}

		private void _folderCoordinator_CurrentPidlChanged(object sender, EventArgs e)
		{
			_btnUp.Enabled = _folderCoordinator.CanBrowseToParent;
			_btnBack.Enabled = _folderCoordinator.CanBrowseToPrevious;
			_btnForward.Enabled = _folderCoordinator.CanBrowseToNext;
			_lastValidLocation = _txtAddress.Text = _folderCoordinator.CurrentPath;
			this.Text = _folderCoordinator.CurrentDisplayName;
			this.UpdateBackButtonMenu();
			this.UpdateForwardButtonMenu();
		}

		private void UpdateBackButtonMenu()
		{
			int count = 0;
			foreach (Pidl pastPidl in _folderCoordinator.EnumeratePreviousLocations(false))
			{
				if (count >= ShowHistoryCount)
					break;
				_btnBack.DropDownItems[count].Text = pastPidl.DisplayName;
				_btnBack.DropDownItems[count].Visible = true;
				count++;
			}
			for (int n = count; n < ShowHistoryCount; n++)
				_btnBack.DropDownItems[n].Visible = false;
		}

		private void UpdateForwardButtonMenu()
		{
			int count = 0;
			foreach (Pidl futurePidl in _folderCoordinator.EnumerateNextLocations(false))
			{
				if (count >= ShowHistoryCount)
					break;
				_btnForward.DropDownItems[count].Text = futurePidl.DisplayName;
				_btnForward.DropDownItems[count].Visible = true;
				count++;
			}
			for (int n = count; n < ShowHistoryCount; n++)
				_btnForward.DropDownItems[n].Visible = false;
		}

		private void _btnUp_Click(object sender, EventArgs e)
		{
			_folderCoordinator.BrowseToParent();
		}

		private void _btnBack_Click(object sender, EventArgs e)
		{
			_folderCoordinator.BrowseToPrevious();
		}

		private void _btnForward_Click(object sender, EventArgs e)
		{
			_folderCoordinator.BrowseToNext();
		}

		private void _btnHome_Click(object sender, EventArgs e)
		{
			_folderCoordinator.BrowseToHome();
		}

		private void _btnRefresh_Click(object sender, EventArgs e)
		{
			_folderCoordinator.Refresh();
		}

		private void _btnGo_Click(object sender, EventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(_txtAddress.Text))
					_folderCoordinator.BrowseTo(_txtAddress.Text);
			}
			catch (Exception ex)
			{
				this.ReportException(ex);
				_txtAddress.Text = _lastValidLocation;
			}
		}

		private void _txtAddress_KeyEnterPressed(object sender, EventArgs e)
		{
			_btnGo.PerformClick();
		}

		private void _mnuHistoryItem_Click(object sender, EventArgs e)
		{
			_folderCoordinator.BrowseTo((int) ((ToolStripMenuItem) sender).Tag);
		}

		private void _btnShowFolders_Click(object sender, EventArgs e)
		{
			_btnShowFolders.Checked = !_btnShowFolders.Checked;
			_splitPane.Panel1Collapsed = !_btnShowFolders.Checked;
		}

		private void _mnuTilesView_Click(object sender, EventArgs e)
		{
			_folderView.View = System.Windows.Forms.View.Tile;
			_mnuIconsView.Checked = _mnuListView.Checked = _mnuDetailsView.Checked = false;
			_mnuTilesView.Checked = true;
		}

		private void _mnuIconsView_Click(object sender, EventArgs e)
		{
			_folderView.View = System.Windows.Forms.View.LargeIcon;
			_mnuTilesView.Checked = _mnuListView.Checked = _mnuDetailsView.Checked = false;
			_mnuIconsView.Checked = true;
		}

		private void _mnuListView_Click(object sender, EventArgs e)
		{
			_folderView.View = System.Windows.Forms.View.List;
			_mnuTilesView.Checked = _mnuIconsView.Checked = _mnuDetailsView.Checked = false;
			_mnuListView.Checked = true;
		}

		private void _mnuDetailsView_Click(object sender, EventArgs e)
		{
			_folderView.View = System.Windows.Forms.View.Details;
			_mnuTilesView.Checked = _mnuIconsView.Checked = _mnuListView.Checked = false;
			_mnuDetailsView.Checked = true;
		}

		private void _folderView_ItemDoubleClick(object sender, FolderViewItemEventArgs e)
		{
			if (!e.Item.IsFolder)
			{
				this.OnItemOpened(sender, e);
			}
		}

		private void _folderViewContextMenu_Opening(object sender, CancelEventArgs e)
		{
			_lastClickOnFolderView = true;
			ToolStripBuilder.Clear(_folderViewContextMenu.Items);
			ToolStripBuilder.BuildMenu(_folderViewContextMenu.Items, _component.ContextMenuModel.ChildNodes);
		}

		private void _folderTreeContextMenu_Opening(object sender, CancelEventArgs e)
		{
			_lastClickOnFolderView = false;
			ToolStripBuilder.Clear(_folderTreeContextMenu.Items);
			ToolStripBuilder.BuildMenu(_folderTreeContextMenu.Items, _component.ContextMenuModel.ChildNodes);
		}

		#endregion
	}
}