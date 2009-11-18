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
    partial class PerformedProcedureComponentControl
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
			this.splitContainerDocumentationDetails = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._mppsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._procedurePlanSummary = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._mppsDetailsPanel = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitContainerDocumentationDetails.Panel1.SuspendLayout();
			this.splitContainerDocumentationDetails.Panel2.SuspendLayout();
			this.splitContainerDocumentationDetails.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainerDocumentationDetails
			// 
			this.splitContainerDocumentationDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerDocumentationDetails.Location = new System.Drawing.Point(4, 4);
			this.splitContainerDocumentationDetails.Name = "splitContainerDocumentationDetails";
			this.splitContainerDocumentationDetails.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerDocumentationDetails.Panel1
			// 
			this.splitContainerDocumentationDetails.Panel1.Controls.Add(this.tableLayoutPanel1);
			// 
			// splitContainerDocumentationDetails.Panel2
			// 
			this.splitContainerDocumentationDetails.Panel2.Controls.Add(this._mppsDetailsPanel);
			this.splitContainerDocumentationDetails.Size = new System.Drawing.Size(750, 498);
			this.splitContainerDocumentationDetails.SplitterDistance = 112;
			this.splitContainerDocumentationDetails.TabIndex = 1;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this._mppsTableView, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this._procedurePlanSummary, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(750, 112);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _mppsTableView
			// 
			this._mppsTableView.AutoSize = true;
			this._mppsTableView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._mppsTableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._mppsTableView.Location = new System.Drawing.Point(378, 3);
			this._mppsTableView.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this._mppsTableView.MultiSelect = false;
			this._mppsTableView.Name = "_mppsTableView";
			this._mppsTableView.ReadOnly = false;
			this._mppsTableView.Size = new System.Drawing.Size(372, 106);
			this._mppsTableView.TabIndex = 1;
			// 
			// _procedurePlanSummary
			// 
			this._procedurePlanSummary.AutoSize = true;
			this._procedurePlanSummary.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._procedurePlanSummary.Dock = System.Windows.Forms.DockStyle.Fill;
			this._procedurePlanSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._procedurePlanSummary.Location = new System.Drawing.Point(0, 3);
			this._procedurePlanSummary.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this._procedurePlanSummary.Name = "_procedurePlanSummary";
			this._procedurePlanSummary.ReadOnly = false;
			this._procedurePlanSummary.Size = new System.Drawing.Size(372, 106);
			this._procedurePlanSummary.TabIndex = 0;
			// 
			// _mppsDetailsPanel
			// 
			this._mppsDetailsPanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this._mppsDetailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._mppsDetailsPanel.Location = new System.Drawing.Point(0, 0);
			this._mppsDetailsPanel.Name = "_mppsDetailsPanel";
			this._mppsDetailsPanel.Padding = new System.Windows.Forms.Padding(1);
			this._mppsDetailsPanel.Size = new System.Drawing.Size(750, 382);
			this._mppsDetailsPanel.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Control;
			this.panel1.Controls.Add(this.splitContainerDocumentationDetails);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(1, 1);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 5);
			this.panel1.Size = new System.Drawing.Size(758, 507);
			this.panel1.TabIndex = 2;
			// 
			// PerformedProcedureComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Controls.Add(this.panel1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "PerformedProcedureComponentControl";
			this.Padding = new System.Windows.Forms.Padding(1);
			this.Size = new System.Drawing.Size(760, 509);
			this.splitContainerDocumentationDetails.Panel1.ResumeLayout(false);
			this.splitContainerDocumentationDetails.Panel1.PerformLayout();
			this.splitContainerDocumentationDetails.Panel2.ResumeLayout(false);
			this.splitContainerDocumentationDetails.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerDocumentationDetails;
        private ClearCanvas.Desktop.View.WinForms.TableView _mppsTableView;
        private System.Windows.Forms.Panel _mppsDetailsPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.TableView _procedurePlanSummary;
		private System.Windows.Forms.Panel panel1;

    }
}
