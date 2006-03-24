namespace ClearCanvas.Controls
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
			this.m_StatusLabel = new System.Windows.Forms.Label();
			this.m_Timer = new System.Windows.Forms.Timer(this.components);
			this.m_DisclaimerText = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// m_StatusLabel
			// 
			this.m_StatusLabel.AutoEllipsis = true;
			this.m_StatusLabel.AutoSize = true;
			this.m_StatusLabel.BackColor = System.Drawing.SystemColors.Window;
			this.m_StatusLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.m_StatusLabel.ForeColor = System.Drawing.Color.Blue;
			this.m_StatusLabel.Location = new System.Drawing.Point(22, 211);
			this.m_StatusLabel.Name = "m_StatusLabel";
			this.m_StatusLabel.Size = new System.Drawing.Size(87, 13);
			this.m_StatusLabel.TabIndex = 0;
			this.m_StatusLabel.Text = "Progress window";
			// 
			// m_Timer
			// 
			this.m_Timer.Interval = 50;
			this.m_Timer.Tick += new System.EventHandler(this.m_Timer_Tick);
			// 
			// m_DisclaimerText
			// 
			this.m_DisclaimerText.BackColor = System.Drawing.SystemColors.Window;
			this.m_DisclaimerText.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.m_DisclaimerText.Location = new System.Drawing.Point(25, 245);
			this.m_DisclaimerText.Multiline = true;
			this.m_DisclaimerText.Name = "m_DisclaimerText";
			this.m_DisclaimerText.ReadOnly = true;
			this.m_DisclaimerText.Size = new System.Drawing.Size(457, 39);
			this.m_DisclaimerText.TabIndex = 1;
			this.m_DisclaimerText.TabStop = false;
			this.m_DisclaimerText.Text = "Copyright (C) 2006 ClearCanvas Inc.  All rights reserved.";
			// 
			// SplashScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = new System.Drawing.Size(609, 317);
			this.ControlBox = false;
			this.Controls.Add(this.m_DisclaimerText);
			this.Controls.Add(this.m_StatusLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "SplashScreen";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SplashScreen";
			this.TopMost = true;
			this.DoubleClick += new System.EventHandler(this.SplashScreen_DoubleClick);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label m_StatusLabel;
		private System.Windows.Forms.Timer m_Timer;
		private System.Windows.Forms.TextBox m_DisclaimerText;
	}
}