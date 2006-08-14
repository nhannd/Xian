namespace ClearCanvas.ImageViewer.Dashboard.Local
{
	partial class DetailViewControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer _components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (_components != null))
			{
				_components.Dispose();
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
			this._components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailViewControl));
			this._treeViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this._components);
			this._loadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this._loadButton = new System.Windows.Forms.ToolStripButton();
			this._goToComboBox = new System.Windows.Forms.ToolStripComboBox();
			this._pathBox = new System.Windows.Forms.ToolStripLabel();
			this._fileSystemTreeView = new ClearCanvas.Controls.WinForms.FileSystemTreeView();
			this._headerStrip = new ClearCanvas.Controls.WinForms.HeaderStrip();
			this._headerLabel = new System.Windows.Forms.ToolStripLabel();
			this._treeViewContextMenu.SuspendLayout();
			this._toolStrip.SuspendLayout();
			this._headerStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// TreeViewContextMenu
			// 
			this._treeViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._loadMenuItem});
			this._treeViewContextMenu.Name = "ContextMenuStrip";
			this._treeViewContextMenu.Size = new System.Drawing.Size(120, 26);
			// 
			// LoadMenuItem
			// 
			this._loadMenuItem.Name = "LoadMenuItem";
			this._loadMenuItem.Size = new System.Drawing.Size(119, 22);
			this._loadMenuItem.Text = "Open";
			// 
			// ToolStrip
			// 
			this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._loadButton,
            this._goToComboBox,
            this._pathBox});
			this._toolStrip.Location = new System.Drawing.Point(0, 25);
			this._toolStrip.Name = "ToolStrip";
			this._toolStrip.Size = new System.Drawing.Size(513, 28);
			this._toolStrip.TabIndex = 4;
			this._toolStrip.Text = "toolStrip1";
			// 
			// LoadButton
			// 
			this._loadButton.Image = ((System.Drawing.Image)(resources.GetObject("LoadButton.Image")));
			this._loadButton.Name = "LoadButton";
			this._loadButton.Size = new System.Drawing.Size(58, 25);
			this._loadButton.Text = "Open";
			this._loadButton.ToolTipText = "Open DICOM Images";
			// 
			// GoToComboBox
			// 
			this._goToComboBox.Name = "GoToComboBox";
			this._goToComboBox.Size = new System.Drawing.Size(121, 28);
			// 
			// PathBox
			// 
			this._pathBox.AutoSize = false;
			this._pathBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this._pathBox.Name = "PathBox";
			this._pathBox.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this._pathBox.Size = new System.Drawing.Size(150, 25);
			this._pathBox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FileSystemTreeView
			// 
			this._fileSystemTreeView.ContextMenuStrip = this._treeViewContextMenu;
			this._fileSystemTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fileSystemTreeView.ImageIndex = 0;
			this._fileSystemTreeView.Location = new System.Drawing.Point(0, 53);
			this._fileSystemTreeView.Name = "FileSystemTreeView";
			this._fileSystemTreeView.SelectedImageIndex = 0;
			this._fileSystemTreeView.ShowFiles = true;
			this._fileSystemTreeView.Size = new System.Drawing.Size(513, 196);
			this._fileSystemTreeView.TabIndex = 0;
			// 
			// headerStrip
			// 
			this._headerStrip.AutoSize = false;
			this._headerStrip.Font = new System.Drawing.Font("Arial", 13.5F, System.Drawing.FontStyle.Bold);
			this._headerStrip.ForeColor = System.Drawing.Color.White;
			this._headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._headerLabel});
			this._headerStrip.Location = new System.Drawing.Point(0, 0);
			this._headerStrip.Name = "headerStrip";
			this._headerStrip.Size = new System.Drawing.Size(513, 25);
			this._headerStrip.TabIndex = 1;
			this._headerStrip.Text = "headerStrip1";
			// 
			// HeaderLabel
			// 
			this._headerLabel.Name = "HeaderLabel";
			this._headerLabel.Size = new System.Drawing.Size(0, 22);
			// 
			// DetailViewControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._fileSystemTreeView);
			this.Controls.Add(this._toolStrip);
			this.Controls.Add(this._headerStrip);
			this.Name = "DetailViewControl";
			this.Size = new System.Drawing.Size(513, 249);
			this._treeViewContextMenu.ResumeLayout(false);
			this._toolStrip.ResumeLayout(false);
			this._toolStrip.PerformLayout();
			this._headerStrip.ResumeLayout(false);
			this._headerStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ClearCanvas.Controls.WinForms.FileSystemTreeView _fileSystemTreeView;
		private ClearCanvas.Controls.WinForms.HeaderStrip _headerStrip;
		private System.Windows.Forms.ToolStripLabel _headerLabel;
		private System.Windows.Forms.ContextMenuStrip _treeViewContextMenu;
		private System.Windows.Forms.ToolStripMenuItem _loadMenuItem;
		private System.Windows.Forms.ToolStrip _toolStrip;
		private System.Windows.Forms.ToolStripComboBox _goToComboBox;
		private System.Windows.Forms.ToolStripButton _loadButton;
		private System.Windows.Forms.ToolStripLabel _pathBox;
	}
}
