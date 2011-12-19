namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
	partial class ReindexStartupDialogForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReindexStartupDialogForm));
			this._spinner = new System.Windows.Forms.ProgressBar();
			this._message = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _spinner
			// 
			resources.ApplyResources(this._spinner, "_spinner");
			this._spinner.Name = "_spinner";
			this._spinner.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			// 
			// _message
			// 
			resources.ApplyResources(this._message, "_message");
			this._message.Name = "_message";
			// 
			// ReindexStartupDialogForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this._message);
			this.Controls.Add(this._spinner);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ReindexStartupDialogForm";
			this.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar _spinner;
		private System.Windows.Forms.Label _message;
	}
}