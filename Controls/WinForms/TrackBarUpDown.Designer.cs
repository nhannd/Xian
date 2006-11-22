namespace ClearCanvas.Controls.WinForms
{
	partial class TrackBarUpDown
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
			this._numericUpDown = new System.Windows.Forms.NumericUpDown();
			this._trackBar = new System.Windows.Forms.TrackBar();
			((System.ComponentModel.ISupportInitialize)(this._numericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._trackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// _numericUpDown
			// 
			this._numericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._numericUpDown.Location = new System.Drawing.Point(201, 3);
			this._numericUpDown.Name = "_numericUpDown";
			this._numericUpDown.Size = new System.Drawing.Size(76, 20);
			this._numericUpDown.TabIndex = 0;
			this._numericUpDown.Value = new decimal(new int[] {
            99,
            0,
            0,
            0});
			// 
			// _trackBar
			// 
			this._trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._trackBar.AutoSize = false;
			this._trackBar.Location = new System.Drawing.Point(0, 0);
			this._trackBar.Name = "_trackBar";
			this._trackBar.Size = new System.Drawing.Size(193, 56);
			this._trackBar.TabIndex = 1;
			// 
			// TrackBarUpDown
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._trackBar);
			this.Controls.Add(this._numericUpDown);
			this.Name = "TrackBarUpDown";
			this.Size = new System.Drawing.Size(280, 42);
			((System.ComponentModel.ISupportInitialize)(this._numericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._trackBar)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NumericUpDown _numericUpDown;
		private System.Windows.Forms.TrackBar _trackBar;
	}
}
