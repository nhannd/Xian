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
			this._components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
			this._timer = new System.Windows.Forms.Timer(this._components);
			this._statusLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _timer
			// 
			this._timer.Interval = 50;
			this._timer.Tick += new System.EventHandler(this._Timer_Tick);
			// 
			// _statusLabel
			// 
			this._statusLabel.AutoEllipsis = true;
			this._statusLabel.BackColor = System.Drawing.Color.White;
			this._statusLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._statusLabel.Location = new System.Drawing.Point(295, 61);
			this._statusLabel.Name = "_StatusLabel";
			this._statusLabel.Size = new System.Drawing.Size(354, 23);
			this._statusLabel.TabIndex = 0;
			this._statusLabel.Text = "Progress text";
			// 
			// SplashScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = new System.Drawing.Size(673, 385);
			this.ControlBox = false;
			this.Controls.Add(this._statusLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "SplashScreen";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SplashScreen";
			this.TopMost = true;
			this.DoubleClick += new System.EventHandler(this.SplashScreen_DoubleClick);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer _timer;
		private System.Windows.Forms.Label _statusLabel;
	}
}