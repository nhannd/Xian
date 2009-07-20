namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems {
	partial class ListFilterControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this._listBox = new System.Windows.Forms.CheckedListBox();
			this.SuspendLayout();
			// 
			// _listBox
			// 
			this._listBox.CheckOnClick = true;
			this._listBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listBox.FormattingEnabled = true;
			this._listBox.IntegralHeight = false;
			this._listBox.Location = new System.Drawing.Point(0, 0);
			this._listBox.Name = "_listBox";
			this._listBox.Size = new System.Drawing.Size(174, 249);
			this._listBox.TabIndex = 0;
			this._listBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._listBox_ItemCheck);
			this._listBox.MouseLeave += new System.EventHandler(this._listBox_MouseLeave);
			// 
			// ListFilterControl
			// 
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this._listBox);
			this.Name = "ListFilterControl";
			this.Size = new System.Drawing.Size(174, 249);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckedListBox _listBox;

	}
}
