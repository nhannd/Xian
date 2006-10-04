namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	partial class AENavigatorControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AENavigatorControl));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this._btnAdd = new System.Windows.Forms.ToolStripButton();
			this._btnAddGroup = new System.Windows.Forms.ToolStripButton();
			this._btnEdit = new System.Windows.Forms.ToolStripButton();
			this._btnDelete = new System.Windows.Forms.ToolStripButton();
			this._btnCEcho = new System.Windows.Forms.ToolStripButton();
			this.titleBar1 = new Crownwood.DotNetMagic.Controls.TitleBar();
			this._aeTreeView = new System.Windows.Forms.TreeView();
			this._contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editServerGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteServerGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.verifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._imageList = new System.Windows.Forms.ImageList(this.components);
			this.toolStrip1.SuspendLayout();
			this._contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._btnAdd,
            this._btnAddGroup,
            this._btnEdit,
            this._btnDelete,
            this._btnCEcho});
			this.toolStrip1.Location = new System.Drawing.Point(0, 23);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(300, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// _btnAdd
			// 
			this._btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("_btnAdd.Image")));
			this._btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnAdd.Name = "_btnAdd";
			this._btnAdd.Size = new System.Drawing.Size(23, 22);
			this._btnAdd.Text = "toolStripButton1";
			// 
			// _btnAddGroup
			// 
			this._btnAddGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnAddGroup.Image = ((System.Drawing.Image)(resources.GetObject("_btnAddGroup.Image")));
			this._btnAddGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnAddGroup.Name = "_btnAddGroup";
			this._btnAddGroup.Size = new System.Drawing.Size(23, 22);
			this._btnAddGroup.Text = "toolStripButton1";
			// 
			// _btnEdit
			// 
			this._btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("_btnEdit.Image")));
			this._btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnEdit.Name = "_btnEdit";
			this._btnEdit.Size = new System.Drawing.Size(23, 22);
			this._btnEdit.Text = "toolStripButton1";
			// 
			// _btnDelete
			// 
			this._btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("_btnDelete.Image")));
			this._btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnDelete.Name = "_btnDelete";
			this._btnDelete.Size = new System.Drawing.Size(23, 22);
			this._btnDelete.Text = "toolStripButton1";
			// 
			// _btnCEcho
			// 
			this._btnCEcho.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._btnCEcho.Image = ((System.Drawing.Image)(resources.GetObject("_btnCEcho.Image")));
			this._btnCEcho.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._btnCEcho.Name = "_btnCEcho";
			this._btnCEcho.Size = new System.Drawing.Size(23, 22);
			this._btnCEcho.Text = "C-ECHO";
			// 
			// titleBar1
			// 
			this.titleBar1.Dock = System.Windows.Forms.DockStyle.Top;
			this.titleBar1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.titleBar1.Location = new System.Drawing.Point(0, 0);
			this.titleBar1.MouseOverColor = System.Drawing.Color.Empty;
			this.titleBar1.Name = "titleBar1";
			this.titleBar1.Size = new System.Drawing.Size(300, 23);
			this.titleBar1.TabIndex = 2;
			this.titleBar1.Text = "Servers";
			// 
			// _aeTreeView
			// 
			this._aeTreeView.ContextMenuStrip = this._contextMenuStrip;
			this._aeTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._aeTreeView.HideSelection = false;
			this._aeTreeView.ImageIndex = 0;
			this._aeTreeView.ImageList = this._imageList;
			this._aeTreeView.Location = new System.Drawing.Point(0, 48);
			this._aeTreeView.Name = "_aeTreeView";
			this._aeTreeView.SelectedImageIndex = 1;
			this._aeTreeView.Size = new System.Drawing.Size(300, 254);
			this._aeTreeView.StateImageList = this._imageList;
			this._aeTreeView.TabIndex = 3;
			// 
			// _contextMenuStrip
			// 
			this._contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addServerToolStripMenuItem,
            this.addGroupToolStripMenuItem,
            this.editServerGroupToolStripMenuItem,
            this.deleteServerGroupToolStripMenuItem,
            this.verifyToolStripMenuItem});
			this._contextMenuStrip.Name = "_contextMenuStrip";
			this._contextMenuStrip.Size = new System.Drawing.Size(187, 124);
			// 
			// addServerToolStripMenuItem
			// 
			this.addServerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addServerToolStripMenuItem.Image")));
			this.addServerToolStripMenuItem.Name = "addServerToolStripMenuItem";
			this.addServerToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
			this.addServerToolStripMenuItem.Text = "Add Server";
			// 
			// addGroupToolStripMenuItem
			// 
			this.addGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addGroupToolStripMenuItem.Image")));
			this.addGroupToolStripMenuItem.Name = "addGroupToolStripMenuItem";
			this.addGroupToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
			this.addGroupToolStripMenuItem.Text = "Add Group";
			// 
			// editServerGroupToolStripMenuItem
			// 
			this.editServerGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editServerGroupToolStripMenuItem.Image")));
			this.editServerGroupToolStripMenuItem.Name = "editServerGroupToolStripMenuItem";
			this.editServerGroupToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
			this.editServerGroupToolStripMenuItem.Text = "Edit";
			// 
			// deleteServerGroupToolStripMenuItem
			// 
			this.deleteServerGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteServerGroupToolStripMenuItem.Image")));
			this.deleteServerGroupToolStripMenuItem.Name = "deleteServerGroupToolStripMenuItem";
			this.deleteServerGroupToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
			this.deleteServerGroupToolStripMenuItem.Text = "Delete";
			// 
			// verifyToolStripMenuItem
			// 
			this.verifyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("verifyToolStripMenuItem.Image")));
			this.verifyToolStripMenuItem.Name = "verifyToolStripMenuItem";
			this.verifyToolStripMenuItem.Size = new System.Drawing.Size(186, 24);
			this.verifyToolStripMenuItem.Text = "Verify (C-Echo)";
			// 
			// _imageList
			// 
			this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
			this._imageList.TransparentColor = System.Drawing.Color.Transparent;
			this._imageList.Images.SetKeyName(0, "MyComputer.png");
			this._imageList.Images.SetKeyName(1, "Server.png");
			this._imageList.Images.SetKeyName(2, "ServerGroup.png");
			// 
			// AENavigatorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._aeTreeView);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.titleBar1);
			this.Name = "AENavigatorControl";
			this.Size = new System.Drawing.Size(300, 302);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this._contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton _btnAdd;
        private System.Windows.Forms.ToolStripButton _btnDelete;
        private Crownwood.DotNetMagic.Controls.TitleBar titleBar1;
        private System.Windows.Forms.TreeView _aeTreeView;
        private System.Windows.Forms.ToolStripButton _btnEdit;
        private System.Windows.Forms.ToolStripButton _btnCEcho;
        private System.Windows.Forms.ToolStripButton _btnAddGroup;
		private System.Windows.Forms.ContextMenuStrip _contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem addServerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editServerGroupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteServerGroupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem verifyToolStripMenuItem;
		private System.Windows.Forms.ImageList _imageList;


    }
}
