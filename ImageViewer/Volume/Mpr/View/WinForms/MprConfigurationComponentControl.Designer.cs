namespace ClearCanvas.ImageViewer.Volume.Mpr.View.WinForms {
	partial class MprConfigurationComponentControl {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this._lblSliceSpacing = new System.Windows.Forms.Label();
			this._txtSliceSpacing = new System.Windows.Forms.TextBox();
			this._grpSliceSpacing = new System.Windows.Forms.GroupBox();
			this._chkAutoSliceSpacing = new System.Windows.Forms.CheckBox();
			this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this._grpSliceSpacing.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// _lblSliceSpacing
			// 
			this._lblSliceSpacing.AutoSize = true;
			this._lblSliceSpacing.Location = new System.Drawing.Point(23, 41);
			this._lblSliceSpacing.Name = "_lblSliceSpacing";
			this._lblSliceSpacing.Size = new System.Drawing.Size(211, 13);
			this._lblSliceSpacing.TabIndex = 0;
			this._lblSliceSpacing.Text = "Slice Spacing (in multiples of Pixel Spacing)";
			// 
			// _txtSliceSpacing
			// 
			this._txtSliceSpacing.Location = new System.Drawing.Point(26, 57);
			this._txtSliceSpacing.Name = "_txtSliceSpacing";
			this._txtSliceSpacing.Size = new System.Drawing.Size(83, 20);
			this._txtSliceSpacing.TabIndex = 1;
			// 
			// _grpSliceSpacing
			// 
			this._grpSliceSpacing.Controls.Add(this._chkAutoSliceSpacing);
			this._grpSliceSpacing.Controls.Add(this._lblSliceSpacing);
			this._grpSliceSpacing.Controls.Add(this._txtSliceSpacing);
			this._grpSliceSpacing.Dock = System.Windows.Forms.DockStyle.Top;
			this._grpSliceSpacing.Location = new System.Drawing.Point(10, 10);
			this._grpSliceSpacing.Name = "_grpSliceSpacing";
			this._grpSliceSpacing.Size = new System.Drawing.Size(427, 99);
			this._grpSliceSpacing.TabIndex = 2;
			this._grpSliceSpacing.TabStop = false;
			this._grpSliceSpacing.Text = "Slice Spacing";
			// 
			// _chkAutoSliceSpacing
			// 
			this._chkAutoSliceSpacing.AutoSize = true;
			this._chkAutoSliceSpacing.Location = new System.Drawing.Point(6, 19);
			this._chkAutoSliceSpacing.Name = "_chkAutoSliceSpacing";
			this._chkAutoSliceSpacing.Size = new System.Drawing.Size(240, 17);
			this._chkAutoSliceSpacing.TabIndex = 2;
			this._chkAutoSliceSpacing.Text = "Automatically determine suitable slice spacing";
			this._chkAutoSliceSpacing.UseVisualStyleBackColor = true;
			this._chkAutoSliceSpacing.CheckedChanged += new System.EventHandler(this._chkAutoSliceSpacing_CheckedChanged);
			// 
			// _errorProvider
			// 
			this._errorProvider.ContainerControl = this;
			// 
			// MprConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._grpSliceSpacing);
			this.Name = "MprConfigurationComponentControl";
			this.Padding = new System.Windows.Forms.Padding(10);
			this.Size = new System.Drawing.Size(447, 256);
			this._grpSliceSpacing.ResumeLayout(false);
			this._grpSliceSpacing.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label _lblSliceSpacing;
		private System.Windows.Forms.TextBox _txtSliceSpacing;
		private System.Windows.Forms.GroupBox _grpSliceSpacing;
		private System.Windows.Forms.CheckBox _chkAutoSliceSpacing;
		private System.Windows.Forms.ErrorProvider _errorProvider;
	}
}
