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

namespace ClearCanvas.Utilities.RebuildDatabase
{
    public partial class RebuildDatabaseForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RebuildDatabaseForm));
			this.label2 = new System.Windows.Forms.Label();
			this._imageFolderText = new System.Windows.Forms.TextBox();
			this._findFilesRecursivelyCheckbox = new System.Windows.Forms.CheckBox();
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._startButton = new System.Windows.Forms.Button();
			this._stopButton = new System.Windows.Forms.Button();
			this._exitButton = new System.Windows.Forms.Button();
			this._browseForFolderButton = new System.Windows.Forms.Button();
			this._statusGroupBox = new System.Windows.Forms.GroupBox();
			this._statusTextLabel = new System.Windows.Forms.Label();
			this._statusGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 79);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "DICOM Image Folder";
			// 
			// _imageFolderText
			// 
			this._imageFolderText.Location = new System.Drawing.Point(11, 96);
			this._imageFolderText.Margin = new System.Windows.Forms.Padding(2);
			this._imageFolderText.Name = "_imageFolderText";
			this._imageFolderText.Size = new System.Drawing.Size(394, 20);
			this._imageFolderText.TabIndex = 3;
			// 
			// _findFilesRecursivelyCheckbox
			// 
			this._findFilesRecursivelyCheckbox.AutoSize = true;
			this._findFilesRecursivelyCheckbox.Location = new System.Drawing.Point(11, 119);
			this._findFilesRecursivelyCheckbox.Margin = new System.Windows.Forms.Padding(2);
			this._findFilesRecursivelyCheckbox.Name = "_findFilesRecursivelyCheckbox";
			this._findFilesRecursivelyCheckbox.Size = new System.Drawing.Size(158, 17);
			this._findFilesRecursivelyCheckbox.TabIndex = 4;
			this._findFilesRecursivelyCheckbox.Text = "Find DICOM files recursively";
			this._findFilesRecursivelyCheckbox.UseVisualStyleBackColor = true;
			// 
			// _progressBar
			// 
			this._progressBar.Location = new System.Drawing.Point(4, 37);
			this._progressBar.Margin = new System.Windows.Forms.Padding(2);
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(411, 19);
			this._progressBar.TabIndex = 5;
			// 
			// _startButton
			// 
			this._startButton.Location = new System.Drawing.Point(138, 152);
			this._startButton.Margin = new System.Windows.Forms.Padding(2);
			this._startButton.Name = "_startButton";
			this._startButton.Size = new System.Drawing.Size(56, 19);
			this._startButton.TabIndex = 7;
			this._startButton.Text = "Start";
			this._startButton.UseVisualStyleBackColor = true;
			this._startButton.Click += new System.EventHandler(this.StartButtonClick);
			// 
			// _stopButton
			// 
			this._stopButton.Location = new System.Drawing.Point(200, 152);
			this._stopButton.Margin = new System.Windows.Forms.Padding(2);
			this._stopButton.Name = "_stopButton";
			this._stopButton.Size = new System.Drawing.Size(56, 19);
			this._stopButton.TabIndex = 8;
			this._stopButton.Text = "Stop";
			this._stopButton.UseVisualStyleBackColor = true;
			this._stopButton.Click += new System.EventHandler(this.StopButtonClick);
			// 
			// _exitButton
			// 
			this._exitButton.Location = new System.Drawing.Point(261, 152);
			this._exitButton.Margin = new System.Windows.Forms.Padding(2);
			this._exitButton.Name = "_exitButton";
			this._exitButton.Size = new System.Drawing.Size(56, 19);
			this._exitButton.TabIndex = 9;
			this._exitButton.Text = "Exit";
			this._exitButton.UseVisualStyleBackColor = true;
			this._exitButton.Click += new System.EventHandler(this.ExitButtonClick);
			// 
			// _browseForFolderButton
			// 
			this._browseForFolderButton.Location = new System.Drawing.Point(410, 94);
			this._browseForFolderButton.Margin = new System.Windows.Forms.Padding(2);
			this._browseForFolderButton.Name = "_browseForFolderButton";
			this._browseForFolderButton.Size = new System.Drawing.Size(22, 19);
			this._browseForFolderButton.TabIndex = 10;
			this._browseForFolderButton.Text = "...";
			this._browseForFolderButton.UseVisualStyleBackColor = true;
			this._browseForFolderButton.Click += new System.EventHandler(this.BrowseClick);
			// 
			// _statusGroupBox
			// 
			this._statusGroupBox.Controls.Add(this._statusTextLabel);
			this._statusGroupBox.Controls.Add(this._progressBar);
			this._statusGroupBox.Location = new System.Drawing.Point(11, 11);
			this._statusGroupBox.Margin = new System.Windows.Forms.Padding(2);
			this._statusGroupBox.Name = "_statusGroupBox";
			this._statusGroupBox.Padding = new System.Windows.Forms.Padding(2);
			this._statusGroupBox.Size = new System.Drawing.Size(420, 63);
			this._statusGroupBox.TabIndex = 11;
			this._statusGroupBox.TabStop = false;
			this._statusGroupBox.Text = "Status";
			// 
			// _statusTextLabel
			// 
			this._statusTextLabel.AutoSize = true;
			this._statusTextLabel.Location = new System.Drawing.Point(2, 17);
			this._statusTextLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this._statusTextLabel.Name = "_statusTextLabel";
			this._statusTextLabel.Size = new System.Drawing.Size(0, 13);
			this._statusTextLabel.TabIndex = 6;
			// 
			// RebuildDatabaseForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(440, 177);
			this.Controls.Add(this._browseForFolderButton);
			this.Controls.Add(this._exitButton);
			this.Controls.Add(this._stopButton);
			this.Controls.Add(this._startButton);
			this.Controls.Add(this._findFilesRecursivelyCheckbox);
			this.Controls.Add(this._imageFolderText);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._statusGroupBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "RebuildDatabaseForm";
			this.Text = "Rebuild Database";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RebuildDatabaseFormClosing);
			this._statusGroupBox.ResumeLayout(false);
			this._statusGroupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _imageFolderText;
        private System.Windows.Forms.CheckBox _findFilesRecursivelyCheckbox;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Button _startButton;
        private System.Windows.Forms.Button _stopButton;
        private System.Windows.Forms.Button _exitButton;
        private System.Windows.Forms.Button _browseForFolderButton;
        private System.Windows.Forms.GroupBox _statusGroupBox;
        private System.Windows.Forms.Label _statusTextLabel;
    }
}

