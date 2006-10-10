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
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Local Disk (C:)", 26, 27, new System.Windows.Forms.TreeNode[] {
            treeNode1});
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("");
			System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("SCRUBS_DISC1 (D:)", 30, 31, new System.Windows.Forms.TreeNode[] {
            treeNode3});
			System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("");
			System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Control Panel", 20, 21, new System.Windows.Forms.TreeNode[] {
            treeNode5});
			System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("");
			System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("My Sharing Folders", 24, 25, new System.Windows.Forms.TreeNode[] {
            treeNode7});
			System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("");
			System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Shared Documents", 17, 19, new System.Windows.Forms.TreeNode[] {
            treeNode9});
			System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("");
			System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("stewart\'s Documents", 17, 19, new System.Windows.Forms.TreeNode[] {
            treeNode11});
			System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("My Computer", 9, 10, new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode4,
            treeNode6,
            treeNode8,
            treeNode10,
            treeNode12});
			this._fileBrowser = new ClearCanvas.Controls.WinForms.FileBrowser.Browser();
			this._shellBrowser = new ClearCanvas.Controls.WinForms.FileBrowser.ShellDll.ShellBrowser();
			this.SuspendLayout();
			// 
			// _fileBrowser
			// 
			this._fileBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fileBrowser.ListViewMode = System.Windows.Forms.View.Details;
			this._fileBrowser.Location = new System.Drawing.Point(0, 0);
			this._fileBrowser.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
			this._fileBrowser.Name = "_fileBrowser";
			treeNode1.Name = "";
			treeNode1.Text = "";
			treeNode2.ImageIndex = 26;
			treeNode2.Name = "Local Disk (C:)";
			treeNode2.SelectedImageIndex = 27;
			treeNode2.Text = "Local Disk (C:)";
			treeNode3.Name = "";
			treeNode3.Text = "";
			treeNode4.ImageIndex = 30;
			treeNode4.Name = "SCRUBS_DISC1 (D:)";
			treeNode4.SelectedImageIndex = 31;
			treeNode4.Text = "SCRUBS_DISC1 (D:)";
			treeNode5.Name = "";
			treeNode5.Text = "";
			treeNode6.ImageIndex = 20;
			treeNode6.Name = "Control Panel";
			treeNode6.SelectedImageIndex = 21;
			treeNode6.Text = "Control Panel";
			treeNode7.Name = "";
			treeNode7.Text = "";
			treeNode8.ImageIndex = 24;
			treeNode8.Name = "My Sharing Folders";
			treeNode8.SelectedImageIndex = 25;
			treeNode8.Text = "My Sharing Folders";
			treeNode9.Name = "";
			treeNode9.Text = "";
			treeNode10.ImageIndex = 17;
			treeNode10.Name = "Shared Documents";
			treeNode10.SelectedImageIndex = 19;
			treeNode10.Text = "Shared Documents";
			treeNode11.Name = "";
			treeNode11.Text = "";
			treeNode12.ImageIndex = 17;
			treeNode12.Name = "stewart\'s Documents";
			treeNode12.SelectedImageIndex = 19;
			treeNode12.Text = "stewart\'s Documents";
			treeNode13.ImageIndex = 9;
			treeNode13.Name = "My Computer";
			treeNode13.SelectedImageIndex = 10;
			treeNode13.Text = "My Computer";
			this._fileBrowser.SelectedNode = treeNode13;
			this._fileBrowser.ShellBrowser = this._shellBrowser;
			this._fileBrowser.Size = new System.Drawing.Size(959, 597);
			this._fileBrowser.SplitterDistance = 162;
			this._fileBrowser.TabIndex = 0;
			// 
			// LocalImageExplorerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._fileBrowser);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "LocalImageExplorerControl";
			this.Size = new System.Drawing.Size(959, 597);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Controls.WinForms.FileBrowser.Browser _fileBrowser;
		private ClearCanvas.Controls.WinForms.FileBrowser.ShellDll.ShellBrowser _shellBrowser;
		private System.Windows.Forms.ContextMenuStrip _contextMenu;
	}
}
