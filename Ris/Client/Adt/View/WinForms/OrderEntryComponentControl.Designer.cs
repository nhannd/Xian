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
    partial class OrderEntryComponentControl
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
            ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection3 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection4 = new ClearCanvas.Desktop.Selection();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._scheduledTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._schedulingRequestTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._scheduledDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._schedulingRequestDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._visit = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._indication = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._orderingFacility = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._reorderReason = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._priority = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this._proceduresTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this._notesTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._consultantsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.panel2 = new System.Windows.Forms.Panel();
            this._addConsultantButton = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.tableView2 = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._cancelButton = new System.Windows.Forms.Button();
            this._placeOrderButton = new System.Windows.Forms.Button();
            this._orderingPractitioner = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
            this._diagnosticService = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
            this._consultantLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._scheduledTime);
            this.splitContainer1.Panel1.Controls.Add(this._schedulingRequestTime);
            this.splitContainer1.Panel1.Controls.Add(this._scheduledDate);
            this.splitContainer1.Panel1.Controls.Add(this._schedulingRequestDate);
            this.splitContainer1.Panel1.Controls.Add(this._visit);
            this.splitContainer1.Panel1.Controls.Add(this._orderingPractitioner);
            this.splitContainer1.Panel1.Controls.Add(this._indication);
            this.splitContainer1.Panel1.Controls.Add(this._orderingFacility);
            this.splitContainer1.Panel1.Controls.Add(this._reorderReason);
            this.splitContainer1.Panel1.Controls.Add(this._priority);
            this.splitContainer1.Panel1.Controls.Add(this._diagnosticService);
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl2);
            this.splitContainer1.Size = new System.Drawing.Size(1005, 608);
            this.splitContainer1.SplitterDistance = 506;
            this.splitContainer1.TabIndex = 26;
            // 
            // _scheduledTime
            // 
            this._scheduledTime.LabelText = "Scheduled Time";
            this._scheduledTime.Location = new System.Drawing.Point(214, 320);
            this._scheduledTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._scheduledTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._scheduledTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._scheduledTime.Name = "_scheduledTime";
            this._scheduledTime.Nullable = true;
            this._scheduledTime.ShowDate = false;
            this._scheduledTime.ShowTime = true;
            this._scheduledTime.Size = new System.Drawing.Size(183, 50);
            this._scheduledTime.TabIndex = 10;
            this._scheduledTime.Value = null;
            // 
            // _schedulingRequestTime
            // 
            this._schedulingRequestTime.LabelText = "Requested Schedule Time";
            this._schedulingRequestTime.Location = new System.Drawing.Point(214, 266);
            this._schedulingRequestTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._schedulingRequestTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._schedulingRequestTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._schedulingRequestTime.Name = "_schedulingRequestTime";
            this._schedulingRequestTime.Nullable = true;
            this._schedulingRequestTime.ShowDate = false;
            this._schedulingRequestTime.ShowTime = true;
            this._schedulingRequestTime.Size = new System.Drawing.Size(183, 50);
            this._schedulingRequestTime.TabIndex = 8;
            this._schedulingRequestTime.Value = null;
            // 
            // _scheduledDate
            // 
            this._scheduledDate.LabelText = "Scheduled Date";
            this._scheduledDate.Location = new System.Drawing.Point(11, 320);
            this._scheduledDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._scheduledDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._scheduledDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._scheduledDate.Name = "_scheduledDate";
            this._scheduledDate.Nullable = true;
            this._scheduledDate.Size = new System.Drawing.Size(197, 50);
            this._scheduledDate.TabIndex = 9;
            this._scheduledDate.Value = null;
            // 
            // _schedulingRequestDate
            // 
            this._schedulingRequestDate.LabelText = "Requested Schedule Date";
            this._schedulingRequestDate.Location = new System.Drawing.Point(11, 266);
            this._schedulingRequestDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._schedulingRequestDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._schedulingRequestDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._schedulingRequestDate.Name = "_schedulingRequestDate";
            this._schedulingRequestDate.Nullable = true;
            this._schedulingRequestDate.Size = new System.Drawing.Size(197, 50);
            this._schedulingRequestDate.TabIndex = 7;
            this._schedulingRequestDate.Value = null;
            // 
            // _visit
            // 
            this._visit.DataSource = null;
            this._visit.DisplayMember = "";
            this._visit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._visit.LabelText = "Visit";
            this._visit.Location = new System.Drawing.Point(6, 215);
            this._visit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._visit.Name = "_visit";
            this._visit.Size = new System.Drawing.Size(486, 50);
            this._visit.TabIndex = 6;
            this._visit.Value = null;
            // 
            // _indication
            // 
            this._indication.LabelText = "Indication";
            this._indication.Location = new System.Drawing.Point(6, 112);
            this._indication.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._indication.Mask = "";
            this._indication.Name = "_indication";
            this._indication.Size = new System.Drawing.Size(487, 50);
            this._indication.TabIndex = 3;
            this._indication.Value = null;
            // 
            // _orderingFacility
            // 
            this._orderingFacility.DataSource = null;
            this._orderingFacility.DisplayMember = "";
            this._orderingFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._orderingFacility.LabelText = "Ordering Facility (temporary)";
            this._orderingFacility.Location = new System.Drawing.Point(6, 166);
            this._orderingFacility.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._orderingFacility.Name = "_orderingFacility";
            this._orderingFacility.Size = new System.Drawing.Size(228, 50);
            this._orderingFacility.TabIndex = 4;
            this._orderingFacility.Value = null;
            // 
            // _reorderReason
            // 
            this._reorderReason.DataSource = null;
            this._reorderReason.DisplayMember = "";
            this._reorderReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._reorderReason.LabelText = "Re-order Reason";
            this._reorderReason.Location = new System.Drawing.Point(242, 166);
            this._reorderReason.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._reorderReason.Name = "_reorderReason";
            this._reorderReason.Size = new System.Drawing.Size(251, 50);
            this._reorderReason.TabIndex = 5;
            this._reorderReason.Value = null;
            // 
            // _priority
            // 
            this._priority.DataSource = null;
            this._priority.DisplayMember = "";
            this._priority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._priority.LabelText = "Priority";
            this._priority.Location = new System.Drawing.Point(7, 61);
            this._priority.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._priority.Name = "_priority";
            this._priority.Size = new System.Drawing.Size(175, 50);
            this._priority.TabIndex = 1;
            this._priority.Value = null;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(4, 375);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(499, 229);
            this.tabControl1.TabIndex = 35;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this._proceduresTableView);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(491, 200);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Procedures";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // _proceduresTableView
            // 
            this._proceduresTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._proceduresTableView.Location = new System.Drawing.Point(3, 3);
            this._proceduresTableView.Margin = new System.Windows.Forms.Padding(5);
            this._proceduresTableView.MenuModel = null;
            this._proceduresTableView.MultiSelect = false;
            this._proceduresTableView.Name = "_proceduresTableView";
            this._proceduresTableView.ReadOnly = false;
            this._proceduresTableView.Selection = selection1;
            this._proceduresTableView.Size = new System.Drawing.Size(485, 194);
            this._proceduresTableView.TabIndex = 36;
            this._proceduresTableView.Table = null;
            this._proceduresTableView.ToolbarModel = null;
            this._proceduresTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._proceduresTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this._notesTableView);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(491, 200);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Notes";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // _notesTableView
            // 
            this._notesTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._notesTableView.Location = new System.Drawing.Point(0, 0);
            this._notesTableView.Margin = new System.Windows.Forms.Padding(5);
            this._notesTableView.MenuModel = null;
            this._notesTableView.MultiLine = true;
            this._notesTableView.MultiSelect = false;
            this._notesTableView.Name = "_notesTableView";
            this._notesTableView.ReadOnly = false;
            this._notesTableView.Selection = selection2;
            this._notesTableView.ShowToolbar = false;
            this._notesTableView.Size = new System.Drawing.Size(491, 200);
            this._notesTableView.TabIndex = 1;
            this._notesTableView.Table = null;
            this._notesTableView.ToolbarModel = null;
            this._notesTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._notesTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(491, 200);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Copies To";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._consultantsTableView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(485, 194);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _consultantsTableView
            // 
            this._consultantsTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._consultantsTableView.Location = new System.Drawing.Point(5, 83);
            this._consultantsTableView.Margin = new System.Windows.Forms.Padding(5);
            this._consultantsTableView.MenuModel = null;
            this._consultantsTableView.MultiSelect = false;
            this._consultantsTableView.Name = "_consultantsTableView";
            this._consultantsTableView.ReadOnly = false;
            this._consultantsTableView.Selection = selection3;
            this._consultantsTableView.ShowToolbar = false;
            this._consultantsTableView.Size = new System.Drawing.Size(475, 106);
            this._consultantsTableView.TabIndex = 2;
            this._consultantsTableView.Table = null;
            this._consultantsTableView.ToolbarModel = null;
            this._consultantsTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._consultantsTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this._consultantLookup);
            this.panel2.Controls.Add(this._addConsultantButton);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(452, 72);
            this.panel2.TabIndex = 0;
            // 
            // _addConsultantButton
            // 
            this._addConsultantButton.Location = new System.Drawing.Point(368, 33);
            this._addConsultantButton.Name = "_addConsultantButton";
            this._addConsultantButton.Size = new System.Drawing.Size(75, 23);
            this._addConsultantButton.TabIndex = 5;
            this._addConsultantButton.Text = "Add";
            this._addConsultantButton.UseVisualStyleBackColor = true;
            this._addConsultantButton.Click += new System.EventHandler(this._addConsultantButton_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(495, 608);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panel1);
            this.tabPage4.Controls.Add(this.tableView2);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(487, 579);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "Documents";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.webBrowser1);
            this.panel1.Location = new System.Drawing.Point(8, 107);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(471, 469);
            this.panel1.TabIndex = 38;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(467, 465);
            this.webBrowser1.TabIndex = 0;
            // 
            // tableView2
            // 
            this.tableView2.Location = new System.Drawing.Point(8, 16);
            this.tableView2.Margin = new System.Windows.Forms.Padding(5);
            this.tableView2.MenuModel = null;
            this.tableView2.MultiSelect = false;
            this.tableView2.Name = "tableView2";
            this.tableView2.ReadOnly = false;
            this.tableView2.Selection = selection4;
            this.tableView2.Size = new System.Drawing.Size(471, 83);
            this.tableView2.TabIndex = 37;
            this.tableView2.Table = null;
            this.tableView2.ToolbarModel = null;
            this.tableView2.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tableView2.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(916, 616);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 29;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _placeOrderButton
            // 
            this._placeOrderButton.Location = new System.Drawing.Point(805, 616);
            this._placeOrderButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._placeOrderButton.Name = "_placeOrderButton";
            this._placeOrderButton.Size = new System.Drawing.Size(105, 23);
            this._placeOrderButton.TabIndex = 28;
            this._placeOrderButton.Text = "Place Order";
            this._placeOrderButton.UseVisualStyleBackColor = true;
            this._placeOrderButton.Click += new System.EventHandler(this._placeOrderButton_Click);
            // 
            // _orderingPractitioner
            // 
            this._orderingPractitioner.LabelText = "Ordering Practitioner";
            this._orderingPractitioner.Location = new System.Drawing.Point(201, 57);
            this._orderingPractitioner.Name = "_orderingPractitioner";
            this._orderingPractitioner.Size = new System.Drawing.Size(292, 59);
            this._orderingPractitioner.TabIndex = 2;
            this._orderingPractitioner.Value = null;
            // 
            // _diagnosticService
            // 
            this._diagnosticService.LabelText = "Diagnostic Service";
            this._diagnosticService.Location = new System.Drawing.Point(0, 3);
            this._diagnosticService.Name = "_diagnosticService";
            this._diagnosticService.Size = new System.Drawing.Size(492, 59);
            this._diagnosticService.TabIndex = 0;
            this._diagnosticService.Value = null;
            // 
            // _consultantLookup
            // 
            this._consultantLookup.LabelText = "Find Practitioner";
            this._consultantLookup.Location = new System.Drawing.Point(1, 6);
            this._consultantLookup.Name = "_consultantLookup";
            this._consultantLookup.Size = new System.Drawing.Size(348, 59);
            this._consultantLookup.TabIndex = 4;
            this._consultantLookup.Value = null;
            // 
            // OrderEntryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this._placeOrderButton);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "OrderEntryComponentControl";
            this.Size = new System.Drawing.Size(1011, 646);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ClearCanvas.Desktop.View.WinForms.TableView _proceduresTableView;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private ClearCanvas.Desktop.View.WinForms.TableView _notesTableView;
        private System.Windows.Forms.TabPage tabPage2;
        private ClearCanvas.Desktop.View.WinForms.TableView _consultantsTableView;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _placeOrderButton;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private ClearCanvas.Desktop.View.WinForms.TableView tableView2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private ClearCanvas.Ris.Client.View.WinForms.LookupField _diagnosticService;
        private System.Windows.Forms.Button _addConsultantButton;
        private ClearCanvas.Ris.Client.View.WinForms.LookupField _consultantLookup;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _visit;
        private ClearCanvas.Ris.Client.View.WinForms.LookupField _orderingPractitioner;
        private ClearCanvas.Desktop.View.WinForms.TextField _indication;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _orderingFacility;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _reorderReason;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _priority;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _scheduledTime;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _schedulingRequestTime;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _scheduledDate;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _schedulingRequestDate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;

    }
}
