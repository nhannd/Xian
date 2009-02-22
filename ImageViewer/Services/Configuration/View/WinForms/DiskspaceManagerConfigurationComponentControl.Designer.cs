#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.ImageViewer.Services.Configuration.View.WinForms
{
    partial class DiskspaceManagerConfigurationComponentControl
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
			this._tbLowWatermark = new System.Windows.Forms.TrackBar();
			this._tbHighWatermark = new System.Windows.Forms.TrackBar();
			this._pbUsedSpace = new System.Windows.Forms.ProgressBar();
			this._txtUsedSpace = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this._tbFrequency = new System.Windows.Forms.TrackBar();
			this._txtFrequency = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this._bnRefresh = new System.Windows.Forms.Button();
			this._txtDriveName = new System.Windows.Forms.TextBox();
			this._txtHighWatermarkBytesDisplay = new System.Windows.Forms.TextBox();
			this._txtUsedSpaceBytesDisplay = new System.Windows.Forms.TextBox();
			this._txtLowWatermarkBytesDisplay = new System.Windows.Forms.TextBox();
			this._upDownHighWatermark = new System.Windows.Forms.NumericUpDown();
			this._upDownLowWatermark = new System.Windows.Forms.NumericUpDown();
			this._studyLimitGroup = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this._studyLimit = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._numberOfStudies = new System.Windows.Forms.TextBox();
			this._enforceStudyLimit = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this._tbLowWatermark)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._tbHighWatermark)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._tbFrequency)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._upDownHighWatermark)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._upDownLowWatermark)).BeginInit();
			this._studyLimitGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._studyLimit)).BeginInit();
			this.SuspendLayout();
			// 
			// _tbLowWatermark
			// 
			this._tbLowWatermark.LargeChange = 10000;
			this._tbLowWatermark.Location = new System.Drawing.Point(71, 144);
			this._tbLowWatermark.Margin = new System.Windows.Forms.Padding(2);
			this._tbLowWatermark.Maximum = 100000;
			this._tbLowWatermark.Name = "_tbLowWatermark";
			this._tbLowWatermark.Size = new System.Drawing.Size(171, 45);
			this._tbLowWatermark.SmallChange = 1000;
			this._tbLowWatermark.TabIndex = 13;
			this._tbLowWatermark.TickFrequency = 10000;
			this._tbLowWatermark.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			// 
			// _tbHighWatermark
			// 
			this._tbHighWatermark.LargeChange = 10000;
			this._tbHighWatermark.Location = new System.Drawing.Point(71, 62);
			this._tbHighWatermark.Margin = new System.Windows.Forms.Padding(2);
			this._tbHighWatermark.Maximum = 100000;
			this._tbHighWatermark.Name = "_tbHighWatermark";
			this._tbHighWatermark.Size = new System.Drawing.Size(171, 45);
			this._tbHighWatermark.SmallChange = 1000;
			this._tbHighWatermark.TabIndex = 3;
			this._tbHighWatermark.TickFrequency = 10000;
			// 
			// _pbUsedSpace
			// 
			this._pbUsedSpace.Location = new System.Drawing.Point(78, 108);
			this._pbUsedSpace.Margin = new System.Windows.Forms.Padding(2);
			this._pbUsedSpace.Name = "_pbUsedSpace";
			this._pbUsedSpace.Size = new System.Drawing.Size(159, 19);
			this._pbUsedSpace.TabIndex = 8;
			// 
			// _txtUsedSpace
			// 
			this._txtUsedSpace.Location = new System.Drawing.Point(247, 108);
			this._txtUsedSpace.Margin = new System.Windows.Forms.Padding(2);
			this._txtUsedSpace.Name = "_txtUsedSpace";
			this._txtUsedSpace.ReadOnly = true;
			this._txtUsedSpace.Size = new System.Drawing.Size(65, 20);
			this._txtUsedSpace.TabIndex = 9;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(11, 63);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.MaximumSize = new System.Drawing.Size(60, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 26);
			this.label1.TabIndex = 2;
			this.label1.Text = "High Watermark";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(32, 104);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.MaximumSize = new System.Drawing.Size(45, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 26);
			this.label2.TabIndex = 7;
			this.label2.Text = "Used Space";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(11, 145);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.MaximumSize = new System.Drawing.Size(60, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 26);
			this.label3.TabIndex = 12;
			this.label3.Text = "Low Watermark";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(314, 69);
			this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(15, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "%";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(314, 111);
			this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(15, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "%";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(314, 151);
			this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(15, 13);
			this.label6.TabIndex = 15;
			this.label6.Text = "%";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(38, 39);
			this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(32, 13);
			this.label8.TabIndex = 0;
			this.label8.Text = "Drive";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(28, 184);
			this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label9.MaximumSize = new System.Drawing.Size(60, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(42, 26);
			this.label9.TabIndex = 17;
			this.label9.Text = "Check Interval";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _tbFrequency
			// 
			this._tbFrequency.LargeChange = 1;
			this._tbFrequency.Location = new System.Drawing.Point(71, 184);
			this._tbFrequency.Margin = new System.Windows.Forms.Padding(2);
			this._tbFrequency.Minimum = 1;
			this._tbFrequency.Name = "_tbFrequency";
			this._tbFrequency.Size = new System.Drawing.Size(171, 45);
			this._tbFrequency.TabIndex = 18;
			this._tbFrequency.Value = 1;
			// 
			// _txtFrequency
			// 
			this._txtFrequency.Location = new System.Drawing.Point(247, 188);
			this._txtFrequency.Margin = new System.Windows.Forms.Padding(2);
			this._txtFrequency.Name = "_txtFrequency";
			this._txtFrequency.ReadOnly = true;
			this._txtFrequency.Size = new System.Drawing.Size(65, 20);
			this._txtFrequency.TabIndex = 19;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(314, 191);
			this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(24, 13);
			this.label10.TabIndex = 20;
			this.label10.Text = "Min";
			// 
			// _bnRefresh
			// 
			this._bnRefresh.Location = new System.Drawing.Point(333, 244);
			this._bnRefresh.Margin = new System.Windows.Forms.Padding(2);
			this._bnRefresh.Name = "_bnRefresh";
			this._bnRefresh.Size = new System.Drawing.Size(68, 23);
			this._bnRefresh.TabIndex = 22;
			this._bnRefresh.Text = "Refresh";
			this._bnRefresh.UseVisualStyleBackColor = true;
			// 
			// _txtDriveName
			// 
			this._txtDriveName.Location = new System.Drawing.Point(78, 36);
			this._txtDriveName.Margin = new System.Windows.Forms.Padding(2);
			this._txtDriveName.Name = "_txtDriveName";
			this._txtDriveName.ReadOnly = true;
			this._txtDriveName.Size = new System.Drawing.Size(159, 20);
			this._txtDriveName.TabIndex = 1;
			// 
			// _txtHighWatermarkBytesDisplay
			// 
			this._txtHighWatermarkBytesDisplay.Location = new System.Drawing.Point(333, 67);
			this._txtHighWatermarkBytesDisplay.Margin = new System.Windows.Forms.Padding(2);
			this._txtHighWatermarkBytesDisplay.Name = "_txtHighWatermarkBytesDisplay";
			this._txtHighWatermarkBytesDisplay.ReadOnly = true;
			this._txtHighWatermarkBytesDisplay.Size = new System.Drawing.Size(68, 20);
			this._txtHighWatermarkBytesDisplay.TabIndex = 6;
			// 
			// _txtUsedSpaceBytesDisplay
			// 
			this._txtUsedSpaceBytesDisplay.Location = new System.Drawing.Point(333, 108);
			this._txtUsedSpaceBytesDisplay.Margin = new System.Windows.Forms.Padding(2);
			this._txtUsedSpaceBytesDisplay.Name = "_txtUsedSpaceBytesDisplay";
			this._txtUsedSpaceBytesDisplay.ReadOnly = true;
			this._txtUsedSpaceBytesDisplay.Size = new System.Drawing.Size(68, 20);
			this._txtUsedSpaceBytesDisplay.TabIndex = 11;
			// 
			// _txtLowWatermarkBytesDisplay
			// 
			this._txtLowWatermarkBytesDisplay.Location = new System.Drawing.Point(333, 149);
			this._txtLowWatermarkBytesDisplay.Margin = new System.Windows.Forms.Padding(2);
			this._txtLowWatermarkBytesDisplay.Name = "_txtLowWatermarkBytesDisplay";
			this._txtLowWatermarkBytesDisplay.ReadOnly = true;
			this._txtLowWatermarkBytesDisplay.Size = new System.Drawing.Size(68, 20);
			this._txtLowWatermarkBytesDisplay.TabIndex = 16;
			// 
			// _upDownHighWatermark
			// 
			this._upDownHighWatermark.DecimalPlaces = 3;
			this._upDownHighWatermark.Location = new System.Drawing.Point(247, 67);
			this._upDownHighWatermark.Name = "_upDownHighWatermark";
			this._upDownHighWatermark.Size = new System.Drawing.Size(65, 20);
			this._upDownHighWatermark.TabIndex = 4;
			// 
			// _upDownLowWatermark
			// 
			this._upDownLowWatermark.DecimalPlaces = 3;
			this._upDownLowWatermark.Location = new System.Drawing.Point(247, 149);
			this._upDownLowWatermark.Name = "_upDownLowWatermark";
			this._upDownLowWatermark.Size = new System.Drawing.Size(65, 20);
			this._upDownLowWatermark.TabIndex = 14;
			// 
			// _studyLimitGroup
			// 
			this._studyLimitGroup.Controls.Add(this.label7);
			this._studyLimitGroup.Controls.Add(this._studyLimit);
			this._studyLimitGroup.Controls.Add(this._numberOfStudies);
			this._studyLimitGroup.Controls.Add(this._enforceStudyLimit);
			this._studyLimitGroup.Location = new System.Drawing.Point(14, 228);
			this._studyLimitGroup.Name = "_studyLimitGroup";
			this._studyLimitGroup.Size = new System.Drawing.Size(314, 49);
			this._studyLimitGroup.TabIndex = 21;
			this._studyLimitGroup.TabStop = false;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(7, 20);
			this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(49, 13);
			this.label7.TabIndex = 0;
			this.label7.Text = "#Studies";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _studyLimit
			// 
			this._studyLimit.Location = new System.Drawing.Point(233, 17);
			this._studyLimit.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this._studyLimit.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this._studyLimit.Name = "_studyLimit";
			this._studyLimit.Size = new System.Drawing.Size(65, 20);
			this._studyLimit.TabIndex = 3;
			this._studyLimit.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// _numberOfStudies
			// 
			this._numberOfStudies.Location = new System.Drawing.Point(67, 18);
			this._numberOfStudies.Margin = new System.Windows.Forms.Padding(2);
			this._numberOfStudies.Name = "_numberOfStudies";
			this._numberOfStudies.ReadOnly = true;
			this._numberOfStudies.Size = new System.Drawing.Size(65, 20);
			this._numberOfStudies.TabIndex = 1;
			// 
			// _enforceStudyLimit
			// 
			this._enforceStudyLimit.AutoSize = true;
			this._enforceStudyLimit.Location = new System.Drawing.Point(166, 19);
			this._enforceStudyLimit.Name = "_enforceStudyLimit";
			this._enforceStudyLimit.Size = new System.Drawing.Size(62, 17);
			this._enforceStudyLimit.TabIndex = 20;
			this._enforceStudyLimit.Text = "Limit to:";
			this._enforceStudyLimit.UseVisualStyleBackColor = true;
			// 
			// DiskspaceManagerConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._studyLimitGroup);
			this.Controls.Add(this._upDownLowWatermark);
			this.Controls.Add(this._upDownHighWatermark);
			this.Controls.Add(this._txtLowWatermarkBytesDisplay);
			this.Controls.Add(this._txtUsedSpaceBytesDisplay);
			this.Controls.Add(this._txtHighWatermarkBytesDisplay);
			this.Controls.Add(this._txtDriveName);
			this.Controls.Add(this._bnRefresh);
			this.Controls.Add(this.label10);
			this.Controls.Add(this._txtFrequency);
			this.Controls.Add(this._tbFrequency);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._txtUsedSpace);
			this.Controls.Add(this._pbUsedSpace);
			this.Controls.Add(this._tbHighWatermark);
			this.Controls.Add(this._tbLowWatermark);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "DiskspaceManagerConfigurationComponentControl";
			this.Size = new System.Drawing.Size(418, 296);
			((System.ComponentModel.ISupportInitialize)(this._tbLowWatermark)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._tbHighWatermark)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._tbFrequency)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._upDownHighWatermark)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._upDownLowWatermark)).EndInit();
			this._studyLimitGroup.ResumeLayout(false);
			this._studyLimitGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._studyLimit)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar _tbLowWatermark;
        private System.Windows.Forms.TrackBar _tbHighWatermark;
		private System.Windows.Forms.ProgressBar _pbUsedSpace;
		private System.Windows.Forms.TextBox _txtUsedSpace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TrackBar _tbFrequency;
        private System.Windows.Forms.TextBox _txtFrequency;
        private System.Windows.Forms.Button _bnRefresh;
        private System.Windows.Forms.TextBox _txtDriveName;
		private System.Windows.Forms.TextBox _txtUsedSpaceBytesDisplay;
		private System.Windows.Forms.TextBox _txtHighWatermarkBytesDisplay;
		private System.Windows.Forms.TextBox _txtLowWatermarkBytesDisplay;
		private System.Windows.Forms.NumericUpDown _upDownHighWatermark;
		private System.Windows.Forms.NumericUpDown _upDownLowWatermark;
		private System.Windows.Forms.GroupBox _studyLimitGroup;
		private System.Windows.Forms.CheckBox _enforceStudyLimit;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _studyLimit;
		private System.Windows.Forms.TextBox _numberOfStudies;
		private System.Windows.Forms.Label label7;
    }
}
