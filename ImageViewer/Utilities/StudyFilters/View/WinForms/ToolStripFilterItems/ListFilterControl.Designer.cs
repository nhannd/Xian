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
			this._buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._btnCancel = new System.Windows.Forms.Button();
			this._btnOk = new System.Windows.Forms.Button();
			this._buttonPanel.SuspendLayout();
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
			this._listBox.Size = new System.Drawing.Size(174, 221);
			this._listBox.TabIndex = 0;
			this._listBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._listBox_ItemCheck);
			this._listBox.MouseLeave += new System.EventHandler(this._listBox_MouseLeave);
			// 
			// _buttonPanel
			// 
			this._buttonPanel.Controls.Add(this._btnCancel);
			this._buttonPanel.Controls.Add(this._btnOk);
			this._buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this._buttonPanel.Location = new System.Drawing.Point(0, 221);
			this._buttonPanel.Name = "_buttonPanel";
			this._buttonPanel.Size = new System.Drawing.Size(174, 28);
			this._buttonPanel.TabIndex = 1;
			// 
			// _btnCancel
			// 
			this._btnCancel.Location = new System.Drawing.Point(96, 3);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 0;
			this._btnCancel.Text = "Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
			// 
			// _btnOk
			// 
			this._btnOk.Location = new System.Drawing.Point(15, 3);
			this._btnOk.Name = "_btnOk";
			this._btnOk.Size = new System.Drawing.Size(75, 23);
			this._btnOk.TabIndex = 1;
			this._btnOk.Text = "OK";
			this._btnOk.UseVisualStyleBackColor = true;
			this._btnOk.Click += new System.EventHandler(this._btnOk_Click);
			// 
			// ListFilterControl
			// 
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this._listBox);
			this.Controls.Add(this._buttonPanel);
			this.Name = "ListFilterControl";
			this.Size = new System.Drawing.Size(174, 249);
			this._buttonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckedListBox _listBox;
		private System.Windows.Forms.FlowLayoutPanel _buttonPanel;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Button _btnOk;

	}
}
