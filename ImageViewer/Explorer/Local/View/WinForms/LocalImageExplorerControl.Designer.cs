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
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode(global::ClearCanvas.Controls.WinForms.SR.String1);
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Local Disk (C:)", 18, 88, new System.Windows.Forms.TreeNode[] {
            treeNode1});
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode(global::ClearCanvas.Controls.WinForms.SR.String1);
			System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("DVD-RW Drive (D:)", 19, 89, new System.Windows.Forms.TreeNode[] {
            treeNode3});
			System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode(global::ClearCanvas.Controls.WinForms.SR.String1);
			System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Control Panel", 34, 86, new System.Windows.Forms.TreeNode[] {
            treeNode5});
			System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode(global::ClearCanvas.Controls.WinForms.SR.String1);
			System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Norm\'s Documents", 14, 87, new System.Windows.Forms.TreeNode[] {
            treeNode7});
			System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode(global::ClearCanvas.Controls.WinForms.SR.String1);
			System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Shared Documents", 14, 87, new System.Windows.Forms.TreeNode[] {
            treeNode9});
			System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("My Computer", 16, 82, new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode4,
            treeNode6,
            treeNode8,
            treeNode10});
			this._fileBrowser = new ClearCanvas.Controls.WinForms.FileBrowser.Browser();
			this._fileContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._fileOpenedItem = new System.Windows.Forms.ToolStripMenuItem();
			this._folderContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._folderOpenedItem = new System.Windows.Forms.ToolStripMenuItem();
			this._shellBrowser = new ClearCanvas.Controls.WinForms.FileBrowser.ShellDll.ShellBrowser();
			this._fileContextMenu.SuspendLayout();
			this._folderContextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// _fileBrowser
			// 
			this._fileBrowser.CustomFileContextMenu = this._fileContextMenu;
			this._fileBrowser.CustomFolderContextMenu = this._folderContextMenu;
			this._fileBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fileBrowser.ListViewMode = System.Windows.Forms.View.Details;
			this._fileBrowser.Location = new System.Drawing.Point(0, 0);
			this._fileBrowser.Name = "_fileBrowser";
			treeNode1.Name = "";
			treeNode1.Text = global::ClearCanvas.Controls.WinForms.SR.String1;
			treeNode2.ImageIndex = 18;
			treeNode2.Name = "Local Disk (C:)";
			treeNode2.SelectedImageIndex = 88;
			treeNode2.Text = "Local Disk (C:)";
			treeNode3.Name = "";
			treeNode3.Text = global::ClearCanvas.Controls.WinForms.SR.String1;
			treeNode4.ImageIndex = 19;
			treeNode4.Name = "DVD-RW Drive (D:)";
			treeNode4.SelectedImageIndex = 89;
			treeNode4.Text = "DVD-RW Drive (D:)";
			treeNode5.Name = "";
			treeNode5.Text = global::ClearCanvas.Controls.WinForms.SR.String1;
			treeNode6.ImageIndex = 34;
			treeNode6.Name = "Control Panel";
			treeNode6.SelectedImageIndex = 86;
			treeNode6.Text = "Control Panel";
			treeNode7.Name = "";
			treeNode7.Text = global::ClearCanvas.Controls.WinForms.SR.String1;
			treeNode8.ImageIndex = 14;
			treeNode8.Name = "Norm\'s Documents";
			treeNode8.SelectedImageIndex = 87;
			treeNode8.Text = "Norm\'s Documents";
			treeNode9.Name = "";
			treeNode9.Text = global::ClearCanvas.Controls.WinForms.SR.String1;
			treeNode10.ImageIndex = 14;
			treeNode10.Name = "Shared Documents";
			treeNode10.SelectedImageIndex = 87;
			treeNode10.Text = "Shared Documents";
			treeNode11.ImageIndex = 16;
			treeNode11.Name = "My Computer";
			treeNode11.SelectedImageIndex = 82;
			treeNode11.Text = "My Computer";
			this._fileBrowser.SelectedNode = treeNode11;
			this._fileBrowser.ShellBrowser = this._shellBrowser;
			this._fileBrowser.Size = new System.Drawing.Size(719, 485);
			this._fileBrowser.SplitterDistance = 162;
			this._fileBrowser.TabIndex = 0;
			// 
			// _fileContextMenu
			// 
			this._fileContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._fileOpenedItem});
			this._fileContextMenu.Name = "_fileContextMenu";
			this._fileContextMenu.Size = new System.Drawing.Size(117, 28);
			// 
			// _fileOpenedItem
			// 
			this._fileOpenedItem.Name = "_fileOpenedItem";
			this._fileOpenedItem.Size = new System.Drawing.Size(116, 24);
			this._fileOpenedItem.Text = "Open";
			this._fileOpenedItem.Click += new System.EventHandler(this.OnFileViewItemOpened);
			// 
			// _folderContextMenu
			// 
			this._folderContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._folderOpenedItem});
			this._folderContextMenu.Name = "_folderContextMenu";
			this._folderContextMenu.Size = new System.Drawing.Size(153, 50);
			// 
			// _folderOpenedItem
			// 
			this._folderOpenedItem.Name = "_folderOpenedItem";
			this._folderOpenedItem.Size = new System.Drawing.Size(152, 24);
			this._folderOpenedItem.Text = "Open";
			this._folderOpenedItem.Click += new System.EventHandler(this.OnFolderViewItemOpened);
			// 
			// LocalImageExplorerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._fileBrowser);
			this.Name = "LocalImageExplorerControl";
			this.Size = new System.Drawing.Size(719, 485);
			this._fileContextMenu.ResumeLayout(false);
			this._folderContextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Controls.WinForms.FileBrowser.Browser _fileBrowser;
		private ClearCanvas.Controls.WinForms.FileBrowser.ShellDll.ShellBrowser _shellBrowser;
		private System.Windows.Forms.ContextMenuStrip _fileContextMenu;
		private System.Windows.Forms.ContextMenuStrip _folderContextMenu;
		private System.Windows.Forms.ToolStripMenuItem _fileOpenedItem;
		private System.Windows.Forms.ToolStripMenuItem _folderOpenedItem;











	}
}
