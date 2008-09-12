namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms {
	partial class StandardToolsConfigComponentControl {
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
			this.grpProbeTool = new System.Windows.Forms.GroupBox();
			this.chkShowCTRawPixelValue = new System.Windows.Forms.CheckBox();
			this.chkShowNonCTModPixelValue = new System.Windows.Forms.CheckBox();
			this.chkShowVOIPixelValue = new System.Windows.Forms.CheckBox();
			this.grpProbeTool.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpProbeTool
			// 
			this.grpProbeTool.Controls.Add(this.chkShowVOIPixelValue);
			this.grpProbeTool.Controls.Add(this.chkShowNonCTModPixelValue);
			this.grpProbeTool.Controls.Add(this.chkShowCTRawPixelValue);
			this.grpProbeTool.Dock = System.Windows.Forms.DockStyle.Top;
			this.grpProbeTool.Location = new System.Drawing.Point(0, 0);
			this.grpProbeTool.Name = "grpProbeTool";
			this.grpProbeTool.Size = new System.Drawing.Size(475, 94);
			this.grpProbeTool.TabIndex = 1;
			this.grpProbeTool.TabStop = false;
			this.grpProbeTool.Text = "Probe Tool";
			// 
			// chkShowCTRawPixelValue
			// 
			this.chkShowCTRawPixelValue.AutoSize = true;
			this.chkShowCTRawPixelValue.Dock = System.Windows.Forms.DockStyle.Top;
			this.chkShowCTRawPixelValue.Location = new System.Drawing.Point(3, 16);
			this.chkShowCTRawPixelValue.Name = "chkShowCTRawPixelValue";
			this.chkShowCTRawPixelValue.Size = new System.Drawing.Size(469, 17);
			this.chkShowCTRawPixelValue.TabIndex = 0;
			this.chkShowCTRawPixelValue.Text = "Show raw pixel value for CT images";
			this.chkShowCTRawPixelValue.UseVisualStyleBackColor = true;
			// 
			// chkShowNonCTModPixelValue
			// 
			this.chkShowNonCTModPixelValue.AutoSize = true;
			this.chkShowNonCTModPixelValue.Dock = System.Windows.Forms.DockStyle.Top;
			this.chkShowNonCTModPixelValue.Location = new System.Drawing.Point(3, 33);
			this.chkShowNonCTModPixelValue.Name = "chkShowNonCTModPixelValue";
			this.chkShowNonCTModPixelValue.Size = new System.Drawing.Size(469, 17);
			this.chkShowNonCTModPixelValue.TabIndex = 1;
			this.chkShowNonCTModPixelValue.Text = "Show modality pixel value for non-CT images";
			this.chkShowNonCTModPixelValue.UseVisualStyleBackColor = true;
			// 
			// chkShowVOIPixelValue
			// 
			this.chkShowVOIPixelValue.AutoSize = true;
			this.chkShowVOIPixelValue.Dock = System.Windows.Forms.DockStyle.Top;
			this.chkShowVOIPixelValue.Location = new System.Drawing.Point(3, 50);
			this.chkShowVOIPixelValue.Name = "chkShowVOIPixelValue";
			this.chkShowVOIPixelValue.Size = new System.Drawing.Size(469, 17);
			this.chkShowVOIPixelValue.TabIndex = 2;
			this.chkShowVOIPixelValue.Text = "Show VOI LUT pixel value";
			this.chkShowVOIPixelValue.UseVisualStyleBackColor = true;
			// 
			// StandardToolsConfigComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.grpProbeTool);
			this.Name = "StandardToolsConfigComponentControl";
			this.Size = new System.Drawing.Size(475, 296);
			this.grpProbeTool.ResumeLayout(false);
			this.grpProbeTool.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox grpProbeTool;
		private System.Windows.Forms.CheckBox chkShowVOIPixelValue;
		private System.Windows.Forms.CheckBox chkShowNonCTModPixelValue;
		private System.Windows.Forms.CheckBox chkShowCTRawPixelValue;
	}
}
