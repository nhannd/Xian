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

namespace ClearCanvas.ImageViewer.Tools.Measurement.View.WinForms
{
    partial class CalibrationComponentControl
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
			this._length = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this._ok = new System.Windows.Forms.Button();
			this._cancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._length)).BeginInit();
			this.SuspendLayout();
			// 
			// _length
			// 
			this._length.Location = new System.Drawing.Point(49, 20);
			this._length.Name = "_length";
			this._length.Size = new System.Drawing.Size(64, 20);
			this._length.TabIndex = 0;
			this._length.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(119, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(21, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "cm";
			// 
			// _ok
			// 
			this._ok.Location = new System.Drawing.Point(16, 52);
			this._ok.Name = "_ok";
			this._ok.Size = new System.Drawing.Size(75, 23);
			this._ok.TabIndex = 3;
			this._ok.Text = "OK";
			this._ok.UseVisualStyleBackColor = true;
			// 
			// _cancel
			// 
			this._cancel.Location = new System.Drawing.Point(97, 52);
			this._cancel.Name = "_cancel";
			this._cancel.Size = new System.Drawing.Size(75, 23);
			this._cancel.TabIndex = 4;
			this._cancel.Text = "Cancel";
			this._cancel.UseVisualStyleBackColor = true;
			// 
			// CalibrationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._cancel);
			this.Controls.Add(this._ok);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._length);
			this.Name = "CalibrationComponentControl";
			this.Size = new System.Drawing.Size(190, 92);
			((System.ComponentModel.ISupportInitialize)(this._length)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.NumericUpDown _length;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.Button _cancel;
    }
}
