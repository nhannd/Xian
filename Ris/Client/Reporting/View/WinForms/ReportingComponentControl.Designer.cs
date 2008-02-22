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
    partial class ReportingComponentControl
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
            this._reportEditorSplitContainer = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this._orderDetailsTab = new System.Windows.Forms.TabPage();
            this._priorReportsTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._bannerPanel = new System.Windows.Forms.Panel();
            this._reportEditorPanel = new System.Windows.Forms.Panel();
            this._verifyButton = new System.Windows.Forms.Button();
            this._sendToVerifyButton = new System.Windows.Forms.Button();
            this._sendToTranscriptionButton = new System.Windows.Forms.Button();
            this._saveButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._supervisor = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
            this._reportEditorSplitContainer.Panel1.SuspendLayout();
            this._reportEditorSplitContainer.Panel2.SuspendLayout();
            this._reportEditorSplitContainer.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _reportEditorSplitContainer
            // 
            this._reportEditorSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._reportEditorSplitContainer.Location = new System.Drawing.Point(3, 88);
            this._reportEditorSplitContainer.Name = "_reportEditorSplitContainer";
            // 
            // _reportEditorSplitContainer.Panel1
            // 
            this._reportEditorSplitContainer.Panel1.Controls.Add(this._supervisor);
            this._reportEditorSplitContainer.Panel1.Controls.Add(this._reportEditorPanel);
            this._reportEditorSplitContainer.Panel1.Controls.Add(this._cancelButton);
            this._reportEditorSplitContainer.Panel1.Controls.Add(this._saveButton);
            this._reportEditorSplitContainer.Panel1.Controls.Add(this._sendToTranscriptionButton);
            this._reportEditorSplitContainer.Panel1.Controls.Add(this._sendToVerifyButton);
            this._reportEditorSplitContainer.Panel1.Controls.Add(this._verifyButton);
            // 
            // _reportEditorSplitContainer.Panel2
            // 
            this._reportEditorSplitContainer.Panel2.Controls.Add(this.tabControl1);
            this._reportEditorSplitContainer.Size = new System.Drawing.Size(946, 444);
            this._reportEditorSplitContainer.SplitterDistance = 469;
            this._reportEditorSplitContainer.TabIndex = 0;
            this._reportEditorSplitContainer.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this._orderDetailsTab);
            this.tabControl1.Controls.Add(this._priorReportsTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(473, 444);
            this.tabControl1.TabIndex = 0;
            // 
            // _orderDetailsTab
            // 
            this._orderDetailsTab.Location = new System.Drawing.Point(4, 22);
            this._orderDetailsTab.Name = "_orderDetailsTab";
            this._orderDetailsTab.Padding = new System.Windows.Forms.Padding(3);
            this._orderDetailsTab.Size = new System.Drawing.Size(465, 418);
            this._orderDetailsTab.TabIndex = 1;
            this._orderDetailsTab.Text = "Order Details";
            this._orderDetailsTab.UseVisualStyleBackColor = true;
            // 
            // _priorReportsTab
            // 
            this._priorReportsTab.Location = new System.Drawing.Point(4, 22);
            this._priorReportsTab.Name = "_priorReportsTab";
            this._priorReportsTab.Padding = new System.Windows.Forms.Padding(3);
            this._priorReportsTab.Size = new System.Drawing.Size(287, 418);
            this._priorReportsTab.TabIndex = 0;
            this._priorReportsTab.Text = "Prior Reports";
            this._priorReportsTab.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._reportEditorSplitContainer, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._bannerPanel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(952, 535);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // _bannerPanel
            // 
            this._bannerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._bannerPanel.Location = new System.Drawing.Point(3, 3);
            this._bannerPanel.Name = "_bannerPanel";
            this._bannerPanel.Size = new System.Drawing.Size(946, 79);
            this._bannerPanel.TabIndex = 0;
            // 
            // _reportEditorPanel
            // 
            this._reportEditorPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._reportEditorPanel.Location = new System.Drawing.Point(3, 3);
            this._reportEditorPanel.Name = "_reportEditorPanel";
            this._reportEditorPanel.Size = new System.Drawing.Size(463, 341);
            this._reportEditorPanel.TabIndex = 0;
            // 
            // _verifyButton
            // 
            this._verifyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._verifyButton.Location = new System.Drawing.Point(3, 403);
            this._verifyButton.Name = "_verifyButton";
            this._verifyButton.Size = new System.Drawing.Size(84, 37);
            this._verifyButton.TabIndex = 5;
            this._verifyButton.Text = "Verify";
            this._verifyButton.UseVisualStyleBackColor = true;
            this._verifyButton.Click += new System.EventHandler(this._verifyButton_Click);
            // 
            // _sendToVerifyButton
            // 
            this._sendToVerifyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._sendToVerifyButton.Location = new System.Drawing.Point(93, 403);
            this._sendToVerifyButton.Name = "_sendToVerifyButton";
            this._sendToVerifyButton.Size = new System.Drawing.Size(84, 37);
            this._sendToVerifyButton.TabIndex = 6;
            this._sendToVerifyButton.Text = "To be Verified";
            this._sendToVerifyButton.UseVisualStyleBackColor = true;
            this._sendToVerifyButton.Click += new System.EventHandler(this._sendToVerifyButton_Click);
            // 
            // _sendToTranscriptionButton
            // 
            this._sendToTranscriptionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._sendToTranscriptionButton.Location = new System.Drawing.Point(183, 403);
            this._sendToTranscriptionButton.Name = "_sendToTranscriptionButton";
            this._sendToTranscriptionButton.Size = new System.Drawing.Size(84, 37);
            this._sendToTranscriptionButton.TabIndex = 7;
            this._sendToTranscriptionButton.Text = "Send to Transcription";
            this._sendToTranscriptionButton.UseVisualStyleBackColor = true;
            this._sendToTranscriptionButton.Click += new System.EventHandler(this._sendToTranscriptionButton_Click);
            // 
            // _saveButton
            // 
            this._saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._saveButton.Location = new System.Drawing.Point(273, 403);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(84, 37);
            this._saveButton.TabIndex = 8;
            this._saveButton.Text = "Save";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.Location = new System.Drawing.Point(382, 403);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(84, 37);
            this._cancelButton.TabIndex = 9;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _supervisor
            // 
            this._supervisor.LabelText = "Supervising Radiologist:";
            this._supervisor.Location = new System.Drawing.Point(3, 349);
            this._supervisor.Margin = new System.Windows.Forms.Padding(2);
            this._supervisor.Name = "_supervisor";
            this._supervisor.Size = new System.Drawing.Size(234, 49);
            this._supervisor.TabIndex = 10;
            this._supervisor.Value = null;
            // 
            // ReportingComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ReportingComponentControl";
            this.Size = new System.Drawing.Size(952, 535);
            this._reportEditorSplitContainer.Panel1.ResumeLayout(false);
            this._reportEditorSplitContainer.Panel2.ResumeLayout(false);
            this._reportEditorSplitContainer.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer _reportEditorSplitContainer;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage _priorReportsTab;
        private System.Windows.Forms.TabPage _orderDetailsTab;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel _bannerPanel;
        private System.Windows.Forms.Panel _reportEditorPanel;
        private System.Windows.Forms.Button _verifyButton;
        private System.Windows.Forms.Button _sendToVerifyButton;
        private System.Windows.Forms.Button _sendToTranscriptionButton;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.Button _cancelButton;
        private ClearCanvas.Ris.Client.View.WinForms.LookupField _supervisor;
    }
}
