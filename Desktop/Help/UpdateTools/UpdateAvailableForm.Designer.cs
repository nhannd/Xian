namespace ClearCanvas.Desktop.Help
{
	partial class UpdateAvailableForm
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
			this._ok = new System.Windows.Forms.Button();
			this._text = new System.Windows.Forms.Label();
			this._link = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// _ok
			// 
			this._ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._ok.Location = new System.Drawing.Point(268, 49);
			this._ok.Name = "_ok";
			this._ok.Size = new System.Drawing.Size(75, 23);
			this._ok.TabIndex = 1;
			this._ok.Text = "OK";
			this._ok.UseVisualStyleBackColor = true;
			this._ok.Click += new System.EventHandler(this.OnOk);
			// 
			// _text
			// 
			this._text.Location = new System.Drawing.Point(12, 9);
			this._text.Name = "_text";
			this._text.Size = new System.Drawing.Size(331, 25);
			this._text.TabIndex = 2;
			this._text.Text = "label1";
			this._text.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _link
			// 
			this._link.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._link.AutoSize = true;
			this._link.Location = new System.Drawing.Point(136, 54);
			this._link.Name = "_link";
			this._link.Size = new System.Drawing.Size(80, 13);
			this._link.TabIndex = 3;
			this._link.TabStop = true;
			this._link.Text = "Download Now";
			this._link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnDownloadNow);
			// 
			// UpdateAvailableForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this._ok;
			this.ClientSize = new System.Drawing.Size(355, 84);
			this.Controls.Add(this._ok);
			this.Controls.Add(this._link);
			this.Controls.Add(this._text);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UpdateAvailableForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.Label _text;
		private System.Windows.Forms.LinkLabel _link;

	}
}
