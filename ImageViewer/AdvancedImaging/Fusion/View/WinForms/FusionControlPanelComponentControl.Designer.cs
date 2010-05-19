namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.View.WinForms {
	partial class FusionControlPanelComponentControl {
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._sliderAlpha = new System.Windows.Forms.TrackBar();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._sliderThreshold = new System.Windows.Forms.TrackBar();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._sliderAlpha)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._sliderThreshold)).BeginInit();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._sliderAlpha);
			this.groupBox1.Location = new System.Drawing.Point(19, 121);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(216, 121);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Alpha";
			// 
			// _sliderAlpha
			// 
			this._sliderAlpha.Location = new System.Drawing.Point(26, 36);
			this._sliderAlpha.Maximum = 1000;
			this._sliderAlpha.Name = "_sliderAlpha";
			this._sliderAlpha.Size = new System.Drawing.Size(161, 42);
			this._sliderAlpha.TabIndex = 1;
			this._sliderAlpha.TickFrequency = 100;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._sliderThreshold);
			this.groupBox2.Location = new System.Drawing.Point(19, 248);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(216, 121);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Threshold";
			this.groupBox2.Visible = false;
			// 
			// _sliderThreshold
			// 
			this._sliderThreshold.Location = new System.Drawing.Point(26, 36);
			this._sliderThreshold.Maximum = 1000;
			this._sliderThreshold.Name = "_sliderThreshold";
			this._sliderThreshold.Size = new System.Drawing.Size(161, 42);
			this._sliderThreshold.TabIndex = 1;
			this._sliderThreshold.TickFrequency = 100;
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Checked = true;
			this.radioButton1.Location = new System.Drawing.Point(16, 33);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(49, 17);
			this.radioButton1.TabIndex = 2;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "Base";
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Location = new System.Drawing.Point(16, 56);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(61, 17);
			this.radioButton2.TabIndex = 3;
			this.radioButton2.TabStop = true;
			this.radioButton2.Text = "Overlay";
			this.radioButton2.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.radioButton1);
			this.groupBox3.Controls.Add(this.radioButton2);
			this.groupBox3.Location = new System.Drawing.Point(19, 23);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(216, 92);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Active Layer";
			// 
			// FusionControlPanelComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "FusionControlPanelComponentControl";
			this.Size = new System.Drawing.Size(259, 556);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._sliderAlpha)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._sliderThreshold)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TrackBar _sliderAlpha;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TrackBar _sliderThreshold;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.GroupBox groupBox3;
	}
}
