namespace MyPlugin.TextEditor {
	partial class TextEditorControl {
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
			this._txtText = new System.Windows.Forms.TextBox();
			this._pnlSidePanel = new System.Windows.Forms.Panel();
			this._btnSave = new System.Windows.Forms.Button();
			this._txtFilename = new System.Windows.Forms.TextBox();
			this._lblFilename = new System.Windows.Forms.Label();
			this._btnCancel = new System.Windows.Forms.Button();
			this._txtWordCount = new System.Windows.Forms.TextBox();
			this._lblWordCount = new System.Windows.Forms.Label();
			this._pnlSidePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _txtText
			// 
			this._txtText.Dock = System.Windows.Forms.DockStyle.Fill;
			this._txtText.Location = new System.Drawing.Point(0, 0);
			this._txtText.Multiline = true;
			this._txtText.Name = "_txtText";
			this._txtText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this._txtText.Size = new System.Drawing.Size(871, 417);
			this._txtText.TabIndex = 1;
			// 
			// _pnlSidePanel
			// 
			this._pnlSidePanel.Controls.Add(this._btnSave);
			this._pnlSidePanel.Controls.Add(this._txtFilename);
			this._pnlSidePanel.Controls.Add(this._lblFilename);
			this._pnlSidePanel.Controls.Add(this._btnCancel);
			this._pnlSidePanel.Controls.Add(this._txtWordCount);
			this._pnlSidePanel.Controls.Add(this._lblWordCount);
			this._pnlSidePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._pnlSidePanel.Location = new System.Drawing.Point(0, 417);
			this._pnlSidePanel.Name = "_pnlSidePanel";
			this._pnlSidePanel.Size = new System.Drawing.Size(871, 47);
			this._pnlSidePanel.TabIndex = 2;
			// 
			// _btnSave
			// 
			this._btnSave.Dock = System.Windows.Forms.DockStyle.Right;
			this._btnSave.Location = new System.Drawing.Point(739, 0);
			this._btnSave.Name = "_btnSave";
			this._btnSave.Size = new System.Drawing.Size(66, 47);
			this._btnSave.TabIndex = 7;
			this._btnSave.Text = "&Save";
			this._btnSave.UseVisualStyleBackColor = true;
			// 
			// _txtFilename
			// 
			this._txtFilename.Location = new System.Drawing.Point(76, 19);
			this._txtFilename.Name = "_txtFilename";
			this._txtFilename.Size = new System.Drawing.Size(209, 20);
			this._txtFilename.TabIndex = 6;
			// 
			// _lblFilename
			// 
			this._lblFilename.AutoSize = true;
			this._lblFilename.Location = new System.Drawing.Point(73, 3);
			this._lblFilename.Name = "_lblFilename";
			this._lblFilename.Size = new System.Drawing.Size(49, 13);
			this._lblFilename.TabIndex = 5;
			this._lblFilename.Text = "Filename";
			// 
			// _btnCancel
			// 
			this._btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
			this._btnCancel.Location = new System.Drawing.Point(805, 0);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(66, 47);
			this._btnCancel.TabIndex = 8;
			this._btnCancel.Text = "&Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			// 
			// _txtWordCount
			// 
			this._txtWordCount.Location = new System.Drawing.Point(6, 19);
			this._txtWordCount.Name = "_txtWordCount";
			this._txtWordCount.ReadOnly = true;
			this._txtWordCount.Size = new System.Drawing.Size(61, 20);
			this._txtWordCount.TabIndex = 4;
			// 
			// _lblWordCount
			// 
			this._lblWordCount.AutoSize = true;
			this._lblWordCount.Location = new System.Drawing.Point(3, 3);
			this._lblWordCount.Name = "_lblWordCount";
			this._lblWordCount.Size = new System.Drawing.Size(64, 13);
			this._lblWordCount.TabIndex = 3;
			this._lblWordCount.Text = "Word Count";
			// 
			// TextEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._txtText);
			this.Controls.Add(this._pnlSidePanel);
			this.Name = "TextEditorControl";
			this.Size = new System.Drawing.Size(871, 464);
			this._pnlSidePanel.ResumeLayout(false);
			this._pnlSidePanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _txtText;
		private System.Windows.Forms.Panel _pnlSidePanel;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Button _btnSave;
		private System.Windows.Forms.TextBox _txtWordCount;
		private System.Windows.Forms.Label _lblWordCount;
		private System.Windows.Forms.TextBox _txtFilename;
		private System.Windows.Forms.Label _lblFilename;
	}
}
