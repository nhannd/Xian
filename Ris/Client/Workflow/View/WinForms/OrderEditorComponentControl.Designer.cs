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
            this._overviewLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this._orderingPractitionerContactPoint = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this._visit = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._visitSummaryButton = new System.Windows.Forms.Button();
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
            this.panel1 = new System.Windows.Forms.Panel();
            this._downtimeAccession = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._reorderReason = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._rightHandPanel = new System.Windows.Forms.Panel();
            this._bannerPanel = new System.Windows.Forms.Panel();
            this._overviewLayoutPanel.SuspendLayout();
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
            this.panel1.SuspendLayout();
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
            // _overviewLayoutPanel
            // 
            this._overviewLayoutPanel.AutoSize = true;
            this._overviewLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._overviewLayoutPanel.ColumnCount = 1;
            this._overviewLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._overviewLayoutPanel.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this._overviewLayoutPanel.Controls.Add(this.splitContainer1, 0, 1);
            this._overviewLayoutPanel.Controls.Add(this._bannerPanel, 0, 0);
            this._overviewLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._overviewLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._overviewLayoutPanel.Name = "_overviewLayoutPanel";
            this._overviewLayoutPanel.RowCount = 3;
            this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._overviewLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._overviewLayoutPanel.Size = new System.Drawing.Size(1025, 767);
            this._overviewLayoutPanel.TabIndex = 0;
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
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(2, 97);
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
            this.splitContainer1.Size = new System.Drawing.Size(1021, 635);
            this.splitContainer1.SplitterDistance = 510;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 1;
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
            this.tableLayoutPanel3.Controls.Add(this.tabControl1, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this._schedulingRequestTime, 1, 6);
            this.tableLayoutPanel3.Controls.Add(this._diagnosticService, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this._schedulingRequestDate, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this._priority, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this._orderingPractitioner, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this._indication, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this._orderingFacility, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.panel1, 1, 0);
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
            this.tableLayoutPanel3.Size = new System.Drawing.Size(510, 635);
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
            this._orderingPractitionerContactPoint.Location = new System.Drawing.Point(2, 141);
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
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 233);
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
            // tabControl1
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.tabControl1, 2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this._orderNotesTab);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(4, 324);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 2, 2, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(489, 311);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this._proceduresTableView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this.tabPage1.Size = new System.Drawing.Size(481, 285);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Procedures";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // _proceduresTableView
            // 
            this._proceduresTableView.AutoSize = true;
            this._proceduresTableView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._proceduresTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._proceduresTableView.FilterTextBoxWidth = 132;
            this._proceduresTableView.Location = new System.Drawing.Point(2, 2);
            this._proceduresTableView.Margin = new System.Windows.Forms.Padding(4);
            this._proceduresTableView.MultiSelect = false;
            this._proceduresTableView.Name = "_proceduresTableView";
            this._proceduresTableView.ReadOnly = false;
            this._proceduresTableView.ShowToolbar = false;
            this._proceduresTableView.Size = new System.Drawing.Size(462, 281);
            this._proceduresTableView.TabIndex = 0;
            this._proceduresTableView.ItemDoubleClicked += new System.EventHandler(this._proceduresTableView_ItemDoubleClicked);
            // 
            // _orderNotesTab
            // 
            this._orderNotesTab.Location = new System.Drawing.Point(4, 22);
            this._orderNotesTab.Margin = new System.Windows.Forms.Padding(2);
            this._orderNotesTab.Name = "_orderNotesTab";
            this._orderNotesTab.Size = new System.Drawing.Size(481, 295);
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
            this.tabPage2.Size = new System.Drawing.Size(481, 295);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(477, 291);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _recipientsTableView
            // 
            this._recipientsTableView.AutoSize = true;
            this._recipientsTableView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._recipientsTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._recipientsTableView.FilterTextBoxWidth = 132;
            this._recipientsTableView.Location = new System.Drawing.Point(4, 114);
            this._recipientsTableView.Margin = new System.Windows.Forms.Padding(4);
            this._recipientsTableView.MultiSelect = false;
            this._recipientsTableView.Name = "_recipientsTableView";
            this._recipientsTableView.ReadOnly = false;
            this._recipientsTableView.ShowToolbar = false;
            this._recipientsTableView.Size = new System.Drawing.Size(469, 173);
            this._recipientsTableView.TabIndex = 2;
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
            this._addConsultantButton.Location = new System.Drawing.Point(388, 72);
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
            this._schedulingRequestTime.Location = new System.Drawing.Point(249, 280);
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
            this._diagnosticService.Size = new System.Drawing.Size(491, 43);
            this._diagnosticService.TabIndex = 2;
            this._diagnosticService.Value = null;
            // 
            // _schedulingRequestDate
            // 
            this._schedulingRequestDate.AutoSize = true;
            this._schedulingRequestDate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._schedulingRequestDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this._schedulingRequestDate.LabelText = "Requested Schedule Date";
            this._schedulingRequestDate.Location = new System.Drawing.Point(2, 280);
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
            this._priority.Location = new System.Drawing.Point(2, 94);
            this._priority.Margin = new System.Windows.Forms.Padding(2);
            this._priority.Name = "_priority";
            this._priority.Size = new System.Drawing.Size(243, 43);
            this._priority.TabIndex = 3;
            this._priority.Value = null;
            // 
            // _orderingPractitioner
            // 
            this._orderingPractitioner.AutoSize = true;
            this._orderingPractitioner.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._orderingPractitioner.Dock = System.Windows.Forms.DockStyle.Fill;
            this._orderingPractitioner.LabelText = "Ordering Practitioner";
            this._orderingPractitioner.Location = new System.Drawing.Point(249, 94);
            this._orderingPractitioner.Margin = new System.Windows.Forms.Padding(2);
            this._orderingPractitioner.Name = "_orderingPractitioner";
            this._orderingPractitioner.Size = new System.Drawing.Size(244, 43);
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
            this._indication.Location = new System.Drawing.Point(2, 191);
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
            // panel1
            // 
            this.panel1.Controls.Add(this._downtimeAccession);
            this.panel1.Controls.Add(this._reorderReason);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(250, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(242, 39);
            this.panel1.TabIndex = 1;
            // 
            // _downtimeAccession
            // 
            this._downtimeAccession.Dock = System.Windows.Forms.DockStyle.Fill;
            this._downtimeAccession.LabelText = "Downtime Accession #";
            this._downtimeAccession.Location = new System.Drawing.Point(0, 0);
            this._downtimeAccession.Margin = new System.Windows.Forms.Padding(2);
            this._downtimeAccession.Mask = "";
            this._downtimeAccession.Name = "_downtimeAccession";
            this._downtimeAccession.PasswordChar = '\0';
            this._downtimeAccession.Size = new System.Drawing.Size(242, 39);
            this._downtimeAccession.TabIndex = 1;
            this._downtimeAccession.ToolTip = null;
            this._downtimeAccession.Value = null;
            // 
            // _reorderReason
            // 
            this._reorderReason.DataSource = null;
            this._reorderReason.DisplayMember = "";
            this._reorderReason.Dock = System.Windows.Forms.DockStyle.Fill;
            this._reorderReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._reorderReason.LabelText = "Re-order Reason";
            this._reorderReason.Location = new System.Drawing.Point(0, 0);
            this._reorderReason.Margin = new System.Windows.Forms.Padding(2);
            this._reorderReason.Name = "_reorderReason";
            this._reorderReason.Size = new System.Drawing.Size(242, 39);
            this._reorderReason.TabIndex = 0;
            this._reorderReason.Value = null;
            // 
            // _rightHandPanel
            // 
            this._rightHandPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rightHandPanel.Location = new System.Drawing.Point(0, 0);
            this._rightHandPanel.Name = "_rightHandPanel";
            this._rightHandPanel.Size = new System.Drawing.Size(508, 635);
            this._rightHandPanel.TabIndex = 0;
            // 
            // _bannerPanel
            // 
            this._bannerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._bannerPanel.Location = new System.Drawing.Point(3, 3);
            this._bannerPanel.Name = "_bannerPanel";
            this._bannerPanel.Size = new System.Drawing.Size(1019, 89);
            this._bannerPanel.TabIndex = 0;
            // 
            // OrderEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._overviewLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "OrderEditorComponentControl";
            this.Size = new System.Drawing.Size(1025, 767);
            this.Load += new System.EventHandler(this.OrderEditorComponentControl_Load);
            this._overviewLayoutPanel.ResumeLayout(false);
            this._overviewLayoutPanel.PerformLayout();
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
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
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
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _priority;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _schedulingRequestTime;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _schedulingRequestDate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button _visitSummaryButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _orderingFacility;
        private System.Windows.Forms.TableLayoutPanel _overviewLayoutPanel;
        private System.Windows.Forms.Panel _bannerPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Panel _rightHandPanel;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _orderingPractitionerContactPoint;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _consultantContactPoint;
		private System.Windows.Forms.Panel panel1;
		private ClearCanvas.Desktop.View.WinForms.TextField _downtimeAccession;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _reorderReason;

    }
}
