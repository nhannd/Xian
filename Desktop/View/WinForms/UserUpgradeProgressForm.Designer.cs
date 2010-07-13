namespace ClearCanvas.Desktop.View.WinForms
{
	partial class UserUpgradeProgressForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserUpgradeProgressForm));
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._message = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// _progressBar
			// 
			this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._progressBar.Location = new System.Drawing.Point(12, 63);
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(268, 23);
			this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._progressBar.TabIndex = 1;
			// 
			// _message
			// 
			this._message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._message.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._message.Location = new System.Drawing.Point(12, 12);
			this._message.Multiline = true;
			this._message.Name = "_message";
			this._message.ReadOnly = true;
			this._message.Size = new System.Drawing.Size(268, 28);
			this._message.TabIndex = 0;
			this._message.TabStop = false;
			// 
			// UserUpgradeProgressForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 108);
			this.ControlBox = false;
			this.Controls.Add(this._progressBar);
			this.Controls.Add(this._message);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UserUpgradeProgressForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this.Text = "UserUpgradeProgressForm";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar _progressBar;
		private System.Windows.Forms.TextBox _message;
	}
}