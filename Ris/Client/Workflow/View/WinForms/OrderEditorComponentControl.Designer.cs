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
    partial class OrderEditorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderEditorComponentControl));
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._orderingPractitionerContactPoint = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this._visit = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._visitSummaryButton = new System.Windows.Forms.Button();
			this._reorderReason = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this._proceduresTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._orderNotesTab = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._recipientsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.panel2 = new System.Windows.Forms.Panel();
			this._consultantContactPoint = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._consultantLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._addConsultantButton = new System.Windows.Forms.Button();
			this._schedulingRequestTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._diagnosticService = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._schedulingRequestDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._priority = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._orderingPractitioner = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._indication = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._orderingFacility = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._rightHandPanel = new System.Windows.Forms.Panel();
			this._bannerPanel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(942, 2);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Location = new System.Drawing.Point(863, 2);
			this._acceptButton.Margin = new System.Windows.Forms.Padding(2);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 0;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._placeOrderButton_Click);
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.splitContainer1, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this._bannerPanel, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 3;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(1025, 767);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._acceptButton);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 737);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.flowLayoutPanel1.Size = new System.Drawing.Size(1019, 27);
			this.flowLayoutPanel1.TabIndex = 1;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(2, 87);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel3);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._rightHandPanel);
			this.splitContainer1.Size = new System.Drawing.Size(1021, 645);
			this.splitContainer1.SplitterDistance = 510;
			this.splitContainer1.SplitterWidth = 3;
			this.splitContainer1.TabIndex = 26;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoScroll = true;
			this.tableLayoutPanel3.AutoScrollMinSize = new System.Drawing.Size(-1, 450);
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.Controls.Add(this._orderingPractitionerContactPoint, 0, 3);
			this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 5);
			this.tableLayoutPanel3.Controls.Add(this._reorderReason, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.tabControl1, 0, 7);
			this.tableLayoutPanel3.Controls.Add(this._schedulingRequestTime, 1, 6);
			this.tableLayoutPanel3.Controls.Add(this._diagnosticService, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this._schedulingRequestDate, 0, 6);
			this.tableLayoutPanel3.Controls.Add(this._priority, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this._orderingPractitioner, 1, 2);
			this.tableLayoutPanel3.Controls.Add(this._indication, 0, 4);
			this.tableLayoutPanel3.Controls.Add(this._orderingFacility, 0, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
			this.tableLayoutPanel3.RowCount = 8;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(510, 645);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// _orderingPractitionerContactPoint
			// 
			this.tableLayoutPanel3.SetColumnSpan(this._orderingPractitionerContactPoint, 2);
			this._orderingPractitionerContactPoint.DataSource = null;
			this._orderingPractitionerContactPoint.DisplayMember = "";
			this._orderingPractitionerContactPoint.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderingPractitionerContactPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._orderingPractitionerContactPoint.LabelText = "Ordering Practitioner Contact Point";
			this._orderingPractitionerContactPoint.Location = new System.Drawing.Point(2, 137);
			this._orderingPractitionerContactPoint.Margin = new System.Windows.Forms.Padding(2);
			this._orderingPractitionerContactPoint.Name = "_orderingPractitionerContactPoint";
			this._orderingPractitionerContactPoint.Size = new System.Drawing.Size(491, 46);
			this._orderingPractitionerContactPoint.TabIndex = 5;
			this._orderingPractitionerContactPoint.Value = null;
			// 
			// tableLayoutPanel4
			// 
			this.tableLayoutPanel4.AutoSize = true;
			this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel4.ColumnCount = 2;
			this.tableLayoutPanel3.SetColumnSpan(this.tableLayoutPanel4, 2);
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.Controls.Add(this._visit, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this._visitSummaryButton, 1, 0);
			this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 229);
			this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.RowCount = 1;
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.Size = new System.Drawing.Size(495, 45);
			this.tableLayoutPanel4.TabIndex = 7;
			// 
			// _visit
			// 
			this._visit.DataSource = null;
			this._visit.DisplayMember = "";
			this._visit.Dock = System.Windows.Forms.DockStyle.Fill;
			this._visit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._visit.LabelText = "Visit";
			this._visit.Location = new System.Drawing.Point(2, 2);
			this._visit.Margin = new System.Windows.Forms.Padding(2);
			this._visit.Name = "_visit";
			this._visit.Size = new System.Drawing.Size(461, 41);
			this._visit.TabIndex = 0;
			this._visit.Value = null;
			// 
			// _visitSummaryButton
			// 
			this._visitSummaryButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._visitSummaryButton.Image = ((System.Drawing.Image)(resources.GetObject("_visitSummaryButton.Image")));
			this._visitSummaryButton.Location = new System.Drawing.Point(468, 18);
			this._visitSummaryButton.Name = "_visitSummaryButton";
			this._visitSummaryButton.Size = new System.Drawing.Size(24, 24);
			this._visitSummaryButton.TabIndex = 1;
			this._visitSummaryButton.UseVisualStyleBackColor = true;
			this._visitSummaryButton.Click += new System.EventHandler(this._visitSummaryButton_Click);
			// 
			// _reorderReason
			// 
			this._reorderReason.AutoSize = true;
			this._reorderReason.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._reorderReason.DataSource = null;
			this._reorderReason.DisplayMember = "";
			this._reorderReason.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reorderReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._reorderReason.LabelText = "Re-order Reason";
			this._reorderReason.Location = new System.Drawing.Point(249, 2);
			this._reorderReason.Margin = new System.Windows.Forms.Padding(2);
			this._reorderReason.Name = "_reorderReason";
			this._reorderReason.Size = new System.Drawing.Size(244, 41);
			this._reorderReason.TabIndex = 1;
			this._reorderReason.Value = null;
			// 
			// tabControl1
			// 
			this.tableLayoutPanel3.SetColumnSpan(this.tabControl1, 2);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this._orderNotesTab);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(4, 320);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 2, 2, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(489, 325);
			this.tabControl1.TabIndex = 10;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this._proceduresTableView);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
			this.tabPage1.Size = new System.Drawing.Size(481, 299);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Procedures";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// _proceduresTableView
			// 
			this._proceduresTableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._proceduresTableView.FilterTextBoxWidth = 132;
			this._proceduresTableView.Location = new System.Drawing.Point(2, 2);
			this._proceduresTableView.Margin = new System.Windows.Forms.Padding(4);
			this._proceduresTableView.MultiSelect = false;
			this._proceduresTableView.Name = "_proceduresTableView";
			this._proceduresTableView.ReadOnly = false;
			this._proceduresTableView.ShowToolbar = false;
			this._proceduresTableView.Size = new System.Drawing.Size(477, 295);
			this._proceduresTableView.TabIndex = 0;
			this._proceduresTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._proceduresTableView.ItemDoubleClicked += new System.EventHandler(this._proceduresTableView_ItemDoubleClicked);
			// 
			// _orderNotesTab
			// 
			this._orderNotesTab.Location = new System.Drawing.Point(4, 22);
			this._orderNotesTab.Margin = new System.Windows.Forms.Padding(2);
			this._orderNotesTab.Name = "_orderNotesTab";
			this._orderNotesTab.Size = new System.Drawing.Size(481, 299);
			this._orderNotesTab.TabIndex = 2;
			this._orderNotesTab.Text = "Notes";
			this._orderNotesTab.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.tableLayoutPanel1);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
			this.tabPage2.Size = new System.Drawing.Size(481, 299);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Copies To";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._recipientsTableView, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(477, 295);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _recipientsTableView
			// 
			this._recipientsTableView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._recipientsTableView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._recipientsTableView.FilterTextBoxWidth = 132;
			this._recipientsTableView.Location = new System.Drawing.Point(4, 114);
			this._recipientsTableView.Margin = new System.Windows.Forms.Padding(4);
			this._recipientsTableView.MultiSelect = false;
			this._recipientsTableView.Name = "_recipientsTableView";
			this._recipientsTableView.ReadOnly = false;
			this._recipientsTableView.ShowToolbar = false;
			this._recipientsTableView.Size = new System.Drawing.Size(469, 182);
			this._recipientsTableView.TabIndex = 2;
			this._recipientsTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this._consultantContactPoint);
			this.panel2.Controls.Add(this._consultantLookup);
			this.panel2.Controls.Add(this._addConsultantButton);
			this.panel2.Location = new System.Drawing.Point(2, 2);
			this.panel2.Margin = new System.Windows.Forms.Padding(2);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(473, 106);
			this.panel2.TabIndex = 0;
			// 
			// _consultantContactPoint
			// 
			this._consultantContactPoint.DataSource = null;
			this._consultantContactPoint.DisplayMember = "";
			this._consultantContactPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._consultantContactPoint.LabelText = "Practitioner Contact Point";
			this._consultantContactPoint.Location = new System.Drawing.Point(2, 54);
			this._consultantContactPoint.Margin = new System.Windows.Forms.Padding(2);
			this._consultantContactPoint.Name = "_consultantContactPoint";
			this._consultantContactPoint.Size = new System.Drawing.Size(382, 41);
			this._consultantContactPoint.TabIndex = 6;
			this._consultantContactPoint.Value = null;
			// 
			// _consultantLookup
			// 
			this._consultantLookup.LabelText = "Find Practitioner";
			this._consultantLookup.Location = new System.Drawing.Point(2, 5);
			this._consultantLookup.Margin = new System.Windows.Forms.Padding(2);
			this._consultantLookup.Name = "_consultantLookup";
			this._consultantLookup.Size = new System.Drawing.Size(382, 45);
			this._consultantLookup.TabIndex = 4;
			this._consultantLookup.Value = null;
			// 
			// _addConsultantButton
			// 
			this._addConsultantButton.Location = new System.Drawing.Point(400, 42);
			this._addConsultantButton.Margin = new System.Windows.Forms.Padding(2);
			this._addConsultantButton.Name = "_addConsultantButton";
			this._addConsultantButton.Size = new System.Drawing.Size(75, 23);
			this._addConsultantButton.TabIndex = 5;
			this._addConsultantButton.Text = "Add";
			this._addConsultantButton.UseVisualStyleBackColor = true;
			this._addConsultantButton.Click += new System.EventHandler(this._addConsultantButton_Click);
			// 
			// _schedulingRequestTime
			// 
			this._schedulingRequestTime.AutoSize = true;
			this._schedulingRequestTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._schedulingRequestTime.Dock = System.Windows.Forms.DockStyle.Fill;
			this._schedulingRequestTime.LabelText = "Requested Schedule Time";
			this._schedulingRequestTime.Location = new System.Drawing.Point(249, 276);
			this._schedulingRequestTime.Margin = new System.Windows.Forms.Padding(2);
			this._schedulingRequestTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._schedulingRequestTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._schedulingRequestTime.Name = "_schedulingRequestTime";
			this._schedulingRequestTime.Nullable = true;
			this._schedulingRequestTime.ShowDate = false;
			this._schedulingRequestTime.ShowTime = true;
			this._schedulingRequestTime.Size = new System.Drawing.Size(244, 40);
			this._schedulingRequestTime.TabIndex = 9;
			this._schedulingRequestTime.Value = null;
			// 
			// _diagnosticService
			// 
			this._diagnosticService.AutoSize = true;
			this._diagnosticService.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.SetColumnSpan(this._diagnosticService, 2);
			this._diagnosticService.Dock = System.Windows.Forms.DockStyle.Fill;
			this._diagnosticService.LabelText = "Imaging Service";
			this._diagnosticService.Location = new System.Drawing.Point(2, 47);
			this._diagnosticService.Margin = new System.Windows.Forms.Padding(2);
			this._diagnosticService.Name = "_diagnosticService";
			this._diagnosticService.Size = new System.Drawing.Size(491, 41);
			this._diagnosticService.TabIndex = 2;
			this._diagnosticService.Value = null;
			// 
			// _schedulingRequestDate
			// 
			this._schedulingRequestDate.AutoSize = true;
			this._schedulingRequestDate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._schedulingRequestDate.Dock = System.Windows.Forms.DockStyle.Fill;
			this._schedulingRequestDate.LabelText = "Requested Schedule Date";
			this._schedulingRequestDate.Location = new System.Drawing.Point(2, 276);
			this._schedulingRequestDate.Margin = new System.Windows.Forms.Padding(2);
			this._schedulingRequestDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._schedulingRequestDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._schedulingRequestDate.Name = "_schedulingRequestDate";
			this._schedulingRequestDate.Nullable = true;
			this._schedulingRequestDate.Size = new System.Drawing.Size(243, 40);
			this._schedulingRequestDate.TabIndex = 8;
			this._schedulingRequestDate.Value = null;
			// 
			// _priority
			// 
			this._priority.AutoSize = true;
			this._priority.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._priority.DataSource = null;
			this._priority.DisplayMember = "";
			this._priority.Dock = System.Windows.Forms.DockStyle.Fill;
			this._priority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._priority.LabelText = "Priority";
			this._priority.Location = new System.Drawing.Point(2, 92);
			this._priority.Margin = new System.Windows.Forms.Padding(2);
			this._priority.Name = "_priority";
			this._priority.Size = new System.Drawing.Size(243, 41);
			this._priority.TabIndex = 3;
			this._priority.Value = null;
			// 
			// _orderingPractitioner
			// 
			this._orderingPractitioner.AutoSize = true;
			this._orderingPractitioner.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._orderingPractitioner.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderingPractitioner.LabelText = "Ordering Practitioner";
			this._orderingPractitioner.Location = new System.Drawing.Point(249, 92);
			this._orderingPractitioner.Margin = new System.Windows.Forms.Padding(2);
			this._orderingPractitioner.Name = "_orderingPractitioner";
			this._orderingPractitioner.Size = new System.Drawing.Size(244, 41);
			this._orderingPractitioner.TabIndex = 4;
			this._orderingPractitioner.Value = null;
			// 
			// _indication
			// 
			this._indication.AutoSize = true;
			this._indication.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.SetColumnSpan(this._indication, 2);
			this._indication.Dock = System.Windows.Forms.DockStyle.Fill;
			this._indication.LabelText = "Indication";
			this._indication.Location = new System.Drawing.Point(2, 187);
			this._indication.Margin = new System.Windows.Forms.Padding(2);
			this._indication.Mask = "";
			this._indication.Name = "_indication";
			this._indication.PasswordChar = '\0';
			this._indication.Size = new System.Drawing.Size(491, 40);
			this._indication.TabIndex = 6;
			this._indication.ToolTip = null;
			this._indication.Value = null;
			// 
			// _orderingFacility
			// 
			this._orderingFacility.AutoSize = true;
			this._orderingFacility.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._orderingFacility.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderingFacility.LabelText = "Ordering Facility";
			this._orderingFacility.Location = new System.Drawing.Point(2, 2);
			this._orderingFacility.Margin = new System.Windows.Forms.Padding(2);
			this._orderingFacility.Mask = "";
			this._orderingFacility.Name = "_orderingFacility";
			this._orderingFacility.PasswordChar = '\0';
			this._orderingFacility.ReadOnly = true;
			this._orderingFacility.Size = new System.Drawing.Size(243, 41);
			this._orderingFacility.TabIndex = 0;
			this._orderingFacility.ToolTip = null;
			this._orderingFacility.Value = null;
			// 
			// _rightHandPanel
			// 
			this._rightHandPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._rightHandPanel.Location = new System.Drawing.Point(0, 0);
			this._rightHandPanel.Name = "_rightHandPanel";
			this._rightHandPanel.Size = new System.Drawing.Size(508, 645);
			this._rightHandPanel.TabIndex = 0;
			// 
			// _bannerPanel
			// 
			this._bannerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._bannerPanel.Location = new System.Drawing.Point(3, 3);
			this._bannerPanel.Name = "_bannerPanel";
			this._bannerPanel.Size = new System.Drawing.Size(1019, 79);
			this._bannerPanel.TabIndex = 0;
			// 
			// OrderEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel2);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "OrderEditorComponentControl";
			this.Size = new System.Drawing.Size(1025, 767);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ClearCanvas.Desktop.View.WinForms.TableView _proceduresTableView;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage _orderNotesTab;
        private System.Windows.Forms.TabPage tabPage2;
        private ClearCanvas.Desktop.View.WinForms.TableView _recipientsTableView;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private ClearCanvas.Ris.Client.View.WinForms.LookupField _diagnosticService;
        private System.Windows.Forms.Button _addConsultantButton;
        private ClearCanvas.Ris.Client.View.WinForms.LookupField _consultantLookup;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _visit;
        private ClearCanvas.Ris.Client.View.WinForms.LookupField _orderingPractitioner;
        private ClearCanvas.Desktop.View.WinForms.TextField _indication;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _reorderReason;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _priority;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _schedulingRequestTime;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _schedulingRequestDate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button _visitSummaryButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _orderingFacility;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel _bannerPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Panel _rightHandPanel;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _orderingPractitionerContactPoint;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _consultantContactPoint;

    }
}
