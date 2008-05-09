#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms
{
    partial class AviExportComponentControl
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
			this._buttonCancel = new System.Windows.Forms.Button();
			this._buttonOk = new System.Windows.Forms.Button();
			this._trackBarFrameRate = new System.Windows.Forms.TrackBar();
			this.label3 = new System.Windows.Forms.Label();
			this._frameRate = new System.Windows.Forms.TextBox();
			this._duration = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this._groupOptions = new System.Windows.Forms.GroupBox();
			this._scale = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this.scaleLabel = new System.Windows.Forms.Label();
			this._checkOptionCompleteImage = new System.Windows.Forms.RadioButton();
			this._checkOptionWysiwyg = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this._trackBarFrameRate)).BeginInit();
			this._groupOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._scale)).BeginInit();
			this.SuspendLayout();
			// 
			// _buttonCancel
			// 
			this._buttonCancel.Location = new System.Drawing.Point(259, 165);
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.Size = new System.Drawing.Size(70, 23);
			this._buttonCancel.TabIndex = 9;
			this._buttonCancel.Text = "Cancel";
			this._buttonCancel.UseVisualStyleBackColor = true;
			this._buttonCancel.Click += new System.EventHandler(this.OnCancel);
			// 
			// _buttonOk
			// 
			this._buttonOk.Location = new System.Drawing.Point(183, 165);
			this._buttonOk.Name = "_buttonOk";
			this._buttonOk.Size = new System.Drawing.Size(70, 23);
			this._buttonOk.TabIndex = 8;
			this._buttonOk.Text = "Ok";
			this._buttonOk.UseVisualStyleBackColor = true;
			this._buttonOk.Click += new System.EventHandler(this.OnOk);
			// 
			// _trackBarFrameRate
			// 
			this._trackBarFrameRate.LargeChange = 1;
			this._trackBarFrameRate.Location = new System.Drawing.Point(12, 32);
			this._trackBarFrameRate.Margin = new System.Windows.Forms.Padding(2);
			this._trackBarFrameRate.Maximum = 25;
			this._trackBarFrameRate.Minimum = 1;
			this._trackBarFrameRate.Name = "_trackBarFrameRate";
			this._trackBarFrameRate.Size = new System.Drawing.Size(189, 42);
			this._trackBarFrameRate.TabIndex = 1;
			this._trackBarFrameRate.Value = 1;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(11, 17);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(97, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Frames per second";
			// 
			// _frameRate
			// 
			this._frameRate.BackColor = System.Drawing.SystemColors.Control;
			this._frameRate.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._frameRate.Location = new System.Drawing.Point(206, 36);
			this._frameRate.Name = "_frameRate";
			this._frameRate.ReadOnly = true;
			this._frameRate.Size = new System.Drawing.Size(47, 13);
			this._frameRate.TabIndex = 2;
			this._frameRate.TabStop = false;
			this._frameRate.Text = "frames/sec";
			// 
			// _duration
			// 
			this._duration.BackColor = System.Drawing.SystemColors.Control;
			this._duration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._duration.Location = new System.Drawing.Point(282, 34);
			this._duration.Name = "_duration";
			this._duration.ReadOnly = true;
			this._duration.Size = new System.Drawing.Size(47, 20);
			this._duration.TabIndex = 4;
			this._duration.TabStop = false;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(261, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(73, 13);
			this.label5.TabIndex = 3;
			this.label5.Text = "Duration (sec)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// _groupOptions
			// 
			this._groupOptions.Controls.Add(this._scale);
			this._groupOptions.Controls.Add(this.scaleLabel);
			this._groupOptions.Controls.Add(this._checkOptionCompleteImage);
			this._groupOptions.Controls.Add(this._checkOptionWysiwyg);
			this._groupOptions.Location = new System.Drawing.Point(14, 74);
			this._groupOptions.Name = "_groupOptions";
			this._groupOptions.Size = new System.Drawing.Size(315, 72);
			this._groupOptions.TabIndex = 10;
			this._groupOptions.TabStop = false;
			this._groupOptions.Text = "Options";
			// 
			// _scale
			// 
			this._scale.DecimalPlaces = 1;
			this._scale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this._scale.Location = new System.Drawing.Point(222, 34);
			this._scale.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this._scale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this._scale.Name = "_scale";
			this._scale.Size = new System.Drawing.Size(72, 20);
			this._scale.TabIndex = 3;
			this._scale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// scaleLabel
			// 
			this.scaleLabel.AutoSize = true;
			this.scaleLabel.Location = new System.Drawing.Point(219, 18);
			this.scaleLabel.Name = "scaleLabel";
			this.scaleLabel.Size = new System.Drawing.Size(34, 13);
			this.scaleLabel.TabIndex = 2;
			this.scaleLabel.Text = "Scale";
			// 
			// _checkOptionCompleteImage
			// 
			this._checkOptionCompleteImage.AutoSize = true;
			this._checkOptionCompleteImage.Location = new System.Drawing.Point(105, 37);
			this._checkOptionCompleteImage.Name = "_checkOptionCompleteImage";
			this._checkOptionCompleteImage.Size = new System.Drawing.Size(101, 17);
			this._checkOptionCompleteImage.TabIndex = 1;
			this._checkOptionCompleteImage.Text = "Complete Image";
			this._checkOptionCompleteImage.UseVisualStyleBackColor = true;
			// 
			// _checkOptionWysiwyg
			// 
			this._checkOptionWysiwyg.AutoSize = true;
			this._checkOptionWysiwyg.Location = new System.Drawing.Point(16, 37);
			this._checkOptionWysiwyg.Name = "_checkOptionWysiwyg";
			this._checkOptionWysiwyg.Size = new System.Drawing.Size(67, 17);
			this._checkOptionWysiwyg.TabIndex = 0;
			this._checkOptionWysiwyg.Text = "Wysiwyg";
			this._checkOptionWysiwyg.UseVisualStyleBackColor = true;
			// 
			// AviExportComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._groupOptions);
			this.Controls.Add(this.label5);
			this.Controls.Add(this._duration);
			this.Controls.Add(this._frameRate);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._trackBarFrameRate);
			this.Controls.Add(this._buttonCancel);
			this.Controls.Add(this._buttonOk);
			this.Name = "AviExportComponentControl";
			this.Size = new System.Drawing.Size(349, 209);
			((System.ComponentModel.ISupportInitialize)(this._trackBarFrameRate)).EndInit();
			this._groupOptions.ResumeLayout(false);
			this._groupOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._scale)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.Button _buttonOk;
		private System.Windows.Forms.TrackBar _trackBarFrameRate;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _frameRate;
		private System.Windows.Forms.TextBox _duration;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox _groupOptions;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _scale;
		private System.Windows.Forms.Label scaleLabel;
		private System.Windows.Forms.RadioButton _checkOptionCompleteImage;
		private System.Windows.Forms.RadioButton _checkOptionWysiwyg;
    }
}
