namespace ClearCanvas.Desktop.View.WinForms
{
	partial class WorkspaceDialogBoxForm
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
			this._contentPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// _contentPanel
			// 
			this._contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._contentPanel.Location = new System.Drawing.Point(0, 0);
			this._contentPanel.Margin = new System.Windows.Forms.Padding(2);
			this._contentPanel.Name = "_contentPanel";
			this._contentPanel.Size = new System.Drawing.Size(292, 266);
			this._contentPanel.TabIndex = 1;
			// 
			// WorkspaceDialogBoxForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this._contentPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WorkspaceDialogBoxForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "WorkspaceDialogBoxForm";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel _contentPanel;
	}
}