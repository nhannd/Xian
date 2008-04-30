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
			this._radioCustom = new System.Windows.Forms.RadioButton();
			this._radioBetween = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this._betweenFirstValue = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._betweenSecondValue = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._custom = new System.Windows.Forms.TextBox();
			this._radioCopyAllImages = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this._radioCopyAtInterval = new System.Windows.Forms.RadioButton();
			this._copyInterval = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._sendToClipboardButton = new System.Windows.Forms.Button();
			this._betweenGroup = new System.Windows.Forms.GroupBox();
			this._rangeSelectionGroup = new System.Windows.Forms.GroupBox();
			this._radioUseInstanceNumber = new System.Windows.Forms.RadioButton();
			this._radioUsePosition = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this._betweenFirstValue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._betweenSecondValue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._copyInterval)).BeginInit();
			this._betweenGroup.SuspendLayout();
			this._rangeSelectionGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// _radioCustom
			// 
			this._radioCustom.AutoSize = true;
			this._radioCustom.Location = new System.Drawing.Point(17, 207);
			this._radioCustom.Name = "_radioCustom";
			this._radioCustom.Size = new System.Drawing.Size(60, 17);
			this._radioCustom.TabIndex = 12;
			this._radioCustom.Text = "Custom";
			this._radioCustom.UseVisualStyleBackColor = true;
			// 
			// _radioBetween
			// 
			this._radioBetween.AutoSize = true;
			this._radioBetween.Location = new System.Drawing.Point(17, 82);
			this._radioBetween.Name = "_radioBetween";
			this._radioBetween.Size = new System.Drawing.Size(67, 17);
			this._radioBetween.TabIndex = 3;
			this._radioBetween.Text = "Between";
			this._radioBetween.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(184, 84);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(25, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "and";
			// 
			// _betweenFirstValue
			// 
			this._betweenFirstValue.Location = new System.Drawing.Point(93, 81);
			this._betweenFirstValue.Name = "_betweenFirstValue";
			this._betweenFirstValue.Size = new System.Drawing.Size(85, 20);
			this._betweenFirstValue.TabIndex = 4;
			// 
			// _betweenSecondValue
			// 
			this._betweenSecondValue.Location = new System.Drawing.Point(214, 81);
			this._betweenSecondValue.Name = "_betweenSecondValue";
			this._betweenSecondValue.Size = new System.Drawing.Size(85, 20);
			this._betweenSecondValue.TabIndex = 6;
			// 
			// _custom
			// 
			this._custom.Location = new System.Drawing.Point(93, 206);
			this._custom.Name = "_custom";
			this._custom.Size = new System.Drawing.Size(255, 20);
			this._custom.TabIndex = 13;
			// 
			// _radioCopyAllImages
			// 
			this._radioCopyAllImages.AutoSize = true;
			this._radioCopyAllImages.Location = new System.Drawing.Point(8, 19);
			this._radioCopyAllImages.Name = "_radioCopyAllImages";
			this._radioCopyAllImages.Size = new System.Drawing.Size(98, 17);
			this._radioCopyAllImages.TabIndex = 8;
			this._radioCopyAllImages.Text = "Copy all images";
			this._radioCopyAllImages.UseVisualStyleBackColor = true;
			this._radioCopyAllImages.CheckedChanged += new System.EventHandler(this.OnRangeCopyAllImagesCheckedChanged);
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
			// _radioCopyAtInterval
			// 
			this._radioCopyAtInterval.AutoSize = true;
			this._radioCopyAtInterval.Location = new System.Drawing.Point(8, 47);
			this._radioCopyAtInterval.Name = "_radioCopyAtInterval";
			this._radioCopyAtInterval.Size = new System.Drawing.Size(110, 17);
			this._radioCopyAtInterval.TabIndex = 9;
			this._radioCopyAtInterval.Text = "Copy at interval of";
			this._radioCopyAtInterval.UseVisualStyleBackColor = true;
			this._radioCopyAtInterval.CheckedChanged += new System.EventHandler(this.OnRangeCopyAtIntervalCheckedChanged);
			// 
			// _copyInterval
			// 
			this._copyInterval.Location = new System.Drawing.Point(121, 46);
			this._copyInterval.Name = "_copyInterval";
			this._copyInterval.Size = new System.Drawing.Size(85, 20);
			this._copyInterval.TabIndex = 10;
			// 
			// _sendToClipboardButton
			// 
			this._sendToClipboardButton.Location = new System.Drawing.Point(238, 244);
			this._sendToClipboardButton.Name = "_sendToClipboardButton";
			this._sendToClipboardButton.Size = new System.Drawing.Size(110, 23);
			this._sendToClipboardButton.TabIndex = 14;
			this._sendToClipboardButton.Text = "Send To Clipboard";
			this._sendToClipboardButton.UseVisualStyleBackColor = true;
			this._sendToClipboardButton.Click += new System.EventHandler(this.OnSendToClipboard);
			// 
			// _betweenGroup
			// 
			this._betweenGroup.Controls.Add(this._radioCopyAllImages);
			this._betweenGroup.Controls.Add(this._copyInterval);
			this._betweenGroup.Controls.Add(this.label1);
			this._betweenGroup.Controls.Add(this._radioCopyAtInterval);
			this._betweenGroup.Location = new System.Drawing.Point(93, 107);
			this._betweenGroup.Name = "_betweenGroup";
			this._betweenGroup.Size = new System.Drawing.Size(255, 82);
			this._betweenGroup.TabIndex = 7;
			this._betweenGroup.TabStop = false;
			// 
			// _rangeSelectionGroup
			// 
			this._rangeSelectionGroup.Controls.Add(this._radioUsePosition);
			this._rangeSelectionGroup.Controls.Add(this._radioUseInstanceNumber);
			this._rangeSelectionGroup.Location = new System.Drawing.Point(93, 13);
			this._rangeSelectionGroup.Name = "_rangeSelectionGroup";
			this._rangeSelectionGroup.Size = new System.Drawing.Size(255, 51);
			this._rangeSelectionGroup.TabIndex = 0;
			this._rangeSelectionGroup.TabStop = false;
			this._rangeSelectionGroup.Text = "Use";
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
			// _radioUsePosition
			// 
			this._radioUsePosition.AutoSize = true;
			this._radioUsePosition.Location = new System.Drawing.Point(121, 19);
			this._radioUsePosition.Name = "_radioUsePosition";
			this._radioUsePosition.Size = new System.Drawing.Size(62, 17);
			this._radioUsePosition.TabIndex = 2;
			this._radioUsePosition.Text = "Position";
			this._radioUsePosition.UseVisualStyleBackColor = true;
			// 
			// CopySubsetToClipboardComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._rangeSelectionGroup);
			this.Controls.Add(this._sendToClipboardButton);
			this.Controls.Add(this._custom);
			this.Controls.Add(this._betweenSecondValue);
			this.Controls.Add(this._betweenFirstValue);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._radioBetween);
			this.Controls.Add(this._radioCustom);
			this.Controls.Add(this._betweenGroup);
			this.Name = "CopySubsetToClipboardComponentControl";
			this.Size = new System.Drawing.Size(367, 284);
			((System.ComponentModel.ISupportInitialize)(this._betweenFirstValue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._betweenSecondValue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._copyInterval)).EndInit();
			this._betweenGroup.ResumeLayout(false);
			this._betweenGroup.PerformLayout();
			this._rangeSelectionGroup.ResumeLayout(false);
			this._rangeSelectionGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.RadioButton _radioCustom;
		private System.Windows.Forms.RadioButton _radioBetween;
		private System.Windows.Forms.Label label2;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _betweenFirstValue;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _betweenSecondValue;
		private System.Windows.Forms.TextBox _custom;
		private System.Windows.Forms.RadioButton _radioCopyAllImages;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton _radioCopyAtInterval;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _copyInterval;
		private System.Windows.Forms.Button _sendToClipboardButton;
		private System.Windows.Forms.GroupBox _betweenGroup;
		private System.Windows.Forms.GroupBox _rangeSelectionGroup;
		private System.Windows.Forms.RadioButton _radioUsePosition;
		private System.Windows.Forms.RadioButton _radioUseInstanceNumber;
    }
}
