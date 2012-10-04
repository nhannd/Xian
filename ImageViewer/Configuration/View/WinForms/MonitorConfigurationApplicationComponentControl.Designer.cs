namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	partial class MonitorConfigurationApplicationComponentControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._singleWindowRadio = new System.Windows.Forms.RadioButton();
			this._separateWindowRadio = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _singleWindowRadio
			// 
			this._singleWindowRadio.AutoSize = true;
			this._singleWindowRadio.Location = new System.Drawing.Point(20, 29);
			this._singleWindowRadio.Name = "_singleWindowRadio";
			this._singleWindowRadio.Size = new System.Drawing.Size(201, 17);
			this._singleWindowRadio.TabIndex = 0;
			this._singleWindowRadio.TabStop = true;
			this._singleWindowRadio.Text = "Always open images in single window";
			this._singleWindowRadio.UseVisualStyleBackColor = true;
			// 
			// _separateWindowRadio
			// 
			this._separateWindowRadio.AutoSize = true;
			this._separateWindowRadio.Location = new System.Drawing.Point(20, 52);
			this._separateWindowRadio.Name = "_separateWindowRadio";
			this._separateWindowRadio.Size = new System.Drawing.Size(215, 17);
			this._separateWindowRadio.TabIndex = 1;
			this._separateWindowRadio.TabStop = true;
			this._separateWindowRadio.Text = "Always open images in separate window";
			this._separateWindowRadio.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._singleWindowRadio);
			this.groupBox1.Controls.Add(this._separateWindowRadio);
			this.groupBox1.Location = new System.Drawing.Point(3, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(262, 89);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Desktop Window Behaviour";
			// 
			// MonitorConfigurationApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "MonitorConfigurationApplicationComponentControl";
			this.Size = new System.Drawing.Size(292, 132);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RadioButton _singleWindowRadio;
		private System.Windows.Forms.RadioButton _separateWindowRadio;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}