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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class BiographyOrderHistoryComponentControl
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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            this._orderList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this._orderPage = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._mpsScheduledEndTime = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._mpsEndTime = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._mpsStartTime = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._mpsScheduledStartTime = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._mpsState = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._mpsPerformerStaff = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._mpsScheduledPerformerStaff = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._diagnosticServiceBreakdown = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this._modality = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._schedulingRequestDateTime = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._orderingFacility = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._orderingPhysician = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._priority = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._cancelReason = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._placerNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._accessionNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._reasonForStudy = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._visitPage = new System.Windows.Forms.TabPage();
            this._dischargeDateTime = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._admitDateTime = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._ambulatoryStatus = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._admissionType = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._patientType = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._visitStatus = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._patientClass = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._visitNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._preAdmitNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._vip = new System.Windows.Forms.CheckBox();
            this._documentPage = new System.Windows.Forms.TabPage();
            this._billingPage = new System.Windows.Forms.TabPage();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this._orderPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this._visitPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // _orderList
            // 
            this._orderList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._orderList.Location = new System.Drawing.Point(0, 0);
            this._orderList.Name = "_orderList";
            this._orderList.ReadOnly = false;
            this._orderList.Size = new System.Drawing.Size(589, 239);
            this._orderList.TabIndex = 0;
            this._orderList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._orderList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(589, 586);
            this.splitContainer1.SplitterDistance = 239;
            this.splitContainer1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this._orderPage);
            this.tabControl1.Controls.Add(this._visitPage);
            this.tabControl1.Controls.Add(this._documentPage);
            this.tabControl1.Controls.Add(this._billingPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(589, 343);
            this.tabControl1.TabIndex = 24;
            // 
            // _orderPage
            // 
            this._orderPage.Controls.Add(this.groupBox2);
            this._orderPage.Controls.Add(this._schedulingRequestDateTime);
            this._orderPage.Controls.Add(this._orderingFacility);
            this._orderPage.Controls.Add(this._orderingPhysician);
            this._orderPage.Controls.Add(this._priority);
            this._orderPage.Controls.Add(this._cancelReason);
            this._orderPage.Controls.Add(this._placerNumber);
            this._orderPage.Controls.Add(this._accessionNumber);
            this._orderPage.Controls.Add(this._reasonForStudy);
            this._orderPage.Location = new System.Drawing.Point(4, 22);
            this._orderPage.Name = "_orderPage";
            this._orderPage.Padding = new System.Windows.Forms.Padding(3);
            this._orderPage.Size = new System.Drawing.Size(581, 317);
            this._orderPage.TabIndex = 0;
            this._orderPage.Text = "Order Details";
            this._orderPage.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._mpsScheduledEndTime);
            this.groupBox2.Controls.Add(this._mpsEndTime);
            this.groupBox2.Controls.Add(this._mpsStartTime);
            this.groupBox2.Controls.Add(this._mpsScheduledStartTime);
            this.groupBox2.Controls.Add(this._mpsState);
            this.groupBox2.Controls.Add(this._mpsPerformerStaff);
            this.groupBox2.Controls.Add(this._mpsScheduledPerformerStaff);
            this.groupBox2.Controls.Add(this._diagnosticServiceBreakdown);
            this.groupBox2.Controls.Add(this._modality);
            this.groupBox2.Location = new System.Drawing.Point(6, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(564, 211);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Requested Procedures and Procedure Steps";
            // 
            // _mpsScheduledEndTime
            // 
            this._mpsScheduledEndTime.LabelText = "Scheduled End Time";
            this._mpsScheduledEndTime.Location = new System.Drawing.Point(279, 160);
            this._mpsScheduledEndTime.Margin = new System.Windows.Forms.Padding(2);
            this._mpsScheduledEndTime.Mask = "";
            this._mpsScheduledEndTime.Name = "_mpsScheduledEndTime";
            this._mpsScheduledEndTime.ReadOnly = true;
            this._mpsScheduledEndTime.Size = new System.Drawing.Size(136, 41);
            this._mpsScheduledEndTime.TabIndex = 16;
            this._mpsScheduledEndTime.Value = null;
            // 
            // _mpsEndTime
            // 
            this._mpsEndTime.LabelText = "End Time";
            this._mpsEndTime.Location = new System.Drawing.Point(419, 160);
            this._mpsEndTime.Margin = new System.Windows.Forms.Padding(2);
            this._mpsEndTime.Mask = "";
            this._mpsEndTime.Name = "_mpsEndTime";
            this._mpsEndTime.ReadOnly = true;
            this._mpsEndTime.Size = new System.Drawing.Size(136, 41);
            this._mpsEndTime.TabIndex = 15;
            this._mpsEndTime.Value = null;
            // 
            // _mpsStartTime
            // 
            this._mpsStartTime.LabelText = "Start Time";
            this._mpsStartTime.Location = new System.Drawing.Point(419, 108);
            this._mpsStartTime.Margin = new System.Windows.Forms.Padding(2);
            this._mpsStartTime.Mask = "";
            this._mpsStartTime.Name = "_mpsStartTime";
            this._mpsStartTime.ReadOnly = true;
            this._mpsStartTime.Size = new System.Drawing.Size(136, 41);
            this._mpsStartTime.TabIndex = 14;
            this._mpsStartTime.Value = null;
            // 
            // _mpsScheduledStartTime
            // 
            this._mpsScheduledStartTime.LabelText = "Scheduled Start Time";
            this._mpsScheduledStartTime.Location = new System.Drawing.Point(281, 108);
            this._mpsScheduledStartTime.Margin = new System.Windows.Forms.Padding(2);
            this._mpsScheduledStartTime.Mask = "";
            this._mpsScheduledStartTime.Name = "_mpsScheduledStartTime";
            this._mpsScheduledStartTime.ReadOnly = true;
            this._mpsScheduledStartTime.Size = new System.Drawing.Size(136, 41);
            this._mpsScheduledStartTime.TabIndex = 13;
            this._mpsScheduledStartTime.Value = null;
            // 
            // _mpsState
            // 
            this._mpsState.LabelText = "State";
            this._mpsState.Location = new System.Drawing.Point(419, 18);
            this._mpsState.Margin = new System.Windows.Forms.Padding(2);
            this._mpsState.Mask = "";
            this._mpsState.Name = "_mpsState";
            this._mpsState.ReadOnly = true;
            this._mpsState.Size = new System.Drawing.Size(136, 41);
            this._mpsState.TabIndex = 12;
            this._mpsState.Value = null;
            // 
            // _mpsPerformerStaff
            // 
            this._mpsPerformerStaff.LabelText = "Performer Staff";
            this._mpsPerformerStaff.Location = new System.Drawing.Point(419, 63);
            this._mpsPerformerStaff.Margin = new System.Windows.Forms.Padding(2);
            this._mpsPerformerStaff.Mask = "";
            this._mpsPerformerStaff.Name = "_mpsPerformerStaff";
            this._mpsPerformerStaff.ReadOnly = true;
            this._mpsPerformerStaff.Size = new System.Drawing.Size(136, 41);
            this._mpsPerformerStaff.TabIndex = 11;
            this._mpsPerformerStaff.Value = null;
            // 
            // _mpsScheduledPerformerStaff
            // 
            this._mpsScheduledPerformerStaff.LabelText = "Scheduled Performer Staff";
            this._mpsScheduledPerformerStaff.Location = new System.Drawing.Point(279, 63);
            this._mpsScheduledPerformerStaff.Margin = new System.Windows.Forms.Padding(2);
            this._mpsScheduledPerformerStaff.Mask = "";
            this._mpsScheduledPerformerStaff.Name = "_mpsScheduledPerformerStaff";
            this._mpsScheduledPerformerStaff.ReadOnly = true;
            this._mpsScheduledPerformerStaff.Size = new System.Drawing.Size(136, 41);
            this._mpsScheduledPerformerStaff.TabIndex = 10;
            this._mpsScheduledPerformerStaff.Value = null;
            // 
            // _diagnosticServiceBreakdown
            // 
            this._diagnosticServiceBreakdown.AllowDrop = true;
            this._diagnosticServiceBreakdown.ImageList = this.imageList1;
            this._diagnosticServiceBreakdown.Location = new System.Drawing.Point(5, 18);
            this._diagnosticServiceBreakdown.Margin = new System.Windows.Forms.Padding(2);
            this._diagnosticServiceBreakdown.MenuModel = null;
            this._diagnosticServiceBreakdown.Name = "_diagnosticServiceBreakdown";
            this._diagnosticServiceBreakdown.Selection = selection1;
            this._diagnosticServiceBreakdown.ShowRootLines = false;
            this._diagnosticServiceBreakdown.ShowToolbar = false;
            this._diagnosticServiceBreakdown.Size = new System.Drawing.Size(270, 183);
            this._diagnosticServiceBreakdown.TabIndex = 10;
            this._diagnosticServiceBreakdown.ToolbarModel = null;
            this._diagnosticServiceBreakdown.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._diagnosticServiceBreakdown.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._diagnosticServiceBreakdown.Tree = null;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // _modality
            // 
            this._modality.LabelText = "Modality";
            this._modality.Location = new System.Drawing.Point(279, 18);
            this._modality.Margin = new System.Windows.Forms.Padding(2);
            this._modality.Mask = "";
            this._modality.Name = "_modality";
            this._modality.ReadOnly = true;
            this._modality.Size = new System.Drawing.Size(136, 41);
            this._modality.TabIndex = 6;
            this._modality.Value = null;
            // 
            // _schedulingRequestDateTime
            // 
            this._schedulingRequestDateTime.LabelText = "Schedule For";
            this._schedulingRequestDateTime.Location = new System.Drawing.Point(145, 50);
            this._schedulingRequestDateTime.Margin = new System.Windows.Forms.Padding(2);
            this._schedulingRequestDateTime.Mask = "";
            this._schedulingRequestDateTime.Name = "_schedulingRequestDateTime";
            this._schedulingRequestDateTime.ReadOnly = true;
            this._schedulingRequestDateTime.Size = new System.Drawing.Size(136, 41);
            this._schedulingRequestDateTime.TabIndex = 35;
            this._schedulingRequestDateTime.Value = null;
            // 
            // _orderingFacility
            // 
            this._orderingFacility.LabelText = "Ordering Facility";
            this._orderingFacility.Location = new System.Drawing.Point(425, 49);
            this._orderingFacility.Margin = new System.Windows.Forms.Padding(2);
            this._orderingFacility.Mask = "";
            this._orderingFacility.Name = "_orderingFacility";
            this._orderingFacility.ReadOnly = true;
            this._orderingFacility.Size = new System.Drawing.Size(136, 41);
            this._orderingFacility.TabIndex = 34;
            this._orderingFacility.Value = null;
            // 
            // _orderingPhysician
            // 
            this._orderingPhysician.LabelText = "Ordering Physician";
            this._orderingPhysician.Location = new System.Drawing.Point(425, 4);
            this._orderingPhysician.Margin = new System.Windows.Forms.Padding(2);
            this._orderingPhysician.Mask = "";
            this._orderingPhysician.Name = "_orderingPhysician";
            this._orderingPhysician.ReadOnly = true;
            this._orderingPhysician.Size = new System.Drawing.Size(136, 41);
            this._orderingPhysician.TabIndex = 33;
            this._orderingPhysician.Value = null;
            // 
            // _priority
            // 
            this._priority.LabelText = "Priority";
            this._priority.Location = new System.Drawing.Point(145, 5);
            this._priority.Margin = new System.Windows.Forms.Padding(2);
            this._priority.Mask = "";
            this._priority.Name = "_priority";
            this._priority.ReadOnly = true;
            this._priority.Size = new System.Drawing.Size(136, 41);
            this._priority.TabIndex = 32;
            this._priority.Value = null;
            // 
            // _cancelReason
            // 
            this._cancelReason.LabelText = "Cancel Reason";
            this._cancelReason.Location = new System.Drawing.Point(285, 49);
            this._cancelReason.Margin = new System.Windows.Forms.Padding(2);
            this._cancelReason.Mask = "";
            this._cancelReason.Name = "_cancelReason";
            this._cancelReason.ReadOnly = true;
            this._cancelReason.Size = new System.Drawing.Size(136, 41);
            this._cancelReason.TabIndex = 31;
            this._cancelReason.Value = null;
            // 
            // _placerNumber
            // 
            this._placerNumber.LabelText = "Placer Number";
            this._placerNumber.Location = new System.Drawing.Point(5, 5);
            this._placerNumber.Margin = new System.Windows.Forms.Padding(2);
            this._placerNumber.Mask = "";
            this._placerNumber.Name = "_placerNumber";
            this._placerNumber.ReadOnly = true;
            this._placerNumber.Size = new System.Drawing.Size(136, 41);
            this._placerNumber.TabIndex = 28;
            this._placerNumber.Value = null;
            // 
            // _accessionNumber
            // 
            this._accessionNumber.LabelText = "Accession Number";
            this._accessionNumber.Location = new System.Drawing.Point(5, 50);
            this._accessionNumber.Margin = new System.Windows.Forms.Padding(2);
            this._accessionNumber.Mask = "";
            this._accessionNumber.Name = "_accessionNumber";
            this._accessionNumber.ReadOnly = true;
            this._accessionNumber.Size = new System.Drawing.Size(136, 41);
            this._accessionNumber.TabIndex = 29;
            this._accessionNumber.Value = null;
            // 
            // _reasonForStudy
            // 
            this._reasonForStudy.LabelText = "Reason For Study";
            this._reasonForStudy.Location = new System.Drawing.Point(285, 5);
            this._reasonForStudy.Margin = new System.Windows.Forms.Padding(2);
            this._reasonForStudy.Mask = "";
            this._reasonForStudy.Name = "_reasonForStudy";
            this._reasonForStudy.ReadOnly = true;
            this._reasonForStudy.Size = new System.Drawing.Size(136, 41);
            this._reasonForStudy.TabIndex = 30;
            this._reasonForStudy.Value = null;
            // 
            // _visitPage
            // 
            this._visitPage.Controls.Add(this._dischargeDateTime);
            this._visitPage.Controls.Add(this._admitDateTime);
            this._visitPage.Controls.Add(this._ambulatoryStatus);
            this._visitPage.Controls.Add(this._admissionType);
            this._visitPage.Controls.Add(this._patientType);
            this._visitPage.Controls.Add(this._visitStatus);
            this._visitPage.Controls.Add(this._patientClass);
            this._visitPage.Controls.Add(this._visitNumber);
            this._visitPage.Controls.Add(this._preAdmitNumber);
            this._visitPage.Controls.Add(this._vip);
            this._visitPage.Location = new System.Drawing.Point(4, 22);
            this._visitPage.Name = "_visitPage";
            this._visitPage.Padding = new System.Windows.Forms.Padding(3);
            this._visitPage.Size = new System.Drawing.Size(581, 317);
            this._visitPage.TabIndex = 1;
            this._visitPage.Text = "Visit";
            this._visitPage.UseVisualStyleBackColor = true;
            // 
            // _dischargeDateTime
            // 
            this._dischargeDateTime.AutoSize = true;
            this._dischargeDateTime.LabelText = "Discharge Date/Time";
            this._dischargeDateTime.Location = new System.Drawing.Point(297, 111);
            this._dischargeDateTime.Margin = new System.Windows.Forms.Padding(2);
            this._dischargeDateTime.Mask = "";
            this._dischargeDateTime.Name = "_dischargeDateTime";
            this._dischargeDateTime.ReadOnly = true;
            this._dischargeDateTime.Size = new System.Drawing.Size(135, 41);
            this._dischargeDateTime.TabIndex = 41;
            this._dischargeDateTime.Value = null;
            // 
            // _admitDateTime
            // 
            this._admitDateTime.AutoSize = true;
            this._admitDateTime.LabelText = "Admit Date/Time";
            this._admitDateTime.Location = new System.Drawing.Point(155, 111);
            this._admitDateTime.Margin = new System.Windows.Forms.Padding(2);
            this._admitDateTime.Mask = "";
            this._admitDateTime.Name = "_admitDateTime";
            this._admitDateTime.ReadOnly = true;
            this._admitDateTime.Size = new System.Drawing.Size(138, 40);
            this._admitDateTime.TabIndex = 40;
            this._admitDateTime.Value = null;
            // 
            // _ambulatoryStatus
            // 
            this._ambulatoryStatus.AutoSize = true;
            this._ambulatoryStatus.LabelText = "Ambulatory Status";
            this._ambulatoryStatus.Location = new System.Drawing.Point(15, 155);
            this._ambulatoryStatus.Margin = new System.Windows.Forms.Padding(2);
            this._ambulatoryStatus.Mask = "";
            this._ambulatoryStatus.Name = "_ambulatoryStatus";
            this._ambulatoryStatus.ReadOnly = true;
            this._ambulatoryStatus.Size = new System.Drawing.Size(363, 46);
            this._ambulatoryStatus.TabIndex = 39;
            this._ambulatoryStatus.Value = null;
            // 
            // _admissionType
            // 
            this._admissionType.AutoSize = true;
            this._admissionType.LabelText = "Admission Type";
            this._admissionType.Location = new System.Drawing.Point(297, 65);
            this._admissionType.Margin = new System.Windows.Forms.Padding(2);
            this._admissionType.Mask = "";
            this._admissionType.Name = "_admissionType";
            this._admissionType.ReadOnly = true;
            this._admissionType.Size = new System.Drawing.Size(135, 41);
            this._admissionType.TabIndex = 38;
            this._admissionType.Value = null;
            // 
            // _patientType
            // 
            this._patientType.AutoSize = true;
            this._patientType.LabelText = "Patient Type";
            this._patientType.Location = new System.Drawing.Point(155, 65);
            this._patientType.Margin = new System.Windows.Forms.Padding(2);
            this._patientType.Mask = "";
            this._patientType.Name = "_patientType";
            this._patientType.ReadOnly = true;
            this._patientType.Size = new System.Drawing.Size(138, 40);
            this._patientType.TabIndex = 36;
            this._patientType.Value = null;
            // 
            // _visitStatus
            // 
            this._visitStatus.AutoSize = true;
            this._visitStatus.LabelText = "Visit Status";
            this._visitStatus.Location = new System.Drawing.Point(15, 110);
            this._visitStatus.Margin = new System.Windows.Forms.Padding(2);
            this._visitStatus.Mask = "";
            this._visitStatus.Name = "_visitStatus";
            this._visitStatus.ReadOnly = true;
            this._visitStatus.Size = new System.Drawing.Size(136, 41);
            this._visitStatus.TabIndex = 35;
            this._visitStatus.Value = null;
            // 
            // _patientClass
            // 
            this._patientClass.AutoSize = true;
            this._patientClass.LabelText = "Patient Class";
            this._patientClass.Location = new System.Drawing.Point(15, 65);
            this._patientClass.Margin = new System.Windows.Forms.Padding(2);
            this._patientClass.Mask = "";
            this._patientClass.Name = "_patientClass";
            this._patientClass.ReadOnly = true;
            this._patientClass.Size = new System.Drawing.Size(136, 41);
            this._patientClass.TabIndex = 34;
            this._patientClass.Value = null;
            // 
            // _visitNumber
            // 
            this._visitNumber.AutoSize = true;
            this._visitNumber.LabelText = "Visit Number";
            this._visitNumber.Location = new System.Drawing.Point(15, 19);
            this._visitNumber.Margin = new System.Windows.Forms.Padding(2);
            this._visitNumber.Mask = "";
            this._visitNumber.Name = "_visitNumber";
            this._visitNumber.ReadOnly = true;
            this._visitNumber.Size = new System.Drawing.Size(278, 42);
            this._visitNumber.TabIndex = 29;
            this._visitNumber.Value = null;
            // 
            // _preAdmitNumber
            // 
            this._preAdmitNumber.AutoSize = true;
            this._preAdmitNumber.LabelText = "Pre-Admit Number";
            this._preAdmitNumber.Location = new System.Drawing.Point(297, 19);
            this._preAdmitNumber.Margin = new System.Windows.Forms.Padding(2);
            this._preAdmitNumber.Mask = "";
            this._preAdmitNumber.Name = "_preAdmitNumber";
            this._preAdmitNumber.ReadOnly = true;
            this._preAdmitNumber.Size = new System.Drawing.Size(135, 42);
            this._preAdmitNumber.TabIndex = 32;
            this._preAdmitNumber.Value = null;
            // 
            // _vip
            // 
            this._vip.AutoSize = true;
            this._vip.Enabled = false;
            this._vip.Location = new System.Drawing.Point(383, 155);
            this._vip.Name = "_vip";
            this._vip.Padding = new System.Windows.Forms.Padding(0, 16, 0, 0);
            this._vip.Size = new System.Drawing.Size(49, 33);
            this._vip.TabIndex = 33;
            this._vip.Text = "VIP?";
            this._vip.UseVisualStyleBackColor = true;
            // 
            // _documentPage
            // 
            this._documentPage.Location = new System.Drawing.Point(4, 22);
            this._documentPage.Name = "_documentPage";
            this._documentPage.Padding = new System.Windows.Forms.Padding(3);
            this._documentPage.Size = new System.Drawing.Size(581, 317);
            this._documentPage.TabIndex = 3;
            this._documentPage.Text = "Document";
            this._documentPage.UseVisualStyleBackColor = true;
            // 
            // _billingPage
            // 
            this._billingPage.Location = new System.Drawing.Point(4, 22);
            this._billingPage.Name = "_billingPage";
            this._billingPage.Padding = new System.Windows.Forms.Padding(3);
            this._billingPage.Size = new System.Drawing.Size(581, 317);
            this._billingPage.TabIndex = 2;
            this._billingPage.Text = "Billing";
            this._billingPage.UseVisualStyleBackColor = true;
            // 
            // BiographyOrderHistoryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "BiographyOrderHistoryComponentControl";
            this.Size = new System.Drawing.Size(589, 586);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this._orderPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this._visitPage.ResumeLayout(false);
            this._visitPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _orderList;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage _orderPage;
        private System.Windows.Forms.TabPage _visitPage;
        private ClearCanvas.Desktop.View.WinForms.TextField _orderingFacility;
        private ClearCanvas.Desktop.View.WinForms.TextField _orderingPhysician;
        private ClearCanvas.Desktop.View.WinForms.TextField _priority;
        private ClearCanvas.Desktop.View.WinForms.TextField _cancelReason;
        private ClearCanvas.Desktop.View.WinForms.TextField _placerNumber;
        private ClearCanvas.Desktop.View.WinForms.TextField _accessionNumber;
        private ClearCanvas.Desktop.View.WinForms.TextField _reasonForStudy;
        private ClearCanvas.Desktop.View.WinForms.TextField _ambulatoryStatus;
        private ClearCanvas.Desktop.View.WinForms.TextField _admissionType;
        private ClearCanvas.Desktop.View.WinForms.TextField _patientType;
        private ClearCanvas.Desktop.View.WinForms.TextField _visitStatus;
        private ClearCanvas.Desktop.View.WinForms.TextField _patientClass;
        private ClearCanvas.Desktop.View.WinForms.TextField _visitNumber;
        private ClearCanvas.Desktop.View.WinForms.TextField _preAdmitNumber;
        private System.Windows.Forms.CheckBox _vip;
        private System.Windows.Forms.TabPage _billingPage;
        private System.Windows.Forms.ImageList imageList1;
        private ClearCanvas.Desktop.View.WinForms.TextField _schedulingRequestDateTime;
        private ClearCanvas.Desktop.View.WinForms.TextField _dischargeDateTime;
        private ClearCanvas.Desktop.View.WinForms.TextField _admitDateTime;
        private System.Windows.Forms.TabPage _documentPage;
        private System.Windows.Forms.GroupBox groupBox2;
        private ClearCanvas.Desktop.View.WinForms.TextField _mpsScheduledEndTime;
        private ClearCanvas.Desktop.View.WinForms.TextField _mpsEndTime;
        private ClearCanvas.Desktop.View.WinForms.TextField _mpsStartTime;
        private ClearCanvas.Desktop.View.WinForms.TextField _mpsScheduledStartTime;
        private ClearCanvas.Desktop.View.WinForms.TextField _mpsState;
        private ClearCanvas.Desktop.View.WinForms.TextField _mpsPerformerStaff;
        private ClearCanvas.Desktop.View.WinForms.TextField _mpsScheduledPerformerStaff;
        private ClearCanvas.Desktop.View.WinForms.BindingTreeView _diagnosticServiceBreakdown;
        private ClearCanvas.Desktop.View.WinForms.TextField _modality;
    }
}
