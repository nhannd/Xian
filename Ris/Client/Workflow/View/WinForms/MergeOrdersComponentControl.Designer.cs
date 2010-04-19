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
    partial class MergeOrdersComponentControl
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
			this._acceptButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._mergeDirectionButton = new System.Windows.Forms.Button();
			this._mergedOrderPreviewPanel = new System.Windows.Forms.Panel();
			this._order1Accession = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._order2Accession = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._order1DiagnosticServiceName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._order2DiagnosticServiceName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _acceptButton
			// 
			this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._acceptButton.Location = new System.Drawing.Point(443, 667);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 2;
			this._acceptButton.Text = "Merge";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(524, 667);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 3;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _mergeDirectionButton
			// 
			this._mergeDirectionButton.Dock = System.Windows.Forms.DockStyle.Fill;
			this._mergeDirectionButton.Location = new System.Drawing.Point(272, 38);
			this._mergeDirectionButton.Name = "_mergeDirectionButton";
			this._mergeDirectionButton.Size = new System.Drawing.Size(54, 24);
			this._mergeDirectionButton.TabIndex = 6;
			this._mergeDirectionButton.Text = "--->";
			this._mergeDirectionButton.UseVisualStyleBackColor = true;
			this._mergeDirectionButton.Click += new System.EventHandler(this._mergeDirectionButton_Click);
			// 
			// _mergedOrderPreviewPanel
			// 
			this._mergedOrderPreviewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._mergedOrderPreviewPanel.Location = new System.Drawing.Point(3, 109);
			this._mergedOrderPreviewPanel.Name = "_mergedOrderPreviewPanel";
			this._mergedOrderPreviewPanel.Size = new System.Drawing.Size(599, 552);
			this._mergedOrderPreviewPanel.TabIndex = 7;
			// 
			// _order1Accession
			// 
			this._order1Accession.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._order1Accession.LabelText = "Accession #";
			this._order1Accession.Location = new System.Drawing.Point(2, 3);
			this._order1Accession.Margin = new System.Windows.Forms.Padding(2);
			this._order1Accession.Mask = "";
			this._order1Accession.Name = "_order1Accession";
			this._order1Accession.PasswordChar = '\0';
			this._order1Accession.ReadOnly = true;
			this._order1Accession.Size = new System.Drawing.Size(265, 39);
			this._order1Accession.TabIndex = 2;
			this._order1Accession.ToolTip = null;
			this._order1Accession.Value = null;
			// 
			// _order2Accession
			// 
			this._order2Accession.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._order2Accession.LabelText = "Accession #";
			this._order2Accession.Location = new System.Drawing.Point(2, 3);
			this._order2Accession.Margin = new System.Windows.Forms.Padding(2);
			this._order2Accession.Mask = "";
			this._order2Accession.Name = "_order2Accession";
			this._order2Accession.PasswordChar = '\0';
			this._order2Accession.ReadOnly = true;
			this._order2Accession.Size = new System.Drawing.Size(265, 39);
			this._order2Accession.TabIndex = 3;
			this._order2Accession.ToolTip = null;
			this._order2Accession.Value = null;
			// 
			// _order1DiagnosticServiceName
			// 
			this._order1DiagnosticServiceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._order1DiagnosticServiceName.LabelText = "Diagnostic Service";
			this._order1DiagnosticServiceName.Location = new System.Drawing.Point(2, 46);
			this._order1DiagnosticServiceName.Margin = new System.Windows.Forms.Padding(2);
			this._order1DiagnosticServiceName.Mask = "";
			this._order1DiagnosticServiceName.Name = "_order1DiagnosticServiceName";
			this._order1DiagnosticServiceName.PasswordChar = '\0';
			this._order1DiagnosticServiceName.ReadOnly = true;
			this._order1DiagnosticServiceName.Size = new System.Drawing.Size(265, 39);
			this._order1DiagnosticServiceName.TabIndex = 3;
			this._order1DiagnosticServiceName.ToolTip = null;
			this._order1DiagnosticServiceName.Value = null;
			// 
			// _order2DiagnosticServiceName
			// 
			this._order2DiagnosticServiceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._order2DiagnosticServiceName.LabelText = "Diagnostic Service";
			this._order2DiagnosticServiceName.Location = new System.Drawing.Point(2, 50);
			this._order2DiagnosticServiceName.Margin = new System.Windows.Forms.Padding(2);
			this._order2DiagnosticServiceName.Mask = "";
			this._order2DiagnosticServiceName.Name = "_order2DiagnosticServiceName";
			this._order2DiagnosticServiceName.PasswordChar = '\0';
			this._order2DiagnosticServiceName.ReadOnly = true;
			this._order2DiagnosticServiceName.Size = new System.Drawing.Size(265, 39);
			this._order2DiagnosticServiceName.TabIndex = 4;
			this._order2DiagnosticServiceName.ToolTip = null;
			this._order2DiagnosticServiceName.Value = null;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._mergeDirectionButton, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 2, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(599, 100);
			this.tableLayoutPanel1.TabIndex = 8;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this._order1Accession);
			this.panel2.Controls.Add(this._order1DiagnosticServiceName);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Margin = new System.Windows.Forms.Padding(0);
			this.panel2.Name = "panel2";
			this.tableLayoutPanel1.SetRowSpan(this.panel2, 3);
			this.panel2.Size = new System.Drawing.Size(269, 100);
			this.panel2.TabIndex = 8;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._order2DiagnosticServiceName);
			this.panel1.Controls.Add(this._order2Accession);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(329, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.tableLayoutPanel1.SetRowSpan(this.panel1, 3);
			this.panel1.Size = new System.Drawing.Size(270, 100);
			this.panel1.TabIndex = 7;
			// 
			// MergeOrdersComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this._mergedOrderPreviewPanel);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Name = "MergeOrdersComponentControl";
			this.Size = new System.Drawing.Size(605, 696);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _mergeDirectionButton;
		private System.Windows.Forms.Panel _mergedOrderPreviewPanel;
		private ClearCanvas.Desktop.View.WinForms.TextField _order1Accession;
		private ClearCanvas.Desktop.View.WinForms.TextField _order1DiagnosticServiceName;
		private ClearCanvas.Desktop.View.WinForms.TextField _order2DiagnosticServiceName;
		private ClearCanvas.Desktop.View.WinForms.TextField _order2Accession;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
    }
}
