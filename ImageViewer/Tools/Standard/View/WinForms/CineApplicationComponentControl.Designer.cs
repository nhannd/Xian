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
			this._startButton = new System.Windows.Forms.Button();
			this._stopButton = new System.Windows.Forms.Button();
			this._checkBoxReverse = new System.Windows.Forms.CheckBox();
			this._cineSpeed = new System.Windows.Forms.TrackBar();
			this._minLabel = new System.Windows.Forms.Label();
			this._fastestLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._cineSpeed)).BeginInit();
			this.SuspendLayout();
			// 
			// _startButton
			// 
			this._startButton.Location = new System.Drawing.Point(14, 7);
			this._startButton.Name = "_startButton";
			this._startButton.Size = new System.Drawing.Size(75, 23);
			this._startButton.TabIndex = 0;
			this._startButton.Text = "Start";
			this._startButton.UseVisualStyleBackColor = true;
			this._startButton.Click += new System.EventHandler(this.StartButtonClicked);
			// 
			// _stopButton
			// 
			this._stopButton.Enabled = false;
			this._stopButton.Location = new System.Drawing.Point(97, 7);
			this._stopButton.Name = "_stopButton";
			this._stopButton.Size = new System.Drawing.Size(75, 23);
			this._stopButton.TabIndex = 1;
			this._stopButton.Text = "Stop";
			this._stopButton.UseVisualStyleBackColor = true;
			this._stopButton.Click += new System.EventHandler(this.StopButtonClicked);
			// 
			// _checkBoxReverse
			// 
			this._checkBoxReverse.AutoSize = true;
			this._checkBoxReverse.Location = new System.Drawing.Point(179, 11);
			this._checkBoxReverse.Name = "_checkBoxReverse";
			this._checkBoxReverse.Size = new System.Drawing.Size(66, 17);
			this._checkBoxReverse.TabIndex = 2;
			this._checkBoxReverse.Text = "Reverse";
			this._checkBoxReverse.UseVisualStyleBackColor = true;
			// 
			// _cineSpeed
			// 
			this._cineSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._cineSpeed.LargeChange = 10;
			this._cineSpeed.Location = new System.Drawing.Point(6, 36);
			this._cineSpeed.Maximum = 100;
			this._cineSpeed.Name = "_cineSpeed";
			this._cineSpeed.Size = new System.Drawing.Size(239, 42);
			this._cineSpeed.TabIndex = 4;
			this._cineSpeed.TickFrequency = 10;
			// 
			// _minLabel
			// 
			this._minLabel.AutoSize = true;
			this._minLabel.Location = new System.Drawing.Point(11, 74);
			this._minLabel.Name = "_minLabel";
			this._minLabel.Size = new System.Drawing.Size(44, 13);
			this._minLabel.TabIndex = 5;
			this._minLabel.Text = "Slowest";
			// 
			// _fastestLabel
			// 
			this._fastestLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._fastestLabel.AutoSize = true;
			this._fastestLabel.Location = new System.Drawing.Point(200, 74);
			this._fastestLabel.Name = "_fastestLabel";
			this._fastestLabel.Size = new System.Drawing.Size(41, 13);
			this._fastestLabel.TabIndex = 6;
			this._fastestLabel.Text = "Fastest";
			// 
			// CineApplicationComponentControl
			// 
			this.AcceptButton = this._startButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._fastestLabel);
			this.Controls.Add(this._minLabel);
			this.Controls.Add(this._cineSpeed);
			this.Controls.Add(this._checkBoxReverse);
			this.Controls.Add(this._stopButton);
			this.Controls.Add(this._startButton);
			this.Name = "CineApplicationComponentControl";
			this.Size = new System.Drawing.Size(251, 95);
			((System.ComponentModel.ISupportInitialize)(this._cineSpeed)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _startButton;
		private System.Windows.Forms.Button _stopButton;
		private System.Windows.Forms.CheckBox _checkBoxReverse;
		private System.Windows.Forms.TrackBar _cineSpeed;
		private System.Windows.Forms.Label _minLabel;
		private System.Windows.Forms.Label _fastestLabel;
    }
}
