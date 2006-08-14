namespace ClearCanvas.ImageViewer.Dashboard.Local
{
	partial class MasterViewControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this._textBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this._textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._textBox1.Location = new System.Drawing.Point(11, 13);
			this._textBox1.Multiline = true;
			this._textBox1.Name = "textBox1";
			this._textBox1.ReadOnly = true;
			this._textBox1.Size = new System.Drawing.Size(154, 60);
			this._textBox1.TabIndex = 3;
			this._textBox1.Text = "Select the DICOM images you want to open, or the folders you want to open from, t" +
				"hen click Open.";
			// 
			// MasterViewControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._textBox1);
			this.Name = "MasterViewControl";
			this.Size = new System.Drawing.Size(183, 237);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _textBox1;


	}
}
