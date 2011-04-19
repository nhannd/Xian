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
			resources.ApplyResources(this._ok, "_ok");
			this._ok.Name = "_ok";
			this._ok.UseVisualStyleBackColor = true;
			this._ok.Click += new System.EventHandler(this.button1_Click);
			// 
			// _text
			// 
			resources.ApplyResources(this._text, "_text");
			this._text.AutoEllipsis = true;
			this._text.Name = "_text";
			// 
			// DismissableMessageBox
			// 
			this.AcceptButton = this._ok;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._ok;
			this.ControlBox = false;
			this.Controls.Add(this._text);
			this.Controls.Add(this._ok);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DismissableMessageBox";
			this.ShowInTaskbar = false;
			this.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.Label _text;
	}
}