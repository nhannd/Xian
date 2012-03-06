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

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiskspaceManagerConfigurationComponentControl));
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
			this._studyCount = new System.Windows.Forms.TextBox();
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
			resources.ApplyResources(this._tbLowWatermark, "_tbLowWatermark");
			this._tbLowWatermark.Maximum = 100000;
			this._tbLowWatermark.Name = "_tbLowWatermark";
			this._tbLowWatermark.SmallChange = 10;
			this._tbLowWatermark.TickFrequency = 10000;
			this._tbLowWatermark.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			// 
			// _tbHighWatermark
			// 
			this._tbHighWatermark.LargeChange = 10000;
			resources.ApplyResources(this._tbHighWatermark, "_tbHighWatermark");
			this._tbHighWatermark.Maximum = 100000;
			this._tbHighWatermark.Name = "_tbHighWatermark";
			this._tbHighWatermark.SmallChange = 10;
			this._tbHighWatermark.TickFrequency = 10000;
			// 
			// _pbUsedSpace
			// 
			resources.ApplyResources(this._pbUsedSpace, "_pbUsedSpace");
			this._pbUsedSpace.Name = "_pbUsedSpace";
			// 
			// _txtUsedSpace
			// 
			resources.ApplyResources(this._txtUsedSpace, "_txtUsedSpace");
			this._txtUsedSpace.Name = "_txtUsedSpace";
			this._txtUsedSpace.ReadOnly = true;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.MaximumSize = new System.Drawing.Size(80, 26);
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.MaximumSize = new System.Drawing.Size(45, 0);
			this.label2.Name = "label2";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.MaximumSize = new System.Drawing.Size(80, 26);
			this.label3.Name = "label3";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
			// 
			// label9
			// 
			resources.ApplyResources(this.label9, "label9");
			this.label9.MaximumSize = new System.Drawing.Size(60, 0);
			this.label9.Name = "label9";
			// 
			// _tbFrequency
			// 
			this._tbFrequency.LargeChange = 1;
			resources.ApplyResources(this._tbFrequency, "_tbFrequency");
			this._tbFrequency.Minimum = 1;
			this._tbFrequency.Name = "_tbFrequency";
			this._tbFrequency.Value = 1;
			// 
			// _txtFrequency
			// 
			resources.ApplyResources(this._txtFrequency, "_txtFrequency");
			this._txtFrequency.Name = "_txtFrequency";
			this._txtFrequency.ReadOnly = true;
			// 
			// label10
			// 
			resources.ApplyResources(this.label10, "label10");
			this.label10.Name = "label10";
			// 
			// _bnRefresh
			// 
			resources.ApplyResources(this._bnRefresh, "_bnRefresh");
			this._bnRefresh.Name = "_bnRefresh";
			this._bnRefresh.UseVisualStyleBackColor = true;
			// 
			// _txtDriveName
			// 
			resources.ApplyResources(this._txtDriveName, "_txtDriveName");
			this._txtDriveName.Name = "_txtDriveName";
			this._txtDriveName.ReadOnly = true;
			// 
			// _txtHighWatermarkBytesDisplay
			// 
			resources.ApplyResources(this._txtHighWatermarkBytesDisplay, "_txtHighWatermarkBytesDisplay");
			this._txtHighWatermarkBytesDisplay.Name = "_txtHighWatermarkBytesDisplay";
			this._txtHighWatermarkBytesDisplay.ReadOnly = true;
			// 
			// _txtUsedSpaceBytesDisplay
			// 
			resources.ApplyResources(this._txtUsedSpaceBytesDisplay, "_txtUsedSpaceBytesDisplay");
			this._txtUsedSpaceBytesDisplay.Name = "_txtUsedSpaceBytesDisplay";
			this._txtUsedSpaceBytesDisplay.ReadOnly = true;
			// 
			// _txtLowWatermarkBytesDisplay
			// 
			resources.ApplyResources(this._txtLowWatermarkBytesDisplay, "_txtLowWatermarkBytesDisplay");
			this._txtLowWatermarkBytesDisplay.Name = "_txtLowWatermarkBytesDisplay";
			this._txtLowWatermarkBytesDisplay.ReadOnly = true;
			// 
			// _upDownHighWatermark
			// 
			this._upDownHighWatermark.DecimalPlaces = 3;
			resources.ApplyResources(this._upDownHighWatermark, "_upDownHighWatermark");
			this._upDownHighWatermark.Name = "_upDownHighWatermark";
			// 
			// _upDownLowWatermark
			// 
			this._upDownLowWatermark.DecimalPlaces = 3;
			resources.ApplyResources(this._upDownLowWatermark, "_upDownLowWatermark");
			this._upDownLowWatermark.Name = "_upDownLowWatermark";
			// 
			// _studyLimitGroup
			// 
			this._studyLimitGroup.Controls.Add(this.label7);
			this._studyLimitGroup.Controls.Add(this._studyLimit);
			this._studyLimitGroup.Controls.Add(this._studyCount);
			this._studyLimitGroup.Controls.Add(this._enforceStudyLimit);
			resources.ApplyResources(this._studyLimitGroup, "_studyLimitGroup");
			this._studyLimitGroup.Name = "_studyLimitGroup";
			this._studyLimitGroup.TabStop = false;
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// _studyLimit
			// 
			resources.ApplyResources(this._studyLimit, "_studyLimit");
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
			this._studyLimit.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// _studyCount
			// 
			resources.ApplyResources(this._studyCount, "_studyCount");
			this._studyCount.Name = "_studyCount";
			this._studyCount.ReadOnly = true;
			// 
			// _enforceStudyLimit
			// 
			resources.ApplyResources(this._enforceStudyLimit, "_enforceStudyLimit");
			this._enforceStudyLimit.Name = "_enforceStudyLimit";
			this._enforceStudyLimit.UseVisualStyleBackColor = true;
			// 
			// DiskspaceManagerConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
			this.Name = "DiskspaceManagerConfigurationComponentControl";
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
		private System.Windows.Forms.TextBox _studyCount;
		private System.Windows.Forms.Label label7;
    }
}
