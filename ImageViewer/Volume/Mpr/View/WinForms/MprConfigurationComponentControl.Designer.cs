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
			this._lblProportionalSliceSpacing = new System.Windows.Forms.Label();
			this._txtProportionalSliceSpacing = new System.Windows.Forms.TextBox();
			this._grpSliceSpacing = new System.Windows.Forms.GroupBox();
			this._radAutomaticSliceSpacing = new System.Windows.Forms.RadioButton();
			this._radProportionalSliceSpacing = new System.Windows.Forms.RadioButton();
			this._pnlProportionalSliceSpacing = new System.Windows.Forms.Panel();
			this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this._grpSliceSpacing.SuspendLayout();
			this._pnlProportionalSliceSpacing.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// _lblProportionalSliceSpacing
			// 
			this._lblProportionalSliceSpacing.AutoSize = true;
			this._lblProportionalSliceSpacing.Location = new System.Drawing.Point(60, 3);
			this._lblProportionalSliceSpacing.Name = "_lblProportionalSliceSpacing";
			this._lblProportionalSliceSpacing.Size = new System.Drawing.Size(95, 13);
			this._lblProportionalSliceSpacing.TabIndex = 0;
			this._lblProportionalSliceSpacing.Text = "times pixel spacing";
			// 
			// _txtProportionalSliceSpacing
			// 
			this._txtProportionalSliceSpacing.Location = new System.Drawing.Point(0, 0);
			this._txtProportionalSliceSpacing.Name = "_txtProportionalSliceSpacing";
			this._txtProportionalSliceSpacing.Size = new System.Drawing.Size(54, 20);
			this._txtProportionalSliceSpacing.TabIndex = 1;
			// 
			// _grpSliceSpacing
			// 
			this._grpSliceSpacing.Controls.Add(this._radAutomaticSliceSpacing);
			this._grpSliceSpacing.Controls.Add(this._radProportionalSliceSpacing);
			this._grpSliceSpacing.Controls.Add(this._pnlProportionalSliceSpacing);
			this._grpSliceSpacing.Location = new System.Drawing.Point(10, 10);
			this._grpSliceSpacing.Name = "_grpSliceSpacing";
			this._grpSliceSpacing.Size = new System.Drawing.Size(310, 112);
			this._grpSliceSpacing.TabIndex = 2;
			this._grpSliceSpacing.TabStop = false;
			this._grpSliceSpacing.Text = "Slice Spacing";
			// 
			// _radAutomaticSliceSpacing
			// 
			this._radAutomaticSliceSpacing.AutoSize = true;
			this._radAutomaticSliceSpacing.Location = new System.Drawing.Point(6, 19);
			this._radAutomaticSliceSpacing.Name = "_radAutomaticSliceSpacing";
			this._radAutomaticSliceSpacing.Size = new System.Drawing.Size(157, 17);
			this._radAutomaticSliceSpacing.TabIndex = 3;
			this._radAutomaticSliceSpacing.TabStop = true;
			this._radAutomaticSliceSpacing.Text = "Use automatic slice spacing";
			this._radAutomaticSliceSpacing.UseVisualStyleBackColor = true;
			// 
			// _radProportionalSliceSpacing
			// 
			this._radProportionalSliceSpacing.AutoSize = true;
			this._radProportionalSliceSpacing.Location = new System.Drawing.Point(6, 42);
			this._radProportionalSliceSpacing.Name = "_radProportionalSliceSpacing";
			this._radProportionalSliceSpacing.Size = new System.Drawing.Size(269, 17);
			this._radProportionalSliceSpacing.TabIndex = 4;
			this._radProportionalSliceSpacing.TabStop = true;
			this._radProportionalSliceSpacing.Text = "Use a slice spacing proportional to the pixel spacing";
			this._radProportionalSliceSpacing.UseVisualStyleBackColor = true;
			// 
			// _pnlProportionalSliceSpacing
			// 
			this._pnlProportionalSliceSpacing.AutoSize = true;
			this._pnlProportionalSliceSpacing.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._pnlProportionalSliceSpacing.Controls.Add(this._txtProportionalSliceSpacing);
			this._pnlProportionalSliceSpacing.Controls.Add(this._lblProportionalSliceSpacing);
			this._pnlProportionalSliceSpacing.Location = new System.Drawing.Point(23, 65);
			this._pnlProportionalSliceSpacing.Name = "_pnlProportionalSliceSpacing";
			this._pnlProportionalSliceSpacing.Size = new System.Drawing.Size(158, 23);
			this._pnlProportionalSliceSpacing.TabIndex = 5;
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
			this._pnlProportionalSliceSpacing.ResumeLayout(false);
			this._pnlProportionalSliceSpacing.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label _lblProportionalSliceSpacing;
		private System.Windows.Forms.TextBox _txtProportionalSliceSpacing;
		private System.Windows.Forms.GroupBox _grpSliceSpacing;
		private System.Windows.Forms.ErrorProvider _errorProvider;
		private System.Windows.Forms.RadioButton _radProportionalSliceSpacing;
		private System.Windows.Forms.RadioButton _radAutomaticSliceSpacing;
		private System.Windows.Forms.Panel _pnlProportionalSliceSpacing;
	}
}
