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
			this._grpSliceSpacing = new System.Windows.Forms.GroupBox();
			this._radAutomaticSliceSpacing = new System.Windows.Forms.RadioButton();
			this._radProportionalSliceSpacing = new System.Windows.Forms.RadioButton();
			this._txtProportionalSliceSpacing = new System.Windows.Forms.TextBox();
			this._lblProportionalSliceSpacing = new System.Windows.Forms.Label();
			this._grpSliceSpacing.SuspendLayout();
			this.SuspendLayout();
			// 
			// _grpSliceSpacing
			// 
			this._grpSliceSpacing.Controls.Add(this._txtProportionalSliceSpacing);
			this._grpSliceSpacing.Controls.Add(this._lblProportionalSliceSpacing);
			this._grpSliceSpacing.Controls.Add(this._radAutomaticSliceSpacing);
			this._grpSliceSpacing.Controls.Add(this._radProportionalSliceSpacing);
			this._grpSliceSpacing.Location = new System.Drawing.Point(10, 10);
			this._grpSliceSpacing.Name = "_grpSliceSpacing";
			this._grpSliceSpacing.Size = new System.Drawing.Size(310, 99);
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
			// _txtProportionalSliceSpacing
			// 
			this._txtProportionalSliceSpacing.Location = new System.Drawing.Point(25, 65);
			this._txtProportionalSliceSpacing.Name = "_txtProportionalSliceSpacing";
			this._txtProportionalSliceSpacing.Size = new System.Drawing.Size(54, 20);
			this._txtProportionalSliceSpacing.TabIndex = 6;
			// 
			// _lblProportionalSliceSpacing
			// 
			this._lblProportionalSliceSpacing.AutoSize = true;
			this._lblProportionalSliceSpacing.Location = new System.Drawing.Point(85, 68);
			this._lblProportionalSliceSpacing.Name = "_lblProportionalSliceSpacing";
			this._lblProportionalSliceSpacing.Size = new System.Drawing.Size(95, 13);
			this._lblProportionalSliceSpacing.TabIndex = 5;
			this._lblProportionalSliceSpacing.Text = "times pixel spacing";
			// 
			// MprConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._grpSliceSpacing);
			this.Name = "MprConfigurationComponentControl";
			this.Padding = new System.Windows.Forms.Padding(10);
			this.Size = new System.Drawing.Size(334, 128);
			this._grpSliceSpacing.ResumeLayout(false);
			this._grpSliceSpacing.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox _grpSliceSpacing;
		private System.Windows.Forms.RadioButton _radProportionalSliceSpacing;
		private System.Windows.Forms.RadioButton _radAutomaticSliceSpacing;
		private System.Windows.Forms.TextBox _txtProportionalSliceSpacing;
		private System.Windows.Forms.Label _lblProportionalSliceSpacing;
	}
}
