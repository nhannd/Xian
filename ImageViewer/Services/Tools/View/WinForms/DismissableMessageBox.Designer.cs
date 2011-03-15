namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
	partial class DismissableMessageBox
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DismissableMessageBox));
			this._ok = new System.Windows.Forms.Button();
			this._text = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _ok
			// 
			this._ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._ok.Location = new System.Drawing.Point(125, 57);
			this._ok.Name = "_ok";
			this._ok.Size = new System.Drawing.Size(95, 27);
			this._ok.TabIndex = 0;
			this._ok.Text = "OK";
			this._ok.UseVisualStyleBackColor = true;
			this._ok.Click += new System.EventHandler(this.button1_Click);
			// 
			// _text
			// 
			this._text.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._text.AutoEllipsis = true;
			this._text.Location = new System.Drawing.Point(12, 9);
			this._text.Name = "_text";
			this._text.Size = new System.Drawing.Size(323, 40);
			this._text.TabIndex = 1;
			this._text.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// DismissableMessageBox
			// 
			this.AcceptButton = this._ok;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._ok;
			this.ClientSize = new System.Drawing.Size(347, 97);
			this.ControlBox = false;
			this.Controls.Add(this._text);
			this.Controls.Add(this._ok);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DismissableMessageBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this.Text = "Message Box";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.Label _text;
	}
}