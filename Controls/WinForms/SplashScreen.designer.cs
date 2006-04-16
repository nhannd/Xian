namespace ClearCanvas.Controls.WinForms
{
	partial class SplashScreen
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
			this.m_Timer = new System.Windows.Forms.Timer(this.components);
			this.m_StatusLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// m_Timer
			// 
			this.m_Timer.Interval = 50;
			this.m_Timer.Tick += new System.EventHandler(this.m_Timer_Tick);
			// 
			// m_StatusLabel
			// 
			this.m_StatusLabel.AutoEllipsis = true;
			this.m_StatusLabel.BackColor = System.Drawing.Color.White;
			this.m_StatusLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.m_StatusLabel.Location = new System.Drawing.Point(295, 61);
			this.m_StatusLabel.Name = "m_StatusLabel";
			this.m_StatusLabel.Size = new System.Drawing.Size(354, 23);
			this.m_StatusLabel.TabIndex = 0;
			this.m_StatusLabel.Text = "Progress text";
			// 
			// SplashScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = new System.Drawing.Size(673, 385);
			this.ControlBox = false;
			this.Controls.Add(this.m_StatusLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "SplashScreen";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SplashScreen";
			this.TopMost = true;
			this.DoubleClick += new System.EventHandler(this.SplashScreen_DoubleClick);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer m_Timer;
		private System.Windows.Forms.Label m_StatusLabel;
	}
}