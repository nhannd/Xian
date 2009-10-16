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

namespace ClearCanvas.ImageViewer.Explorer.Local.View.WinForms
{
	partial class LocalImageExplorerControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			this.PerformDispose(disposing);
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocalImageExplorerControl));
			this._toolStripContainer = new System.Windows.Forms.ToolStripContainer();
			this._splitPane = new System.Windows.Forms.SplitContainer();
			this._folderTree = new ClearCanvas.ImageViewer.Explorer.Local.View.WinForms.CustomFolderTree();
			this._folderTreeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._folderCoordinator = new ClearCanvas.Controls.WinForms.FolderCoordinator(this.components);
			this._folderView = new ClearCanvas.ImageViewer.Explorer.Local.View.WinForms.CustomFolderView();
			this._folderViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this._btnBack = new System.Windows.Forms.ToolStripSplitButton();
			this._btnForward = new System.Windows.Forms.ToolStripSplitButton();
			this._btnUp = new System.Windows.Forms.ToolStripButton();
			this._separatorScimitar = new System.Windows.Forms.ToolStripSeparator();
			this._btnHome = new System.Windows.Forms.ToolStripButton();
			this._btnRefresh = new System.Windows.Forms.ToolStripButton();
			this._separatorSabre = new System.Windows.Forms.ToolStripSeparator();
			this._btnShowFolders = new System.Windows.Forms.ToolStripButton();
			this._btnViews = new System.Windows.Forms.ToolStripDropDownButton();
			this._mnuTilesView = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuIconsView = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuListView = new System.Windows.Forms.ToolStripMenuItem();
			this._mnuDetailsView = new System.Windows.Forms.ToolStripMenuItem();
			this._addressStrip = new System.Windows.Forms.ToolStrip();
			this._lblAddress = new System.Windows.Forms.ToolStripLabel();
			this._txtAddress = new ClearCanvas.Controls.WinForms.FolderLocationToolStripTextBox();
			this._btnGo = new System.Windows.Forms.ToolStripButton();
			this._largeIconImageList = new System.Windows.Forms.ImageList(this.components);
			this._mediumIconImageList = new System.Windows.Forms.ImageList(this.components);
			this._smallIconImageList = new System.Windows.Forms.ImageList(this.components);
			this._toolStripContainer.ContentPanel.SuspendLayout();
			this._toolStripContainer.TopToolStripPanel.SuspendLayout();
			this._toolStripContainer.SuspendLayout();
			this._splitPane.Panel1.SuspendLayout();
			this._splitPane.Panel2.SuspendLayout();
			this._splitPane.SuspendLayout();
			this._toolStrip.SuspendLayout();
			this._addressStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _toolStripContainer
			// 
			// 
			// _toolStripContainer.ContentPanel
			// 
			this._toolStripContainer.ContentPanel.Controls.Add(this._splitPane);
			this._toolStripContainer.ContentPanel.Size = new System.Drawing.Size(959, 547);
			this._toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// _toolStripContainer.LeftToolStripPanel
			// 
			this._toolStripContainer.LeftToolStripPanel.Enabled = false;
			this._toolStripContainer.LeftToolStripPanelVisible = false;
			this._toolStripContainer.Location = new System.Drawing.Point(0, 0);
			this._toolStripContainer.Name = "_toolStripContainer";
			// 
			// _toolStripContainer.RightToolStripPanel
			// 
			this._toolStripContainer.RightToolStripPanel.Enabled = false;
			this._toolStripContainer.RightToolStripPanelVisible = false;
			this._toolStripContainer.Size = new System.Drawing.Size(959, 597);
			this._toolStripContainer.TabIndex = 0;
			this._toolStripContainer.Text = "toolStripContainer1";
			// 
			// _toolStripContainer.TopToolStripPanel
			// 
			this._toolStripContainer.TopToolStripPanel.Controls.Add(this._toolStrip);
			this._toolStripContainer.TopToolStripPanel.Controls.Add(this._addressStrip);
			// 
			// _splitPane
			// 
			this._splitPane.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitPane.Location = new System.Drawing.Point(0, 0);
			this._splitPane.Name = "_splitPane";
			// 
			// _splitPane.Panel1
			// 
			this._splitPane.Panel1.Controls.Add(this._folderTree);
			// 
			// _splitPane.Panel2
			// 
			this._splitPane.Panel2.Controls.Add(this._folderView);
			this._splitPane.Size = new System.Drawing.Size(959, 547);
			this._splitPane.SplitterDistance = 319;
			this._splitPane.TabIndex = 0;
			// 
			// _folderTree
			// 
			this._folderTree.ContextMenuStrip = this._folderTreeContextMenu;
			this._folderTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this._folderTree.FolderCoordinator = this._folderCoordinator;
			this._folderTree.Location = new System.Drawing.Point(0, 0);
			this._folderTree.Name = "_folderTree";
			this._folderTree.Size = new System.Drawing.Size(319, 547);
			this._folderTree.TabIndex = 0;
			// 
			// _folderTreeContextMenu
			// 
			this._folderTreeContextMenu.Name = "_folderTreeContextMenu";
			this._folderTreeContextMenu.Size = new System.Drawing.Size(61, 4);
			this._folderTreeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._folderTreeContextMenu_Opening);
			// 
			// _folderCoordinator
			// 
			this._folderCoordinator.CurrentPidlChanged += new System.EventHandler(this._folderCoordinator_CurrentPidlChanged);
			// 
			// _folderView
			// 
			this._folderView.ContextMenuStrip = this._folderViewContextMenu;
			this._folderView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._folderView.FolderCoordinator = this._folderCoordinator;
			this._folderView.Location = new System.Drawing.Point(0, 0);
			this._folderView.Name = "_folderView";
			this._folderView.Size = new System.Drawing.Size(636, 547);
			this._folderView.TabIndex = 0;
			this._folderView.ItemDoubleClick += new ClearCanvas.Controls.WinForms.FolderViewItemEventHandler(this._folderView_ItemDoubleClick);
			// 
			// _folderViewContextMenu
			// 
			this._folderViewContextMenu.Name = "_folderViewContextMenu";
			this._folderViewContextMenu.Size = new System.Drawing.Size(61, 4);
			this._folderViewContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._folderViewContextMenu_Opening);
			// 
			// _toolStrip
			// 
			this._toolStrip.Dock = System.Windows.Forms.DockStyle.None;
			this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._btnBack,
            this._btnForward,
            this._btnUp,
            this._separatorScimitar,
            this._btnHome,
            this._btnRefresh,
            this._separatorSabre,
            this._btnShowFolders,
            this._btnViews});
			this._toolStrip.Location = new System.Drawing.Point(0, 0);
			this._toolStrip.Name = "_toolStrip";
			this._toolStrip.Size = new System.Drawing.Size(959, 25);
			this._toolStrip.Stretch = true;
			this._toolStrip.TabIndex = 3;
			this._toolStrip.Text = "Standard Buttons";
			// 
			// _btnBack
			// 
			this._btnBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnBack.Image = ((System.Drawing.Image)(resources.GetObject("_btnBack.Image")));
			this._btnBack.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._btnBack.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnBack.Name = "_btnBack";
			this._btnBack.Size = new System.Drawing.Size(32, 22);
			this._btnBack.Text = "Back";
			this._btnBack.ButtonClick += new System.EventHandler(this._btnBack_Click);
			// 
			// _btnForward
			// 
			this._btnForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnForward.Image = ((System.Drawing.Image)(resources.GetObject("_btnForward.Image")));
			this._btnForward.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._btnForward.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnForward.Name = "_btnForward";
			this._btnForward.Size = new System.Drawing.Size(32, 22);
			this._btnForward.Text = "Forward";
			this._btnForward.ButtonClick += new System.EventHandler(this._btnForward_Click);
			// 
			// _btnUp
			// 
			this._btnUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnUp.Image = ((System.Drawing.Image)(resources.GetObject("_btnUp.Image")));
			this._btnUp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._btnUp.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnUp.Name = "_btnUp";
			this._btnUp.Size = new System.Drawing.Size(23, 22);
			this._btnUp.Text = "Up";
			this._btnUp.Click += new System.EventHandler(this._btnUp_Click);
			// 
			// _separatorScimitar
			// 
			this._separatorScimitar.Name = "_separatorScimitar";
			this._separatorScimitar.Size = new System.Drawing.Size(6, 25);
			// 
			// _btnHome
			// 
			this._btnHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnHome.Image = ((System.Drawing.Image)(resources.GetObject("_btnHome.Image")));
			this._btnHome.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._btnHome.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnHome.Name = "_btnHome";
			this._btnHome.Size = new System.Drawing.Size(23, 22);
			this._btnHome.Text = "Home";
			this._btnHome.Click += new System.EventHandler(this._btnHome_Click);
			// 
			// _btnRefresh
			// 
			this._btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("_btnRefresh.Image")));
			this._btnRefresh.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnRefresh.Name = "_btnRefresh";
			this._btnRefresh.Size = new System.Drawing.Size(23, 22);
			this._btnRefresh.Text = "Refresh";
			this._btnRefresh.Click += new System.EventHandler(this._btnRefresh_Click);
			// 
			// _separatorSabre
			// 
			this._separatorSabre.Name = "_separatorSabre";
			this._separatorSabre.Size = new System.Drawing.Size(6, 25);
			// 
			// _btnShowFolders
			// 
			this._btnShowFolders.Checked = true;
			this._btnShowFolders.CheckState = System.Windows.Forms.CheckState.Checked;
			this._btnShowFolders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnShowFolders.Image = ((System.Drawing.Image)(resources.GetObject("_btnShowFolders.Image")));
			this._btnShowFolders.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._btnShowFolders.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnShowFolders.Name = "_btnShowFolders";
			this._btnShowFolders.Size = new System.Drawing.Size(23, 22);
			this._btnShowFolders.Text = "Folders";
			this._btnShowFolders.Click += new System.EventHandler(this._btnShowFolders_Click);
			// 
			// _btnViews
			// 
			this._btnViews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnViews.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuTilesView,
            this._mnuIconsView,
            this._mnuListView,
            this._mnuDetailsView});
			this._btnViews.Image = ((System.Drawing.Image)(resources.GetObject("_btnViews.Image")));
			this._btnViews.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._btnViews.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnViews.Name = "_btnViews";
			this._btnViews.Size = new System.Drawing.Size(29, 22);
			this._btnViews.Text = "Views";
			// 
			// _mnuTilesView
			// 
			this._mnuTilesView.Name = "_mnuTilesView";
			this._mnuTilesView.Size = new System.Drawing.Size(106, 22);
			this._mnuTilesView.Text = "Tile&s";
			this._mnuTilesView.Click += new System.EventHandler(this._mnuTilesView_Click);
			// 
			// _mnuIconsView
			// 
			this._mnuIconsView.Checked = true;
			this._mnuIconsView.CheckState = System.Windows.Forms.CheckState.Checked;
			this._mnuIconsView.Name = "_mnuIconsView";
			this._mnuIconsView.Size = new System.Drawing.Size(106, 22);
			this._mnuIconsView.Text = "Ico&ns";
			this._mnuIconsView.Click += new System.EventHandler(this._mnuIconsView_Click);
			// 
			// _mnuListView
			// 
			this._mnuListView.Name = "_mnuListView";
			this._mnuListView.Size = new System.Drawing.Size(106, 22);
			this._mnuListView.Text = "&List";
			this._mnuListView.Click += new System.EventHandler(this._mnuListView_Click);
			// 
			// _mnuDetailsView
			// 
			this._mnuDetailsView.Name = "_mnuDetailsView";
			this._mnuDetailsView.Size = new System.Drawing.Size(106, 22);
			this._mnuDetailsView.Text = "&Details";
			this._mnuDetailsView.Click += new System.EventHandler(this._mnuDetailsView_Click);
			// 
			// _addressStrip
			// 
			this._addressStrip.Dock = System.Windows.Forms.DockStyle.None;
			this._addressStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._addressStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._lblAddress,
            this._txtAddress,
            this._btnGo});
			this._addressStrip.Location = new System.Drawing.Point(0, 25);
			this._addressStrip.Name = "_addressStrip";
			this._addressStrip.Size = new System.Drawing.Size(959, 25);
			this._addressStrip.Stretch = true;
			this._addressStrip.TabIndex = 4;
			this._addressStrip.Text = "Address Bar";
			this._addressStrip.SizeChanged += new System.EventHandler(this._addressStrip_SizeChanged);
			// 
			// _lblAddress
			// 
			this._lblAddress.Name = "_lblAddress";
			this._lblAddress.Size = new System.Drawing.Size(46, 22);
			this._lblAddress.Text = "Address";
			// 
			// _txtAddress
			// 
			this._txtAddress.Name = "_txtAddress";
			this._txtAddress.Size = new System.Drawing.Size(100, 25);
			this._txtAddress.KeyEnterPressed += new System.EventHandler(this._txtAddress_KeyEnterPressed);
			// 
			// _btnGo
			// 
			this._btnGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnGo.Image = ((System.Drawing.Image)(resources.GetObject("_btnGo.Image")));
			this._btnGo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._btnGo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnGo.Name = "_btnGo";
			this._btnGo.Size = new System.Drawing.Size(23, 22);
			this._btnGo.Text = "Go";
			this._btnGo.Click += new System.EventHandler(this._btnGo_Click);
			// 
			// _largeIconImageList
			// 
			this._largeIconImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._largeIconImageList.ImageSize = new System.Drawing.Size(64, 64);
			this._largeIconImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// _mediumIconImageList
			// 
			this._mediumIconImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._mediumIconImageList.ImageSize = new System.Drawing.Size(48, 48);
			this._mediumIconImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// _smallIconImageList
			// 
			this._smallIconImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._smallIconImageList.ImageSize = new System.Drawing.Size(24, 24);
			this._smallIconImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// LocalImageExplorerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._toolStripContainer);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "LocalImageExplorerControl";
			this.Size = new System.Drawing.Size(959, 597);
			this._toolStripContainer.ContentPanel.ResumeLayout(false);
			this._toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this._toolStripContainer.TopToolStripPanel.PerformLayout();
			this._toolStripContainer.ResumeLayout(false);
			this._toolStripContainer.PerformLayout();
			this._splitPane.Panel1.ResumeLayout(false);
			this._splitPane.Panel2.ResumeLayout(false);
			this._splitPane.ResumeLayout(false);
			this._toolStrip.ResumeLayout(false);
			this._toolStrip.PerformLayout();
			this._addressStrip.ResumeLayout(false);
			this._addressStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer _toolStripContainer;
		private System.Windows.Forms.SplitContainer _splitPane;
		private System.Windows.Forms.ToolStrip _addressStrip;
		private System.Windows.Forms.ToolStripLabel _lblAddress;
		private ClearCanvas.Controls.WinForms.FolderLocationToolStripTextBox _txtAddress;
		private System.Windows.Forms.ToolStrip _toolStrip;
		private System.Windows.Forms.ToolStripSplitButton _btnBack;
		private System.Windows.Forms.ToolStripSplitButton _btnForward;
		private System.Windows.Forms.ToolStripButton _btnUp;
		private System.Windows.Forms.ToolStripSeparator _separatorScimitar;
		private System.Windows.Forms.ToolStripButton _btnHome;
		private System.Windows.Forms.ToolStripButton _btnRefresh;
		private System.Windows.Forms.ToolStripSeparator _separatorSabre;
		private System.Windows.Forms.ToolStripButton _btnShowFolders;
		private System.Windows.Forms.ToolStripDropDownButton _btnViews;
		private System.Windows.Forms.ToolStripMenuItem _mnuTilesView;
		private System.Windows.Forms.ToolStripMenuItem _mnuIconsView;
		private System.Windows.Forms.ToolStripMenuItem _mnuListView;
		private System.Windows.Forms.ToolStripMenuItem _mnuDetailsView;
		private ClearCanvas.Controls.WinForms.FolderCoordinator _folderCoordinator;
		private ClearCanvas.ImageViewer.Explorer.Local.View.WinForms.CustomFolderTree _folderTree;
		private ClearCanvas.ImageViewer.Explorer.Local.View.WinForms.CustomFolderView _folderView;
		private System.Windows.Forms.ContextMenuStrip _folderViewContextMenu;
		private System.Windows.Forms.ContextMenuStrip _folderTreeContextMenu;
		private System.Windows.Forms.ImageList _largeIconImageList;
		private System.Windows.Forms.ImageList _mediumIconImageList;
		private System.Windows.Forms.ImageList _smallIconImageList;
		private System.Windows.Forms.ToolStripButton _btnGo;
	}
}
