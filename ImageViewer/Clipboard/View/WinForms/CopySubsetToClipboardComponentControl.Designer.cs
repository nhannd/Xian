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
    partial class CopySubsetToClipboardComponentControl
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
			this._radioCopyCustom = new System.Windows.Forms.RadioButton();
			this._radioCopyRange = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this._copyRangeStart = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._copyRangeEnd = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._customRange = new System.Windows.Forms.TextBox();
			this._radioCopyRangeAll = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this._radioCopyRangeAtInterval = new System.Windows.Forms.RadioButton();
			this._copyRangeInterval = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._sendToClipboardButton = new System.Windows.Forms.Button();
			this._betweenGroup = new System.Windows.Forms.GroupBox();
			this._rangeSelectionGroup = new System.Windows.Forms.GroupBox();
			this._radioUsePositionNumber = new System.Windows.Forms.RadioButton();
			this._radioUseInstanceNumber = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this._sourceDisplaySet = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._copyRangeStart)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._copyRangeEnd)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._copyRangeInterval)).BeginInit();
			this._betweenGroup.SuspendLayout();
			this._rangeSelectionGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// _radioCopyCustom
			// 
			this._radioCopyCustom.AutoSize = true;
			this._radioCopyCustom.Location = new System.Drawing.Point(14, 249);
			this._radioCopyCustom.Name = "_radioCopyCustom";
			this._radioCopyCustom.Size = new System.Drawing.Size(60, 17);
			this._radioCopyCustom.TabIndex = 12;
			this._radioCopyCustom.Text = "Custom";
			this._radioCopyCustom.UseVisualStyleBackColor = true;
			// 
			// _radioCopyRange
			// 
			this._radioCopyRange.AutoSize = true;
			this._radioCopyRange.Location = new System.Drawing.Point(14, 124);
			this._radioCopyRange.Name = "_radioCopyRange";
			this._radioCopyRange.Size = new System.Drawing.Size(67, 17);
			this._radioCopyRange.TabIndex = 3;
			this._radioCopyRange.Text = "Between";
			this._radioCopyRange.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(181, 126);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(25, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "and";
			// 
			// _copyRangeStart
			// 
			this._copyRangeStart.Location = new System.Drawing.Point(90, 123);
			this._copyRangeStart.Name = "_copyRangeStart";
			this._copyRangeStart.Size = new System.Drawing.Size(85, 20);
			this._copyRangeStart.TabIndex = 4;
			// 
			// _copyRangeEnd
			// 
			this._copyRangeEnd.Location = new System.Drawing.Point(211, 123);
			this._copyRangeEnd.Name = "_copyRangeEnd";
			this._copyRangeEnd.Size = new System.Drawing.Size(85, 20);
			this._copyRangeEnd.TabIndex = 6;
			// 
			// _customRange
			// 
			this._customRange.Location = new System.Drawing.Point(90, 248);
			this._customRange.Name = "_customRange";
			this._customRange.Size = new System.Drawing.Size(255, 20);
			this._customRange.TabIndex = 13;
			// 
			// _radioCopyRangeAll
			// 
			this._radioCopyRangeAll.AutoSize = true;
			this._radioCopyRangeAll.Location = new System.Drawing.Point(8, 19);
			this._radioCopyRangeAll.Name = "_radioCopyRangeAll";
			this._radioCopyRangeAll.Size = new System.Drawing.Size(98, 17);
			this._radioCopyRangeAll.TabIndex = 8;
			this._radioCopyRangeAll.Text = "Copy all images";
			this._radioCopyRangeAll.UseVisualStyleBackColor = true;
			this._radioCopyRangeAll.CheckedChanged += new System.EventHandler(this.OnRangeCopyAllImagesCheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(209, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 13);
			this.label1.TabIndex = 11;
			this.label1.Text = "images";
			// 
			// _radioCopyRangeAtInterval
			// 
			this._radioCopyRangeAtInterval.AutoSize = true;
			this._radioCopyRangeAtInterval.Location = new System.Drawing.Point(8, 47);
			this._radioCopyRangeAtInterval.Name = "_radioCopyRangeAtInterval";
			this._radioCopyRangeAtInterval.Size = new System.Drawing.Size(110, 17);
			this._radioCopyRangeAtInterval.TabIndex = 9;
			this._radioCopyRangeAtInterval.Text = "Copy at interval of";
			this._radioCopyRangeAtInterval.UseVisualStyleBackColor = true;
			this._radioCopyRangeAtInterval.CheckedChanged += new System.EventHandler(this.OnRangeCopyAtIntervalCheckedChanged);
			// 
			// _copyRangeInterval
			// 
			this._copyRangeInterval.Location = new System.Drawing.Point(121, 46);
			this._copyRangeInterval.Name = "_copyRangeInterval";
			this._copyRangeInterval.Size = new System.Drawing.Size(85, 20);
			this._copyRangeInterval.TabIndex = 10;
			// 
			// _sendToClipboardButton
			// 
			this._sendToClipboardButton.Location = new System.Drawing.Point(235, 286);
			this._sendToClipboardButton.Name = "_sendToClipboardButton";
			this._sendToClipboardButton.Size = new System.Drawing.Size(110, 23);
			this._sendToClipboardButton.TabIndex = 14;
			this._sendToClipboardButton.Text = "Send To Clipboard";
			this._sendToClipboardButton.UseVisualStyleBackColor = true;
			this._sendToClipboardButton.Click += new System.EventHandler(this.OnSendToClipboard);
			// 
			// _betweenGroup
			// 
			this._betweenGroup.Controls.Add(this._radioCopyRangeAll);
			this._betweenGroup.Controls.Add(this._copyRangeInterval);
			this._betweenGroup.Controls.Add(this.label1);
			this._betweenGroup.Controls.Add(this._radioCopyRangeAtInterval);
			this._betweenGroup.Location = new System.Drawing.Point(90, 149);
			this._betweenGroup.Name = "_betweenGroup";
			this._betweenGroup.Size = new System.Drawing.Size(255, 82);
			this._betweenGroup.TabIndex = 7;
			this._betweenGroup.TabStop = false;
			// 
			// _rangeSelectionGroup
			// 
			this._rangeSelectionGroup.Controls.Add(this._radioUsePositionNumber);
			this._rangeSelectionGroup.Controls.Add(this._radioUseInstanceNumber);
			this._rangeSelectionGroup.Location = new System.Drawing.Point(90, 55);
			this._rangeSelectionGroup.Name = "_rangeSelectionGroup";
			this._rangeSelectionGroup.Size = new System.Drawing.Size(255, 51);
			this._rangeSelectionGroup.TabIndex = 0;
			this._rangeSelectionGroup.TabStop = false;
			this._rangeSelectionGroup.Text = "Use";
			// 
			// _radioUsePositionNumber
			// 
			this._radioUsePositionNumber.AutoSize = true;
			this._radioUsePositionNumber.Location = new System.Drawing.Point(121, 19);
			this._radioUsePositionNumber.Name = "_radioUsePositionNumber";
			this._radioUsePositionNumber.Size = new System.Drawing.Size(62, 17);
			this._radioUsePositionNumber.TabIndex = 2;
			this._radioUsePositionNumber.Text = "Position";
			this._radioUsePositionNumber.UseVisualStyleBackColor = true;
			// 
			// _radioUseInstanceNumber
			// 
			this._radioUseInstanceNumber.AutoSize = true;
			this._radioUseInstanceNumber.Location = new System.Drawing.Point(6, 19);
			this._radioUseInstanceNumber.Name = "_radioUseInstanceNumber";
			this._radioUseInstanceNumber.Size = new System.Drawing.Size(94, 17);
			this._radioUseInstanceNumber.TabIndex = 1;
			this._radioUseInstanceNumber.Text = "Image Number";
			this._radioUseInstanceNumber.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(37, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(44, 13);
			this.label3.TabIndex = 15;
			this.label3.Text = "Source:";
			// 
			// _sourceDisplaySet
			// 
			this._sourceDisplaySet.AutoEllipsis = true;
			this._sourceDisplaySet.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._sourceDisplaySet.Location = new System.Drawing.Point(90, 16);
			this._sourceDisplaySet.Name = "_sourceDisplaySet";
			this._sourceDisplaySet.Size = new System.Drawing.Size(255, 24);
			this._sourceDisplaySet.TabIndex = 16;
			this._sourceDisplaySet.Text = "<source display set>";
			this._sourceDisplaySet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// CopySubsetToClipboardComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._sourceDisplaySet);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._rangeSelectionGroup);
			this.Controls.Add(this._sendToClipboardButton);
			this.Controls.Add(this._customRange);
			this.Controls.Add(this._copyRangeEnd);
			this.Controls.Add(this._copyRangeStart);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._radioCopyRange);
			this.Controls.Add(this._radioCopyCustom);
			this.Controls.Add(this._betweenGroup);
			this.Name = "CopySubsetToClipboardComponentControl";
			this.Size = new System.Drawing.Size(368, 333);
			((System.ComponentModel.ISupportInitialize)(this._copyRangeStart)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._copyRangeEnd)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._copyRangeInterval)).EndInit();
			this._betweenGroup.ResumeLayout(false);
			this._betweenGroup.PerformLayout();
			this._rangeSelectionGroup.ResumeLayout(false);
			this._rangeSelectionGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.RadioButton _radioCopyCustom;
		private System.Windows.Forms.RadioButton _radioCopyRange;
		private System.Windows.Forms.Label label2;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _copyRangeStart;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _copyRangeEnd;
		private System.Windows.Forms.TextBox _customRange;
		private System.Windows.Forms.RadioButton _radioCopyRangeAll;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton _radioCopyRangeAtInterval;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _copyRangeInterval;
		private System.Windows.Forms.Button _sendToClipboardButton;
		private System.Windows.Forms.GroupBox _betweenGroup;
		private System.Windows.Forms.GroupBox _rangeSelectionGroup;
		private System.Windows.Forms.RadioButton _radioUsePositionNumber;
		private System.Windows.Forms.RadioButton _radioUseInstanceNumber;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label _sourceDisplaySet;
    }
}
