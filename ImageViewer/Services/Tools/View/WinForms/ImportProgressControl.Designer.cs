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
	partial class ImportProgressControl
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
			this._statusLabel = new System.Windows.Forms.Label();
			this._processedCount = new System.Windows.Forms.Label();
			this._processedProgress = new System.Windows.Forms.ProgressBar();
			this._failedLabel = new System.Windows.Forms.Label();
			this._processedLabel = new System.Windows.Forms.Label();
			this._availableLabel = new System.Windows.Forms.Label();
			this._failedCount = new System.Windows.Forms.Label();
			this._statusMessage = new System.Windows.Forms.Label();
			this._availableCount = new System.Windows.Forms.Label();
			this._button = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _statusLabel
			// 
			this._statusLabel.AutoSize = true;
			this._statusLabel.Location = new System.Drawing.Point(23, 15);
			this._statusLabel.Name = "_statusLabel";
			this._statusLabel.Size = new System.Drawing.Size(40, 13);
			this._statusLabel.TabIndex = 0;
			this._statusLabel.Text = "Status:";
			this._statusLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// _processedCount
			// 
			this._processedCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._processedCount.AutoEllipsis = true;
			this._processedCount.Location = new System.Drawing.Point(338, 47);
			this._processedCount.Name = "_processedCount";
			this._processedCount.Size = new System.Drawing.Size(40, 23);
			this._processedCount.TabIndex = 4;
			this._processedCount.Text = "0";
			this._processedCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _processedProgress
			// 
			this._processedProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._processedProgress.Location = new System.Drawing.Point(69, 48);
			this._processedProgress.Name = "_processedProgress";
			this._processedProgress.Size = new System.Drawing.Size(263, 20);
			this._processedProgress.TabIndex = 3;
			// 
			// _failedLabel
			// 
			this._failedLabel.AutoSize = true;
			this._failedLabel.Location = new System.Drawing.Point(25, 126);
			this._failedLabel.Name = "_failedLabel";
			this._failedLabel.Size = new System.Drawing.Size(38, 13);
			this._failedLabel.TabIndex = 7;
			this._failedLabel.Text = "Failed:";
			this._failedLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// _processedLabel
			// 
			this._processedLabel.AutoSize = true;
			this._processedLabel.Location = new System.Drawing.Point(3, 52);
			this._processedLabel.Name = "_processedLabel";
			this._processedLabel.Size = new System.Drawing.Size(60, 13);
			this._processedLabel.TabIndex = 2;
			this._processedLabel.Text = "Processed:";
			this._processedLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// _availableLabel
			// 
			this._availableLabel.AutoSize = true;
			this._availableLabel.Location = new System.Drawing.Point(10, 92);
			this._availableLabel.Name = "_availableLabel";
			this._availableLabel.Size = new System.Drawing.Size(53, 13);
			this._availableLabel.TabIndex = 5;
			this._availableLabel.Text = "Available:";
			this._availableLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// _failedCount
			// 
			this._failedCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._failedCount.AutoEllipsis = true;
			this._failedCount.Location = new System.Drawing.Point(69, 126);
			this._failedCount.Name = "_failedCount";
			this._failedCount.Size = new System.Drawing.Size(85, 23);
			this._failedCount.TabIndex = 8;
			this._failedCount.Text = "0";
			// 
			// _statusMessage
			// 
			this._statusMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._statusMessage.AutoEllipsis = true;
			this._statusMessage.Location = new System.Drawing.Point(69, 15);
			this._statusMessage.Name = "_statusMessage";
			this._statusMessage.Size = new System.Drawing.Size(263, 23);
			this._statusMessage.TabIndex = 1;
			this._statusMessage.Text = "Pending ...";
			// 
			// _availableCount
			// 
			this._availableCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._availableCount.AutoEllipsis = true;
			this._availableCount.Location = new System.Drawing.Point(69, 87);
			this._availableCount.Name = "_availableCount";
			this._availableCount.Size = new System.Drawing.Size(85, 23);
			this._availableCount.TabIndex = 6;
			this._availableCount.Text = "0";
			this._availableCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _button
			// 
			this._button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._button.Location = new System.Drawing.Point(257, 121);
			this._button.Name = "_button";
			this._button.Size = new System.Drawing.Size(75, 23);
			this._button.TabIndex = 9;
			this._button.Text = "Cancel";
			this._button.UseVisualStyleBackColor = true;
			// 
			// ImportProgressControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._button);
			this.Controls.Add(this._availableCount);
			this.Controls.Add(this._statusMessage);
			this.Controls.Add(this._failedCount);
			this.Controls.Add(this._availableLabel);
			this.Controls.Add(this._processedLabel);
			this.Controls.Add(this._failedLabel);
			this.Controls.Add(this._processedProgress);
			this.Controls.Add(this._processedCount);
			this.Controls.Add(this._statusLabel);
			this.Name = "ImportProgressControl";
			this.Size = new System.Drawing.Size(385, 170);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _statusLabel;
		private System.Windows.Forms.Label _processedCount;
		private System.Windows.Forms.ProgressBar _processedProgress;
		private System.Windows.Forms.Label _failedLabel;
		private System.Windows.Forms.Label _processedLabel;
		private System.Windows.Forms.Label _availableLabel;
		private System.Windows.Forms.Label _failedCount;
		private System.Windows.Forms.Label _statusMessage;
		private System.Windows.Forms.Label _availableCount;
		private System.Windows.Forms.Button _button;
	}
}
