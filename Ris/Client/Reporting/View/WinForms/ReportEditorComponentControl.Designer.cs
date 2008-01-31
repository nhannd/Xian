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

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    partial class ReportEditorComponentControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._browserSplitContainer = new System.Windows.Forms.SplitContainer();
            this._residentPanel = new System.Windows.Forms.Panel();
            this._supervisor = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
            this._cancelButton = new System.Windows.Forms.Button();
            this._buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._verifyButton = new System.Windows.Forms.Button();
            this._sendToVerifyButton = new System.Windows.Forms.Button();
            this._sendToTranscriptionButton = new System.Windows.Forms.Button();
            this._saveButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this._browserSplitContainer.SuspendLayout();
            this._residentPanel.SuspendLayout();
            this._buttonPanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this._browserSplitContainer, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._residentPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._buttonPanel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(661, 606);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _browserSplitContainer
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._browserSplitContainer, 2);
            this._browserSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._browserSplitContainer.Location = new System.Drawing.Point(3, 3);
            this._browserSplitContainer.Name = "_browserSplitContainer";
            this._browserSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this._browserSplitContainer.Size = new System.Drawing.Size(655, 490);
            this._browserSplitContainer.SplitterDistance = 120;
            this._browserSplitContainer.TabIndex = 6;
            // 
            // _residentPanel
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._residentPanel, 2);
            this._residentPanel.Controls.Add(this._supervisor);
            this._residentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._residentPanel.Location = new System.Drawing.Point(3, 499);
            this._residentPanel.Name = "_residentPanel";
            this._residentPanel.Size = new System.Drawing.Size(655, 54);
            this._residentPanel.TabIndex = 2;
            // 
            // _supervisor
            // 
            this._supervisor.LabelText = "Supervising Radiologist:";
            this._supervisor.Location = new System.Drawing.Point(3, 2);
            this._supervisor.Margin = new System.Windows.Forms.Padding(2);
            this._supervisor.Name = "_supervisor";
            this._supervisor.Size = new System.Drawing.Size(234, 49);
            this._supervisor.TabIndex = 5;
            this._supervisor.Value = null;
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.Location = new System.Drawing.Point(13, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(84, 37);
            this._cancelButton.TabIndex = 4;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _buttonPanel
            // 
            this._buttonPanel.Controls.Add(this._verifyButton);
            this._buttonPanel.Controls.Add(this._sendToVerifyButton);
            this._buttonPanel.Controls.Add(this._sendToTranscriptionButton);
            this._buttonPanel.Controls.Add(this._saveButton);
            this._buttonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._buttonPanel.Location = new System.Drawing.Point(3, 559);
            this._buttonPanel.Name = "_buttonPanel";
            this._buttonPanel.Size = new System.Drawing.Size(549, 44);
            this._buttonPanel.TabIndex = 5;
            // 
            // _verifyButton
            // 
            this._verifyButton.Location = new System.Drawing.Point(3, 3);
            this._verifyButton.Name = "_verifyButton";
            this._verifyButton.Size = new System.Drawing.Size(84, 37);
            this._verifyButton.TabIndex = 4;
            this._verifyButton.Text = "Verify";
            this._verifyButton.UseVisualStyleBackColor = true;
            this._verifyButton.Click += new System.EventHandler(this._verifyButton_Click);
            // 
            // _sendToVerifyButton
            // 
            this._sendToVerifyButton.Location = new System.Drawing.Point(93, 3);
            this._sendToVerifyButton.Name = "_sendToVerifyButton";
            this._sendToVerifyButton.Size = new System.Drawing.Size(84, 37);
            this._sendToVerifyButton.TabIndex = 5;
            this._sendToVerifyButton.Text = "To be Verified";
            this._sendToVerifyButton.UseVisualStyleBackColor = true;
            this._sendToVerifyButton.Click += new System.EventHandler(this._sendToVerifyButton_Click);
            // 
            // _sendToTranscriptionButton
            // 
            this._sendToTranscriptionButton.Location = new System.Drawing.Point(183, 3);
            this._sendToTranscriptionButton.Name = "_sendToTranscriptionButton";
            this._sendToTranscriptionButton.Size = new System.Drawing.Size(84, 37);
            this._sendToTranscriptionButton.TabIndex = 6;
            this._sendToTranscriptionButton.Text = "Send to Transcription";
            this._sendToTranscriptionButton.UseVisualStyleBackColor = true;
            this._sendToTranscriptionButton.Click += new System.EventHandler(this._sendToTranscriptionButton_Click);
            // 
            // _saveButton
            // 
            this._saveButton.Location = new System.Drawing.Point(273, 3);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(84, 37);
            this._saveButton.TabIndex = 7;
            this._saveButton.Text = "Save";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(558, 559);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(100, 44);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // ReportEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ReportEditorComponentControl";
            this.Size = new System.Drawing.Size(661, 606);
            this.tableLayoutPanel1.ResumeLayout(false);
            this._browserSplitContainer.ResumeLayout(false);
            this._residentPanel.ResumeLayout(false);
            this._buttonPanel.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Panel _residentPanel;
        private ClearCanvas.Ris.Client.View.WinForms.LookupField _supervisor;
        private System.Windows.Forms.FlowLayoutPanel _buttonPanel;
        private System.Windows.Forms.Button _verifyButton;
        private System.Windows.Forms.Button _sendToVerifyButton;
        private System.Windows.Forms.Button _sendToTranscriptionButton;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.SplitContainer _browserSplitContainer;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;

    }
}
