namespace MyPlugin.TextEditor {
	partial class TextEditorConfigControl {
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
			this._chkWordWrap = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _chkWordWrap
			// 
			this._chkWordWrap.AutoSize = true;
			this._chkWordWrap.Location = new System.Drawing.Point(31, 28);
			this._chkWordWrap.Name = "_chkWordWrap";
			this._chkWordWrap.Size = new System.Drawing.Size(81, 17);
			this._chkWordWrap.TabIndex = 0;
			this._chkWordWrap.Text = "Word Wrap";
			this._chkWordWrap.UseVisualStyleBackColor = true;
			// 
			// TextEditorConfigControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._chkWordWrap);
			this.Name = "TextEditorConfigControl";
			this.Size = new System.Drawing.Size(351, 166);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkWordWrap;
	}
}
