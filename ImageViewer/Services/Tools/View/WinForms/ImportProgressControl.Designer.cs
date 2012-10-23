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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportProgressControl));
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
			resources.ApplyResources(this._statusLabel, "_statusLabel");
			this._statusLabel.Name = "_statusLabel";
			// 
			// _processedCount
			// 
			resources.ApplyResources(this._processedCount, "_processedCount");
			this._processedCount.AutoEllipsis = true;
			this._processedCount.Name = "_processedCount";
			// 
			// _processedProgress
			// 
			resources.ApplyResources(this._processedProgress, "_processedProgress");
			this._processedProgress.Name = "_processedProgress";
			// 
			// _failedLabel
			// 
			resources.ApplyResources(this._failedLabel, "_failedLabel");
			this._failedLabel.Name = "_failedLabel";
			// 
			// _processedLabel
			// 
			resources.ApplyResources(this._processedLabel, "_processedLabel");
			this._processedLabel.Name = "_processedLabel";
			// 
			// _availableLabel
			// 
			resources.ApplyResources(this._availableLabel, "_availableLabel");
			this._availableLabel.Name = "_availableLabel";
			// 
			// _failedCount
			// 
			resources.ApplyResources(this._failedCount, "_failedCount");
			this._failedCount.AutoEllipsis = true;
			this._failedCount.Name = "_failedCount";
			// 
			// _statusMessage
			// 
			resources.ApplyResources(this._statusMessage, "_statusMessage");
			this._statusMessage.AutoEllipsis = true;
			this._statusMessage.Name = "_statusMessage";
			// 
			// _availableCount
			// 
			resources.ApplyResources(this._availableCount, "_availableCount");
			this._availableCount.AutoEllipsis = true;
			this._availableCount.Name = "_availableCount";
			// 
			// _button
			// 
			resources.ApplyResources(this._button, "_button");
			this._button.Name = "_button";
			this._button.UseVisualStyleBackColor = true;
			// 
			// ImportProgressControl
			// 
			resources.ApplyResources(this, "$this");
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
			this.ResumeLayout(false);

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
