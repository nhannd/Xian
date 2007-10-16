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

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class TechnologistDocumentationComponentControl
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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            this._tableLayoutPanelRoot = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._btnSave = new System.Windows.Forms.Button();
            this._btnComplete = new System.Windows.Forms.Button();
            this._splitContainerRoot = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._splitContainerOrderSummary = new System.Windows.Forms.SplitContainer();
            this._orderSummaryPanel = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this._tabPageProcedurePlan = new System.Windows.Forms.TabPage();
            this._procedurePlanSummary = new ClearCanvas.Desktop.View.WinForms.DecoratedTableView();
            this._tabPageAdditionalDetails = new System.Windows.Forms.TabPage();
            this._panelAdditionalDetails = new System.Windows.Forms.Panel();
            this._browserAdditionalDetails = new System.Windows.Forms.WebBrowser();
            this._tableLayoutPanelRoot.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this._splitContainerRoot.Panel1.SuspendLayout();
            this._splitContainerRoot.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this._splitContainerOrderSummary.Panel1.SuspendLayout();
            this._splitContainerOrderSummary.Panel2.SuspendLayout();
            this._splitContainerOrderSummary.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this._tabPageProcedurePlan.SuspendLayout();
            this._tabPageAdditionalDetails.SuspendLayout();
            this._panelAdditionalDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tableLayoutPanelRoot
            // 
            this._tableLayoutPanelRoot.AutoSize = true;
            this._tableLayoutPanelRoot.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._tableLayoutPanelRoot.ColumnCount = 1;
            this._tableLayoutPanelRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanelRoot.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this._tableLayoutPanelRoot.Controls.Add(this._splitContainerRoot, 0, 0);
            this._tableLayoutPanelRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanelRoot.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanelRoot.Name = "_tableLayoutPanelRoot";
            this._tableLayoutPanelRoot.RowCount = 2;
            this._tableLayoutPanelRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanelRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanelRoot.Size = new System.Drawing.Size(1033, 650);
            this._tableLayoutPanelRoot.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this._btnSave);
            this.flowLayoutPanel1.Controls.Add(this._btnComplete);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 618);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1027, 29);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // _btnSave
            // 
            this._btnSave.Location = new System.Drawing.Point(949, 3);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(75, 23);
            this._btnSave.TabIndex = 1;
            this._btnSave.Text = "Save";
            this._btnSave.UseVisualStyleBackColor = true;
            this._btnSave.Click += new System.EventHandler(this._btnSave_Click);
            // 
            // _btnComplete
            // 
            this._btnComplete.Location = new System.Drawing.Point(838, 3);
            this._btnComplete.Name = "_btnComplete";
            this._btnComplete.Size = new System.Drawing.Size(105, 23);
            this._btnComplete.TabIndex = 0;
            this._btnComplete.Text = "Complete";
            this._btnComplete.UseVisualStyleBackColor = true;
            this._btnComplete.Click += new System.EventHandler(this._btnComplete_Click);
            // 
            // _splitContainerRoot
            // 
            this._splitContainerRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitContainerRoot.Location = new System.Drawing.Point(3, 3);
            this._splitContainerRoot.Name = "_splitContainerRoot";
            // 
            // _splitContainerRoot.Panel1
            // 
            this._splitContainerRoot.Panel1.Controls.Add(this.groupBox1);
            this._splitContainerRoot.Size = new System.Drawing.Size(1027, 609);
            this._splitContainerRoot.SplitterDistance = 267;
            this._splitContainerRoot.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._splitContainerOrderSummary);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(267, 609);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Order Summary";
            // 
            // _splitContainerOrderSummary
            // 
            this._splitContainerOrderSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitContainerOrderSummary.Location = new System.Drawing.Point(3, 16);
            this._splitContainerOrderSummary.Name = "_splitContainerOrderSummary";
            this._splitContainerOrderSummary.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _splitContainerOrderSummary.Panel1
            // 
            this._splitContainerOrderSummary.Panel1.Controls.Add(this._orderSummaryPanel);
            // 
            // _splitContainerOrderSummary.Panel2
            // 
            this._splitContainerOrderSummary.Panel2.Controls.Add(this.tabControl1);
            this._splitContainerOrderSummary.Size = new System.Drawing.Size(261, 590);
            this._splitContainerOrderSummary.SplitterDistance = 298;
            this._splitContainerOrderSummary.TabIndex = 0;
            // 
            // _orderSummaryPanel
            // 
            this._orderSummaryPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._orderSummaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._orderSummaryPanel.Location = new System.Drawing.Point(0, 0);
            this._orderSummaryPanel.Name = "_orderSummaryPanel";
            this._orderSummaryPanel.Size = new System.Drawing.Size(261, 298);
            this._orderSummaryPanel.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this._tabPageProcedurePlan);
            this.tabControl1.Controls.Add(this._tabPageAdditionalDetails);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(261, 288);
            this.tabControl1.TabIndex = 0;
            // 
            // _tabPageProcedurePlan
            // 
            this._tabPageProcedurePlan.Controls.Add(this._procedurePlanSummary);
            this._tabPageProcedurePlan.Location = new System.Drawing.Point(4, 22);
            this._tabPageProcedurePlan.Name = "_tabPageProcedurePlan";
            this._tabPageProcedurePlan.Padding = new System.Windows.Forms.Padding(3);
            this._tabPageProcedurePlan.Size = new System.Drawing.Size(253, 262);
            this._tabPageProcedurePlan.TabIndex = 0;
            this._tabPageProcedurePlan.Text = "Procedure Plan";
            this._tabPageProcedurePlan.UseVisualStyleBackColor = true;
            // 
            // _procedurePlanSummary
            // 
            this._procedurePlanSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this._procedurePlanSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._procedurePlanSummary.Location = new System.Drawing.Point(3, 3);
            this._procedurePlanSummary.MenuModel = null;
            this._procedurePlanSummary.Name = "_procedurePlanSummary";
            this._procedurePlanSummary.ReadOnly = false;
            this._procedurePlanSummary.Selection = selection1;
            this._procedurePlanSummary.Size = new System.Drawing.Size(247, 256);
            this._procedurePlanSummary.TabIndex = 1;
            this._procedurePlanSummary.Table = null;
            this._procedurePlanSummary.ToolbarModel = null;
            this._procedurePlanSummary.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._procedurePlanSummary.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _tabPageAdditionalDetails
            // 
            this._tabPageAdditionalDetails.Controls.Add(this._panelAdditionalDetails);
            this._tabPageAdditionalDetails.Location = new System.Drawing.Point(4, 22);
            this._tabPageAdditionalDetails.Name = "_tabPageAdditionalDetails";
            this._tabPageAdditionalDetails.Padding = new System.Windows.Forms.Padding(3);
            this._tabPageAdditionalDetails.Size = new System.Drawing.Size(253, 262);
            this._tabPageAdditionalDetails.TabIndex = 1;
            this._tabPageAdditionalDetails.Text = "Additional Details";
            this._tabPageAdditionalDetails.UseVisualStyleBackColor = true;
            // 
            // _panelAdditionalDetails
            // 
            this._panelAdditionalDetails.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._panelAdditionalDetails.Controls.Add(this._browserAdditionalDetails);
            this._panelAdditionalDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelAdditionalDetails.Location = new System.Drawing.Point(3, 3);
            this._panelAdditionalDetails.Name = "_panelAdditionalDetails";
            this._panelAdditionalDetails.Size = new System.Drawing.Size(247, 256);
            this._panelAdditionalDetails.TabIndex = 0;
            // 
            // _browserAdditionalDetails
            // 
            this._browserAdditionalDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this._browserAdditionalDetails.Location = new System.Drawing.Point(0, 0);
            this._browserAdditionalDetails.MinimumSize = new System.Drawing.Size(20, 20);
            this._browserAdditionalDetails.Name = "_browserAdditionalDetails";
            this._browserAdditionalDetails.Size = new System.Drawing.Size(243, 252);
            this._browserAdditionalDetails.TabIndex = 0;
            // 
            // TechnologistDocumentationComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this._tableLayoutPanelRoot);
            this.Name = "TechnologistDocumentationComponentControl";
            this.Size = new System.Drawing.Size(1033, 650);
            this._tableLayoutPanelRoot.ResumeLayout(false);
            this._tableLayoutPanelRoot.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this._splitContainerRoot.Panel1.ResumeLayout(false);
            this._splitContainerRoot.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this._splitContainerOrderSummary.Panel1.ResumeLayout(false);
            this._splitContainerOrderSummary.Panel2.ResumeLayout(false);
            this._splitContainerOrderSummary.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this._tabPageProcedurePlan.ResumeLayout(false);
            this._tabPageAdditionalDetails.ResumeLayout(false);
            this._panelAdditionalDetails.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelRoot;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _btnComplete;
        private System.Windows.Forms.SplitContainer _splitContainerRoot;
        private System.Windows.Forms.SplitContainer _splitContainerOrderSummary;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage _tabPageProcedurePlan;
        private System.Windows.Forms.TabPage _tabPageAdditionalDetails;
        private System.Windows.Forms.Panel _orderSummaryPanel;
        private System.Windows.Forms.Panel _panelAdditionalDetails;
        private System.Windows.Forms.WebBrowser _browserAdditionalDetails;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button _btnSave;
        private ClearCanvas.Desktop.View.WinForms.DecoratedTableView _procedurePlanSummary;
    }
}
