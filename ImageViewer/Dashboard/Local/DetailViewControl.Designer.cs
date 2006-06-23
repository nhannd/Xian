namespace ClearCanvas.Workstation.Dashboard.Local
{
	partial class DetailViewControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailViewControl));
			this.TreeViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.LoadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStrip = new System.Windows.Forms.ToolStrip();
			this.LoadButton = new System.Windows.Forms.ToolStripButton();
			this.GoToComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.PathBox = new System.Windows.Forms.ToolStripLabel();
			this.FileSystemTreeView = new ClearCanvas.Controls.WinForms.FileSystemTreeView();
			this.headerStrip = new ClearCanvas.Controls.WinForms.HeaderStrip();
			this.HeaderLabel = new System.Windows.Forms.ToolStripLabel();
			this.TreeViewContextMenu.SuspendLayout();
			this.ToolStrip.SuspendLayout();
			this.headerStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// TreeViewContextMenu
			// 
			this.TreeViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadMenuItem});
			this.TreeViewContextMenu.Name = "ContextMenuStrip";
			this.TreeViewContextMenu.Size = new System.Drawing.Size(120, 26);
			// 
			// LoadMenuItem
			// 
			this.LoadMenuItem.Name = "LoadMenuItem";
			this.LoadMenuItem.Size = new System.Drawing.Size(119, 22);
			this.LoadMenuItem.Text = "Open";
			// 
			// ToolStrip
			// 
			this.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadButton,
            this.GoToComboBox,
            this.PathBox});
			this.ToolStrip.Location = new System.Drawing.Point(0, 25);
			this.ToolStrip.Name = "ToolStrip";
			this.ToolStrip.Size = new System.Drawing.Size(513, 28);
			this.ToolStrip.TabIndex = 4;
			this.ToolStrip.Text = "toolStrip1";
			// 
			// LoadButton
			// 
			this.LoadButton.Image = ((System.Drawing.Image)(resources.GetObject("LoadButton.Image")));
			this.LoadButton.Name = "LoadButton";
			this.LoadButton.Size = new System.Drawing.Size(58, 25);
			this.LoadButton.Text = "Open";
			this.LoadButton.ToolTipText = "Open DICOM Images";
			// 
			// GoToComboBox
			// 
			this.GoToComboBox.Name = "GoToComboBox";
			this.GoToComboBox.Size = new System.Drawing.Size(121, 28);
			// 
			// PathBox
			// 
			this.PathBox.AutoSize = false;
			this.PathBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.PathBox.Name = "PathBox";
			this.PathBox.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this.PathBox.Size = new System.Drawing.Size(150, 25);
			this.PathBox.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FileSystemTreeView
			// 
			this.FileSystemTreeView.ContextMenuStrip = this.TreeViewContextMenu;
			this.FileSystemTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.FileSystemTreeView.ImageIndex = 0;
			this.FileSystemTreeView.Location = new System.Drawing.Point(0, 53);
			this.FileSystemTreeView.Name = "FileSystemTreeView";
			this.FileSystemTreeView.SelectedImageIndex = 0;
			this.FileSystemTreeView.ShowFiles = true;
			this.FileSystemTreeView.Size = new System.Drawing.Size(513, 196);
			this.FileSystemTreeView.TabIndex = 0;
			// 
			// headerStrip
			// 
			this.headerStrip.AutoSize = false;
			this.headerStrip.Font = new System.Drawing.Font("Arial", 13.5F, System.Drawing.FontStyle.Bold);
			this.headerStrip.ForeColor = System.Drawing.Color.White;
			this.headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.HeaderLabel});
			this.headerStrip.Location = new System.Drawing.Point(0, 0);
			this.headerStrip.Name = "headerStrip";
			this.headerStrip.Size = new System.Drawing.Size(513, 25);
			this.headerStrip.TabIndex = 1;
			this.headerStrip.Text = "headerStrip1";
			// 
			// HeaderLabel
			// 
			this.HeaderLabel.Name = "HeaderLabel";
			this.HeaderLabel.Size = new System.Drawing.Size(0, 22);
			// 
			// DetailViewControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.FileSystemTreeView);
			this.Controls.Add(this.ToolStrip);
			this.Controls.Add(this.headerStrip);
			this.Name = "DetailViewControl";
			this.Size = new System.Drawing.Size(513, 249);
			this.TreeViewContextMenu.ResumeLayout(false);
			this.ToolStrip.ResumeLayout(false);
			this.ToolStrip.PerformLayout();
			this.headerStrip.ResumeLayout(false);
			this.headerStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ClearCanvas.Controls.WinForms.FileSystemTreeView FileSystemTreeView;
		private ClearCanvas.Controls.WinForms.HeaderStrip headerStrip;
		private System.Windows.Forms.ToolStripLabel HeaderLabel;
		private System.Windows.Forms.ContextMenuStrip TreeViewContextMenu;
		private System.Windows.Forms.ToolStripMenuItem LoadMenuItem;
		private System.Windows.Forms.ToolStrip ToolStrip;
		private System.Windows.Forms.ToolStripComboBox GoToComboBox;
		private System.Windows.Forms.ToolStripButton LoadButton;
		private System.Windows.Forms.ToolStripLabel PathBox;
	}
}
