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
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this._supervisor = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._reportEditorPanel = new System.Windows.Forms.Panel();
			this._cancelButton = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._verifyButton = new System.Windows.Forms.Button();
			this._sendToVerifyButton = new System.Windows.Forms.Button();
			this._sendToTranscriptionButton = new System.Windows.Forms.Button();
			this._saveButton = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this._orderDetailsTab = new System.Windows.Forms.TabPage();
			this._priorReportsTab = new System.Windows.Forms.TabPage();
			this._orderAdditionalInfoTab = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._bannerPanel = new System.Windows.Forms.Panel();
			this._reportEditorSplitContainer.Panel1.SuspendLayout();
			this._reportEditorSplitContainer.Panel2.SuspendLayout();
			this._reportEditorSplitContainer.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
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
			this._reportEditorSplitContainer.Panel1.Controls.Add(this.tableLayoutPanel2);
			// 
			// _reportEditorSplitContainer.Panel2
			// 
			this._reportEditorSplitContainer.Panel2.Controls.Add(this.tabControl1);
			this._reportEditorSplitContainer.Size = new System.Drawing.Size(977, 826);
			this._reportEditorSplitContainer.SplitterDistance = 484;
			this._reportEditorSplitContainer.TabIndex = 0;
			this._reportEditorSplitContainer.TabStop = false;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this._supervisor, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this._cancelButton, 1, 2);
			this.tableLayoutPanel2.Controls.Add(this._reportEditorPanel, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 3;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(484, 826);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// _supervisor
			// 
			this._supervisor.AutoSize = true;
			this._supervisor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.SetColumnSpan(this._supervisor, 2);
			this._supervisor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._supervisor.LabelText = "Supervising Radiologist:";
			this._supervisor.Location = new System.Drawing.Point(2, 740);
			this._supervisor.Margin = new System.Windows.Forms.Padding(2);
			this._supervisor.Name = "_supervisor";
			this._supervisor.Size = new System.Drawing.Size(480, 41);
			this._supervisor.TabIndex = 1;
			this._supervisor.Value = null;
			// 
			// _reportEditorPanel
			// 
			this._reportEditorPanel.AutoSize = true;
			this._reportEditorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.SetColumnSpan(this._reportEditorPanel, 2);
			this._reportEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reportEditorPanel.Location = new System.Drawing.Point(3, 3);
			this._reportEditorPanel.Name = "_reportEditorPanel";
			this._reportEditorPanel.Size = new System.Drawing.Size(478, 732);
			this._reportEditorPanel.TabIndex = 0;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(397, 786);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(84, 37);
			this._cancelButton.TabIndex = 3;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._verifyButton);
			this.flowLayoutPanel1.Controls.Add(this._sendToVerifyButton);
			this.flowLayoutPanel1.Controls.Add(this._sendToTranscriptionButton);
			this.flowLayoutPanel1.Controls.Add(this._saveButton);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 783);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(394, 43);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// _verifyButton
			// 
			this._verifyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._verifyButton.Location = new System.Drawing.Point(3, 3);
			this._verifyButton.Name = "_verifyButton";
			this._verifyButton.Size = new System.Drawing.Size(84, 37);
			this._verifyButton.TabIndex = 0;
			this._verifyButton.Text = "Verify";
			this._verifyButton.UseVisualStyleBackColor = true;
			this._verifyButton.Click += new System.EventHandler(this._verifyButton_Click);
			// 
			// _sendToVerifyButton
			// 
			this._sendToVerifyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._sendToVerifyButton.Location = new System.Drawing.Point(93, 3);
			this._sendToVerifyButton.Name = "_sendToVerifyButton";
			this._sendToVerifyButton.Size = new System.Drawing.Size(84, 37);
			this._sendToVerifyButton.TabIndex = 1;
			this._sendToVerifyButton.Text = "To be Verified";
			this._sendToVerifyButton.UseVisualStyleBackColor = true;
			this._sendToVerifyButton.Click += new System.EventHandler(this._sendToVerifyButton_Click);
			// 
			// _sendToTranscriptionButton
			// 
			this._sendToTranscriptionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._sendToTranscriptionButton.Location = new System.Drawing.Point(183, 3);
			this._sendToTranscriptionButton.Name = "_sendToTranscriptionButton";
			this._sendToTranscriptionButton.Size = new System.Drawing.Size(84, 37);
			this._sendToTranscriptionButton.TabIndex = 2;
			this._sendToTranscriptionButton.Text = "Send to Transcription";
			this._sendToTranscriptionButton.UseVisualStyleBackColor = true;
			this._sendToTranscriptionButton.Click += new System.EventHandler(this._sendToTranscriptionButton_Click);
			// 
			// _saveButton
			// 
			this._saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._saveButton.Location = new System.Drawing.Point(273, 3);
			this._saveButton.Name = "_saveButton";
			this._saveButton.Size = new System.Drawing.Size(84, 37);
			this._saveButton.TabIndex = 3;
			this._saveButton.Text = "Save";
			this._saveButton.UseVisualStyleBackColor = true;
			this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this._orderDetailsTab);
			this.tabControl1.Controls.Add(this._priorReportsTab);
			this.tabControl1.Controls.Add(this._orderAdditionalInfoTab);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(489, 826);
			this.tabControl1.TabIndex = 0;
			// 
			// _orderDetailsTab
			// 
			this._orderDetailsTab.Location = new System.Drawing.Point(4, 22);
			this._orderDetailsTab.Name = "_orderDetailsTab";
			this._orderDetailsTab.Padding = new System.Windows.Forms.Padding(3);
			this._orderDetailsTab.Size = new System.Drawing.Size(481, 800);
			this._orderDetailsTab.TabIndex = 1;
			this._orderDetailsTab.Text = "Order Details";
			this._orderDetailsTab.UseVisualStyleBackColor = true;
			// 
			// _priorReportsTab
			// 
			this._priorReportsTab.Location = new System.Drawing.Point(4, 22);
			this._priorReportsTab.Name = "_priorReportsTab";
			this._priorReportsTab.Padding = new System.Windows.Forms.Padding(3);
			this._priorReportsTab.Size = new System.Drawing.Size(481, 800);
			this._priorReportsTab.TabIndex = 0;
			this._priorReportsTab.Text = "Prior Reports";
			this._priorReportsTab.UseVisualStyleBackColor = true;
			// 
			// _orderAdditionalInfoTab
			// 
			this._orderAdditionalInfoTab.Location = new System.Drawing.Point(4, 22);
			this._orderAdditionalInfoTab.Name = "_orderAdditionalInfoTab";
			this._orderAdditionalInfoTab.Padding = new System.Windows.Forms.Padding(3);
			this._orderAdditionalInfoTab.Size = new System.Drawing.Size(481, 800);
			this._orderAdditionalInfoTab.TabIndex = 2;
			this._orderAdditionalInfoTab.Text = "Additional Info";
			this._orderAdditionalInfoTab.UseVisualStyleBackColor = true;
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
			this.tableLayoutPanel1.Size = new System.Drawing.Size(983, 917);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _bannerPanel
			// 
			this._bannerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bannerPanel.Location = new System.Drawing.Point(3, 3);
			this._bannerPanel.Name = "_bannerPanel";
			this._bannerPanel.Size = new System.Drawing.Size(977, 79);
			this._bannerPanel.TabIndex = 0;
			// 
			// ReportingComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ReportingComponentControl";
			this.Size = new System.Drawing.Size(983, 917);
			this._reportEditorSplitContainer.Panel1.ResumeLayout(false);
			this._reportEditorSplitContainer.Panel1.PerformLayout();
			this._reportEditorSplitContainer.Panel2.ResumeLayout(false);
			this._reportEditorSplitContainer.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage _orderAdditionalInfoTab;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
