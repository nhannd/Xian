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
            this.components = new System.ComponentModel.Container();
            ClearCanvas.Desktop.Selection selection4 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection5 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
            this._visitTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._orderingPhysician = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._orderingFacility = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._priority = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._diagnosticServiceBreakdown = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
            this._placeOrderButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._schedulingRequestDateTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._scheduleOrder = new System.Windows.Forms.CheckBox();
            this._diagnosticServiceTree = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
            this._selectedDiagnosticService = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this._reorderReason = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _visitTable
            // 
            this._visitTable.Location = new System.Drawing.Point(413, 33);
            this._visitTable.MenuModel = null;
            this._visitTable.MultiSelect = false;
            this._visitTable.Name = "_visitTable";
            this._visitTable.ReadOnly = false;
            this._visitTable.Selection = selection4;
            this._visitTable.Size = new System.Drawing.Size(416, 127);
            this._visitTable.TabIndex = 0;
            this._visitTable.Table = null;
            this._visitTable.ToolbarModel = null;
            this._visitTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._visitTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(413, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Visit";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Diagnostic Service Breakdown";
            // 
            // _orderingPhysician
            // 
            this._orderingPhysician.DataSource = null;
            this._orderingPhysician.DisplayMember = "";
            this._orderingPhysician.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._orderingPhysician.LabelText = "Ordering Physician";
            this._orderingPhysician.Location = new System.Drawing.Point(854, 74);
            this._orderingPhysician.Margin = new System.Windows.Forms.Padding(2);
            this._orderingPhysician.Name = "_orderingPhysician";
            this._orderingPhysician.Size = new System.Drawing.Size(150, 41);
            this._orderingPhysician.TabIndex = 5;
            this._orderingPhysician.Value = null;
            // 
            // _orderingFacility
            // 
            this._orderingFacility.DataSource = null;
            this._orderingFacility.DisplayMember = "";
            this._orderingFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._orderingFacility.LabelText = "Ordering Facility";
            this._orderingFacility.Location = new System.Drawing.Point(854, 142);
            this._orderingFacility.Margin = new System.Windows.Forms.Padding(2);
            this._orderingFacility.Name = "_orderingFacility";
            this._orderingFacility.Size = new System.Drawing.Size(150, 41);
            this._orderingFacility.TabIndex = 6;
            this._orderingFacility.Value = null;
            // 
            // _priority
            // 
            this._priority.DataSource = null;
            this._priority.DisplayMember = "";
            this._priority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._priority.LabelText = "Priority";
            this._priority.Location = new System.Drawing.Point(854, 13);
            this._priority.Margin = new System.Windows.Forms.Padding(2);
            this._priority.Name = "_priority";
            this._priority.Size = new System.Drawing.Size(150, 41);
            this._priority.TabIndex = 7;
            this._priority.Value = null;
            // 
            // _diagnosticServiceBreakdown
            // 
            this._diagnosticServiceBreakdown.AllowDrop = true;
            this._diagnosticServiceBreakdown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._diagnosticServiceBreakdown.AutoSize = true;
            this._diagnosticServiceBreakdown.Location = new System.Drawing.Point(2, 21);
            this._diagnosticServiceBreakdown.Margin = new System.Windows.Forms.Padding(2);
            this._diagnosticServiceBreakdown.MenuModel = null;
            this._diagnosticServiceBreakdown.Name = "_diagnosticServiceBreakdown";
            this._diagnosticServiceBreakdown.Selection = selection5;
            this._diagnosticServiceBreakdown.ShowToolbar = false;
            this._diagnosticServiceBreakdown.Size = new System.Drawing.Size(412, 122);
            this._diagnosticServiceBreakdown.TabIndex = 8;
            this._diagnosticServiceBreakdown.ToolbarModel = null;
            this._diagnosticServiceBreakdown.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._diagnosticServiceBreakdown.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._diagnosticServiceBreakdown.Tree = null;
            // 
            // _placeOrderButton
            // 
            this._placeOrderButton.Location = new System.Drawing.Point(866, 419);
            this._placeOrderButton.Margin = new System.Windows.Forms.Padding(2);
            this._placeOrderButton.Name = "_placeOrderButton";
            this._placeOrderButton.Size = new System.Drawing.Size(79, 19);
            this._placeOrderButton.TabIndex = 9;
            this._placeOrderButton.Text = "Place Order";
            this._placeOrderButton.UseVisualStyleBackColor = true;
            this._placeOrderButton.Click += new System.EventHandler(this._placeOrderButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(949, 419);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(56, 19);
            this._cancelButton.TabIndex = 10;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _schedulingRequestDateTime
            // 
            this._schedulingRequestDateTime.LabelText = "Requested Schedule Time";
            this._schedulingRequestDateTime.Location = new System.Drawing.Point(854, 217);
            this._schedulingRequestDateTime.Margin = new System.Windows.Forms.Padding(2);
            this._schedulingRequestDateTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._schedulingRequestDateTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._schedulingRequestDateTime.Name = "_schedulingRequestDateTime";
            this._schedulingRequestDateTime.Nullable = false;
            this._schedulingRequestDateTime.ShowTime = true;
            this._schedulingRequestDateTime.Size = new System.Drawing.Size(150, 41);
            this._schedulingRequestDateTime.TabIndex = 11;
            this._schedulingRequestDateTime.Value = null;
            // 
            // _scheduleOrder
            // 
            this._scheduleOrder.AutoSize = true;
            this._scheduleOrder.Location = new System.Drawing.Point(854, 195);
            this._scheduleOrder.Name = "_scheduleOrder";
            this._scheduleOrder.Size = new System.Drawing.Size(100, 17);
            this._scheduleOrder.TabIndex = 12;
            this._scheduleOrder.Text = "Schedule Order";
            this._scheduleOrder.UseVisualStyleBackColor = true;
            // 
            // _diagnosticServiceTree
            // 
            this._diagnosticServiceTree.AllowDrop = true;
            this._diagnosticServiceTree.Location = new System.Drawing.Point(3, 18);
            this._diagnosticServiceTree.Margin = new System.Windows.Forms.Padding(2);
            this._diagnosticServiceTree.MenuModel = null;
            this._diagnosticServiceTree.Name = "_diagnosticServiceTree";
            this._diagnosticServiceTree.Selection = selection2;
            this._diagnosticServiceTree.ShowToolbar = false;
            this._diagnosticServiceTree.Size = new System.Drawing.Size(385, 351);
            this._diagnosticServiceTree.TabIndex = 13;
            this._diagnosticServiceTree.ToolbarModel = null;
            this._diagnosticServiceTree.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._diagnosticServiceTree.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._diagnosticServiceTree.Tree = null;
            // 
            // _selectedDiagnosticService
            // 
            this._selectedDiagnosticService.Location = new System.Drawing.Point(5, 368);
            this._selectedDiagnosticService.Name = "_selectedDiagnosticService";
            this._selectedDiagnosticService.ReadOnly = true;
            this._selectedDiagnosticService.Size = new System.Drawing.Size(381, 20);
            this._selectedDiagnosticService.TabIndex = 14;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._diagnosticServiceTree);
            this.groupBox1.Controls.Add(this._selectedDiagnosticService);
            this.groupBox1.Location = new System.Drawing.Point(5, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(390, 395);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Diagnositc Service";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._diagnosticServiceBreakdown);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(413, 166);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(416, 145);
            this.panel1.TabIndex = 16;
            // 
            // _reorderReason
            // 
            this._reorderReason.DataSource = null;
            this._reorderReason.DisplayMember = "";
            this._reorderReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._reorderReason.LabelText = "Re-order Reason";
            this._reorderReason.Location = new System.Drawing.Point(854, 270);
            this._reorderReason.Margin = new System.Windows.Forms.Padding(2);
            this._reorderReason.Name = "_reorderReason";
            this._reorderReason.Size = new System.Drawing.Size(150, 41);
            this._reorderReason.TabIndex = 17;
            this._reorderReason.Value = null;
            // 
            // OrderEntryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._reorderReason);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._scheduleOrder);
            this.Controls.Add(this._schedulingRequestDateTime);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._placeOrderButton);
            this.Controls.Add(this._priority);
            this.Controls.Add(this._orderingFacility);
            this.Controls.Add(this._orderingPhysician);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._visitTable);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "OrderEntryComponentControl";
            this.Size = new System.Drawing.Size(1020, 449);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _visitTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _orderingPhysician;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _orderingFacility;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _priority;
        private ClearCanvas.Desktop.View.WinForms.BindingTreeView _diagnosticServiceBreakdown;
        private System.Windows.Forms.Button _placeOrderButton;
        private System.Windows.Forms.Button _cancelButton;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _schedulingRequestDateTime;
        private System.Windows.Forms.CheckBox _scheduleOrder;
        private ClearCanvas.Desktop.View.WinForms.BindingTreeView _diagnosticServiceTree;
        private System.Windows.Forms.TextBox _selectedDiagnosticService;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _reorderReason;
    }
}
