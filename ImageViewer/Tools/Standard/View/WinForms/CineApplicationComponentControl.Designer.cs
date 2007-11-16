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

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    partial class CineApplicationComponentControl
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
			this._cineSpeed = new System.Windows.Forms.TrackBar();
			this._minLabel = new System.Windows.Forms.Label();
			this._fastestLabel = new System.Windows.Forms.Label();
			this._startReverseButton = new System.Windows.Forms.Button();
			this._startForwardButton = new System.Windows.Forms.Button();
			this._stopButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._cineSpeed)).BeginInit();
			this.SuspendLayout();
			// 
			// _cineSpeed
			// 
			this._cineSpeed.LargeChange = 10;
			this._cineSpeed.Location = new System.Drawing.Point(6, 41);
			this._cineSpeed.Maximum = 100;
			this._cineSpeed.Name = "_cineSpeed";
			this._cineSpeed.Size = new System.Drawing.Size(186, 42);
			this._cineSpeed.TabIndex = 3;
			this._cineSpeed.TickFrequency = 10;
			// 
			// _minLabel
			// 
			this._minLabel.AutoSize = true;
			this._minLabel.Location = new System.Drawing.Point(11, 74);
			this._minLabel.Name = "_minLabel";
			this._minLabel.Size = new System.Drawing.Size(44, 13);
			this._minLabel.TabIndex = 4;
			this._minLabel.Text = "Slowest";
			// 
			// _fastestLabel
			// 
			this._fastestLabel.AutoSize = true;
			this._fastestLabel.Location = new System.Drawing.Point(148, 74);
			this._fastestLabel.Name = "_fastestLabel";
			this._fastestLabel.Size = new System.Drawing.Size(41, 13);
			this._fastestLabel.TabIndex = 5;
			this._fastestLabel.Text = "Fastest";
			// 
			// _startReverseButton
			// 
			this._startReverseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._startReverseButton.Image = global::ClearCanvas.ImageViewer.Tools.Standard.View.WinForms.Properties.Resources.PlayReverse;
			this._startReverseButton.Location = new System.Drawing.Point(14, 7);
			this._startReverseButton.Name = "_startReverseButton";
			this._startReverseButton.Size = new System.Drawing.Size(44, 31);
			this._startReverseButton.TabIndex = 0;
			this._startReverseButton.UseVisualStyleBackColor = true;
			this._startReverseButton.Click += new System.EventHandler(this.StartReverseButtonClicked);
			// 
			// _startForwardButton
			// 
			this._startForwardButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._startForwardButton.Image = global::ClearCanvas.ImageViewer.Tools.Standard.View.WinForms.Properties.Resources.Play;
			this._startForwardButton.Location = new System.Drawing.Point(140, 7);
			this._startForwardButton.Name = "_startForwardButton";
			this._startForwardButton.Size = new System.Drawing.Size(44, 31);
			this._startForwardButton.TabIndex = 2;
			this._startForwardButton.UseVisualStyleBackColor = true;
			this._startForwardButton.Click += new System.EventHandler(this.StartForwardButtonClicked);
			// 
			// _stopButton
			// 
			this._stopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._stopButton.Enabled = false;
			this._stopButton.Image = global::ClearCanvas.ImageViewer.Tools.Standard.View.WinForms.Properties.Resources.Stop;
			this._stopButton.Location = new System.Drawing.Point(77, 7);
			this._stopButton.Name = "_stopButton";
			this._stopButton.Size = new System.Drawing.Size(44, 31);
			this._stopButton.TabIndex = 1;
			this._stopButton.UseVisualStyleBackColor = true;
			this._stopButton.Click += new System.EventHandler(this.StopButtonClicked);
			// 
			// CineApplicationComponentControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this._startForwardButton);
			this.Controls.Add(this._fastestLabel);
			this.Controls.Add(this._minLabel);
			this.Controls.Add(this._cineSpeed);
			this.Controls.Add(this._stopButton);
			this.Controls.Add(this._startReverseButton);
			this.Name = "CineApplicationComponentControl";
			this.Size = new System.Drawing.Size(208, 112);
			((System.ComponentModel.ISupportInitialize)(this._cineSpeed)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _startReverseButton;
		private System.Windows.Forms.Button _stopButton;
		private System.Windows.Forms.TrackBar _cineSpeed;
		private System.Windows.Forms.Label _minLabel;
		private System.Windows.Forms.Label _fastestLabel;
		private System.Windows.Forms.Button _startForwardButton;
    }
}
