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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
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
			this._hasErrors = new System.Windows.Forms.Label();
			this._imagesUnavailable = new System.Windows.Forms.Label();
			this._supervisor = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._cancelButton = new System.Windows.Forms.Button();
			this._reportEditorPanel = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._verifyButton = new System.Windows.Forms.Button();
			this._sendToVerifyButton = new System.Windows.Forms.Button();
			this._sendToTranscriptionButton = new System.Windows.Forms.Button();
			this._saveButton = new System.Windows.Forms.Button();
			this._btnSkip = new System.Windows.Forms.Button();
			this._reportNextItem = new System.Windows.Forms.CheckBox();
			this._reportedProcedures = new System.Windows.Forms.Label();
			this._rightHandPanel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._statusText = new System.Windows.Forms.Label();
			this._bannerPanel = new System.Windows.Forms.Panel();
			this._rememberSupervisorCheckbox = new System.Windows.Forms.CheckBox();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._reportEditorSplitContainer.Panel1.SuspendLayout();
			this._reportEditorSplitContainer.Panel2.SuspendLayout();
			this._reportEditorSplitContainer.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// _reportEditorSplitContainer
			// 
			this._reportEditorSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reportEditorSplitContainer.Location = new System.Drawing.Point(3, 113);
			this._reportEditorSplitContainer.Name = "_reportEditorSplitContainer";
			// 
			// _reportEditorSplitContainer.Panel1
			// 
			this._reportEditorSplitContainer.Panel1.Controls.Add(this.tableLayoutPanel2);
			// 
			// _reportEditorSplitContainer.Panel2
			// 
			this._reportEditorSplitContainer.Panel2.Controls.Add(this._rightHandPanel);
			this._reportEditorSplitContainer.Size = new System.Drawing.Size(977, 801);
			this._reportEditorSplitContainer.SplitterDistance = 479;
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
			this.tableLayoutPanel2.Controls.Add(this._hasErrors, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this._imagesUnavailable, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this._cancelButton, 1, 5);
			this.tableLayoutPanel2.Controls.Add(this._reportEditorPanel, 0, 3);
			this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 5);
			this.tableLayoutPanel2.Controls.Add(this._reportedProcedures, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 4);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 6;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(479, 801);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// _hasErrors
			// 
			this._hasErrors.AutoSize = true;
			this._hasErrors.BackColor = System.Drawing.Color.White;
			this._hasErrors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel2.SetColumnSpan(this._hasErrors, 2);
			this._hasErrors.Dock = System.Windows.Forms.DockStyle.Fill;
			this._hasErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._hasErrors.ForeColor = System.Drawing.Color.Red;
			this._hasErrors.Location = new System.Drawing.Point(3, 31);
			this._hasErrors.Margin = new System.Windows.Forms.Padding(3);
			this._hasErrors.Name = "_hasErrors";
			this._hasErrors.Padding = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this._hasErrors.Size = new System.Drawing.Size(473, 22);
			this._hasErrors.TabIndex = 5;
			this._hasErrors.Text = "Transcription has flagged the report.";
			this._hasErrors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _imagesUnavailable
			// 
			this._imagesUnavailable.AutoSize = true;
			this._imagesUnavailable.BackColor = System.Drawing.Color.White;
			this._imagesUnavailable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel2.SetColumnSpan(this._imagesUnavailable, 2);
			this._imagesUnavailable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._imagesUnavailable.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._imagesUnavailable.ForeColor = System.Drawing.Color.Red;
			this._imagesUnavailable.Location = new System.Drawing.Point(3, 3);
			this._imagesUnavailable.Margin = new System.Windows.Forms.Padding(3);
			this._imagesUnavailable.Name = "_imagesUnavailable";
			this._imagesUnavailable.Padding = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this._imagesUnavailable.Size = new System.Drawing.Size(473, 22);
			this._imagesUnavailable.TabIndex = 3;
			this._imagesUnavailable.Text = "Images cannot be opened.";
			this._imagesUnavailable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _supervisor
			// 
			this._supervisor.AutoSize = true;
			this._supervisor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._supervisor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._supervisor.LabelText = "Supervising Radiologist (if applicable):";
			this._supervisor.Location = new System.Drawing.Point(2, 2);
			this._supervisor.Margin = new System.Windows.Forms.Padding(2);
			this._supervisor.Name = "_supervisor";
			this._supervisor.Size = new System.Drawing.Size(365, 41);
			this._supervisor.TabIndex = 1;
			this._supervisor.Value = null;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(401, 746);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 3;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _reportEditorPanel
			// 
			this._reportEditorPanel.AutoSize = true;
			this._reportEditorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.SetColumnSpan(this._reportEditorPanel, 2);
			this._reportEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reportEditorPanel.Location = new System.Drawing.Point(3, 80);
			this._reportEditorPanel.Name = "_reportEditorPanel";
			this._reportEditorPanel.Size = new System.Drawing.Size(473, 615);
			this._reportEditorPanel.TabIndex = 0;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._verifyButton);
			this.flowLayoutPanel1.Controls.Add(this._sendToVerifyButton);
			this.flowLayoutPanel1.Controls.Add(this._sendToTranscriptionButton);
			this.flowLayoutPanel1.Controls.Add(this._saveButton);
			this.flowLayoutPanel1.Controls.Add(this._btnSkip);
			this.flowLayoutPanel1.Controls.Add(this._reportNextItem);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 743);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(398, 58);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// _verifyButton
			// 
			this._verifyButton.Location = new System.Drawing.Point(3, 3);
			this._verifyButton.Name = "_verifyButton";
			this._verifyButton.Size = new System.Drawing.Size(75, 23);
			this._verifyButton.TabIndex = 0;
			this._verifyButton.Text = "Verify";
			this._verifyButton.UseVisualStyleBackColor = true;
			this._verifyButton.Click += new System.EventHandler(this._verifyButton_Click);
			// 
			// _sendToVerifyButton
			// 
			this._sendToVerifyButton.Location = new System.Drawing.Point(84, 3);
			this._sendToVerifyButton.Name = "_sendToVerifyButton";
			this._sendToVerifyButton.Size = new System.Drawing.Size(83, 23);
			this._sendToVerifyButton.TabIndex = 1;
			this._sendToVerifyButton.Text = "For Review";
			this._sendToVerifyButton.UseVisualStyleBackColor = true;
			this._sendToVerifyButton.Click += new System.EventHandler(this._sendToVerifyButton_Click);
			// 
			// _sendToTranscriptionButton
			// 
			this._sendToTranscriptionButton.Location = new System.Drawing.Point(173, 3);
			this._sendToTranscriptionButton.Name = "_sendToTranscriptionButton";
			this._sendToTranscriptionButton.Size = new System.Drawing.Size(145, 23);
			this._sendToTranscriptionButton.TabIndex = 2;
			this._sendToTranscriptionButton.Text = "Send to Transcription";
			this._sendToTranscriptionButton.UseVisualStyleBackColor = true;
			this._sendToTranscriptionButton.Click += new System.EventHandler(this._sendToTranscriptionButton_Click);
			// 
			// _saveButton
			// 
			this._saveButton.Location = new System.Drawing.Point(3, 32);
			this._saveButton.Name = "_saveButton";
			this._saveButton.Size = new System.Drawing.Size(75, 23);
			this._saveButton.TabIndex = 3;
			this._saveButton.Text = "Save";
			this._saveButton.UseVisualStyleBackColor = true;
			this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
			// 
			// _btnSkip
			// 
			this._btnSkip.Location = new System.Drawing.Point(84, 32);
			this._btnSkip.Name = "_btnSkip";
			this._btnSkip.Size = new System.Drawing.Size(75, 23);
			this._btnSkip.TabIndex = 7;
			this._btnSkip.Text = "Skip";
			this._btnSkip.UseVisualStyleBackColor = true;
			this._btnSkip.Click += new System.EventHandler(this._btnSkip_Click);
			// 
			// _reportNextItem
			// 
			this._reportNextItem.AutoSize = true;
			this._reportNextItem.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reportNextItem.Location = new System.Drawing.Point(165, 32);
			this._reportNextItem.Name = "_reportNextItem";
			this._reportNextItem.Size = new System.Drawing.Size(104, 23);
			this._reportNextItem.TabIndex = 8;
			this._reportNextItem.Text = "Go To Next Item";
			this._reportNextItem.UseVisualStyleBackColor = true;
			// 
			// _reportedProcedures
			// 
			this._reportedProcedures.AutoSize = true;
			this._reportedProcedures.BackColor = System.Drawing.SystemColors.Control;
			this.tableLayoutPanel2.SetColumnSpan(this._reportedProcedures, 2);
			this._reportedProcedures.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reportedProcedures.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._reportedProcedures.ForeColor = System.Drawing.SystemColors.ControlText;
			this._reportedProcedures.Location = new System.Drawing.Point(3, 56);
			this._reportedProcedures.Name = "_reportedProcedures";
			this._reportedProcedures.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._reportedProcedures.Size = new System.Drawing.Size(473, 21);
			this._reportedProcedures.TabIndex = 4;
			this._reportedProcedures.Text = "Reported Procedure(s): ";
			// 
			// _rightHandPanel
			// 
			this._rightHandPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._rightHandPanel.Location = new System.Drawing.Point(0, 0);
			this._rightHandPanel.Name = "_rightHandPanel";
			this._rightHandPanel.Size = new System.Drawing.Size(494, 801);
			this._rightHandPanel.TabIndex = 0;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._statusText, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._reportEditorSplitContainer, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._bannerPanel, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(983, 917);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _statusText
			// 
			this._statusText.AutoSize = true;
			this._statusText.BackColor = System.Drawing.Color.LightSteelBlue;
			this._statusText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._statusText.Dock = System.Windows.Forms.DockStyle.Fill;
			this._statusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._statusText.ForeColor = System.Drawing.SystemColors.ControlText;
			this._statusText.Location = new System.Drawing.Point(3, 88);
			this._statusText.Margin = new System.Windows.Forms.Padding(3);
			this._statusText.Name = "_statusText";
			this._statusText.Padding = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this._statusText.Size = new System.Drawing.Size(977, 19);
			this._statusText.TabIndex = 2;
			this._statusText.Text = "Reporting from X worklist - Y items available - Z items completed";
			// 
			// _bannerPanel
			// 
			this._bannerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bannerPanel.Location = new System.Drawing.Point(3, 3);
			this._bannerPanel.Name = "_bannerPanel";
			this._bannerPanel.Size = new System.Drawing.Size(977, 79);
			this._bannerPanel.TabIndex = 0;
			// 
			// _rememberSupervisorCheckbox
			// 
			this._rememberSupervisorCheckbox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._rememberSupervisorCheckbox.Location = new System.Drawing.Point(372, 3);
			this._rememberSupervisorCheckbox.Name = "_rememberSupervisorCheckbox";
			this._rememberSupervisorCheckbox.Size = new System.Drawing.Size(104, 39);
			this._rememberSupervisorCheckbox.TabIndex = 2;
			this._rememberSupervisorCheckbox.Text = "Remember Supervisor?";
			this._rememberSupervisorCheckbox.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel3, 2);
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.Controls.Add(this._rememberSupervisorCheckbox, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this._supervisor, 0, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 698);
			this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(479, 45);
			this.tableLayoutPanel3.TabIndex = 6;
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
			this.flowLayoutPanel1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.SplitContainer _reportEditorSplitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel _bannerPanel;
        private System.Windows.Forms.Panel _reportEditorPanel;
        private System.Windows.Forms.Button _verifyButton;
        private System.Windows.Forms.Button _sendToVerifyButton;
        private System.Windows.Forms.Button _sendToTranscriptionButton;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.Button _cancelButton;
		private ClearCanvas.Ris.Client.View.WinForms.LookupField _supervisor;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _btnSkip;
		private System.Windows.Forms.CheckBox _reportNextItem;
		private System.Windows.Forms.Panel _rightHandPanel;
		private System.Windows.Forms.Label _reportedProcedures;
		private System.Windows.Forms.Label _imagesUnavailable;
		private System.Windows.Forms.Label _statusText;
		private System.Windows.Forms.Label _hasErrors;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.CheckBox _rememberSupervisorCheckbox;
    }
}
