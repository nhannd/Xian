namespace ClearCanvas.Desktop.View.WinForms
{
	partial class GalleryView
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
			if (disposing)
			{
				if (_gallery != null)
					_gallery.ListChanged -= OnListChanged;
				
				if (components != null)
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
			System.Windows.Forms.ColumnHeader colName;
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this._listView = new System.Windows.Forms.ListView();
			this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			colName = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// colName
			// 
			colName.Text = "Name";
			// 
			// _toolStrip
			// 
			this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._toolStrip.Location = new System.Drawing.Point(0, 0);
			this._toolStrip.Name = "_toolStrip";
			this._toolStrip.Size = new System.Drawing.Size(342, 25);
			this._toolStrip.TabIndex = 0;
			this._toolStrip.Text = "toolStrip1";
			// 
			// _listView
			// 
			this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            colName});
			this._listView.ContextMenuStrip = this._contextMenu;
			this._listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listView.Location = new System.Drawing.Point(0, 25);
			this._listView.Name = "_listView";
			this._listView.Size = new System.Drawing.Size(342, 251);
			this._listView.TabIndex = 1;
			this._listView.UseCompatibleStateImageBehavior = false;
			this._listView.Resize += new System.EventHandler(this.OnListViewResize);
			this._listView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.OnAfterLabelEdit);
			// 
			// _contextMenu
			// 
			this._contextMenu.Name = "_contextMenu";
			this._contextMenu.Size = new System.Drawing.Size(61, 4);
			// 
			// GalleryView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._listView);
			this.Controls.Add(this._toolStrip);
			this.Name = "GalleryView";
			this.Size = new System.Drawing.Size(342, 276);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip _toolStrip;
		private System.Windows.Forms.ListView _listView;
		private System.Windows.Forms.ContextMenuStrip _contextMenu;
	}
}
