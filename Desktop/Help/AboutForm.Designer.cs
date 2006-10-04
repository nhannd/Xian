namespace ClearCanvas.Desktop.Help
{
	partial class AboutForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer _components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (_components != null))
			{
				_components.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
			this._closeButton = new System.Windows.Forms.LinkLabel();
			this._versionLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _closeButton
			// 
			this._closeButton.AutoSize = true;
			this._closeButton.BackColor = System.Drawing.Color.White;
			this._closeButton.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(150)))), ((int)(((byte)(208)))));
			this._closeButton.Location = new System.Drawing.Point(626, 9);
			this._closeButton.Name = "_closeButton";
			this._closeButton.Size = new System.Drawing.Size(33, 13);
			this._closeButton.TabIndex = 0;
			this._closeButton.TabStop = true;
			this._closeButton.Text = "Close";
			// 
			// _versionLabel
			// 
			this._versionLabel.AutoSize = true;
			this._versionLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(152)))), ((int)(((byte)(209)))));
			this._versionLabel.ForeColor = System.Drawing.Color.White;
			this._versionLabel.Location = new System.Drawing.Point(447, 296);
			this._versionLabel.Name = "_versionLabel";
			this._versionLabel.Size = new System.Drawing.Size(42, 13);
			this._versionLabel.TabIndex = 2;
			this._versionLabel.Text = "Version";
			// 
			// AboutForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.CancelButton = this._closeButton;
			this.ClientSize = new System.Drawing.Size(673, 385);
			this.ControlBox = false;
			this.Controls.Add(this._versionLabel);
			this.Controls.Add(this._closeButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "AboutForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.LinkLabel _closeButton;
		private System.Windows.Forms.Label _versionLabel;
	}
}