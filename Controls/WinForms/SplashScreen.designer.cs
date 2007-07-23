namespace ClearCanvas.Controls.WinForms
{
	partial class SplashScreen
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
			this._status = new System.Windows.Forms.Label();
			this._versionLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _status
			// 
			this._status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._status.AutoEllipsis = true;
			this._status.BackColor = System.Drawing.Color.White;
			this._status.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._status.Location = new System.Drawing.Point(295, 61);
			this._status.Name = "_status";
			this._status.Size = new System.Drawing.Size(354, 23);
			this._status.TabIndex = 0;
			this._status.Text = "Progress text";
			// 
			// _versionLabel
			// 
			this._versionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._versionLabel.AutoSize = true;
			this._versionLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(152)))), ((int)(((byte)(209)))));
			this._versionLabel.ForeColor = System.Drawing.Color.White;
			this._versionLabel.Location = new System.Drawing.Point(447, 296);
			this._versionLabel.Name = "_versionLabel";
			this._versionLabel.Size = new System.Drawing.Size(42, 13);
			this._versionLabel.TabIndex = 1;
			this._versionLabel.Text = "Version";
			// 
			// label1
			// 
			this.label1.AutoEllipsis = true;
			this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(152)))), ((int)(((byte)(209)))));
			this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(429, 340);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(233, 23);
			this.label1.TabIndex = 2;
			this.label1.Text = "© 2007 ClearCanvas Inc. All rights reserved.";
			// 
			// SplashScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::ClearCanvas.Controls.WinForms.Properties.Resources.Splash;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ClientSize = new System.Drawing.Size(673, 385);
			this.ControlBox = false;
			this.Controls.Add(this.label1);
			this.Controls.Add(this._versionLabel);
			this.Controls.Add(this._status);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SplashScreen";
			this.Shown += new System.EventHandler(this.SplashScreen_Shown);
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ClearCanvas";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _status;
		private System.Windows.Forms.Label _versionLabel;
		private System.Windows.Forms.Label label1;
	}
}