namespace ClearCanvas.Desktop.Configuration.View.WinForms {
	partial class ToolStripConfigComponentControl {
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
			System.Windows.Forms.FlowLayoutPanel _flowGlobalToolbars;
			this._chkWrapGlobalToolbars = new System.Windows.Forms.CheckBox();
			_flowGlobalToolbars = new System.Windows.Forms.FlowLayoutPanel();
			_flowGlobalToolbars.SuspendLayout();
			this.SuspendLayout();
			// 
			// _flowGlobalToolbars
			// 
			_flowGlobalToolbars.Controls.Add(this._chkWrapGlobalToolbars);
			_flowGlobalToolbars.Dock = System.Windows.Forms.DockStyle.Fill;
			_flowGlobalToolbars.Location = new System.Drawing.Point(0, 0);
			_flowGlobalToolbars.Name = "_flowGlobalToolbars";
			_flowGlobalToolbars.Padding = new System.Windows.Forms.Padding(15);
			_flowGlobalToolbars.Size = new System.Drawing.Size(379, 180);
			_flowGlobalToolbars.TabIndex = 1;
			// 
			// _chkWrapGlobalToolbars
			// 
			this._chkWrapGlobalToolbars.AutoSize = true;
			this._chkWrapGlobalToolbars.Location = new System.Drawing.Point(18, 18);
			this._chkWrapGlobalToolbars.Name = "_chkWrapGlobalToolbars";
			this._chkWrapGlobalToolbars.Size = new System.Drawing.Size(147, 17);
			this._chkWrapGlobalToolbars.TabIndex = 1;
			this._chkWrapGlobalToolbars.Text = "Wrap toolbars horizontally";
			this._chkWrapGlobalToolbars.UseVisualStyleBackColor = true;
			// 
			// ToolStripConfigComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(_flowGlobalToolbars);
			this.Name = "ToolStripConfigComponentControl";
			this.Size = new System.Drawing.Size(379, 180);
			_flowGlobalToolbars.ResumeLayout(false);
			_flowGlobalToolbars.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkWrapGlobalToolbars;
	}
}
