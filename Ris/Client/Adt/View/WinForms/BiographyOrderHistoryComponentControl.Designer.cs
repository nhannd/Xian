namespace ClearCanvas.Ris.Client.Adt.View.WinForms
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
            ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection5 = new ClearCanvas.Desktop.Selection();
            this._orderList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this._orderingFacility = new ClearCanvas.Controls.WinForms.TextField();
            this._orderingPhysician = new ClearCanvas.Controls.WinForms.TextField();
            this._priority = new ClearCanvas.Controls.WinForms.TextField();
            this._cancelReason = new ClearCanvas.Controls.WinForms.TextField();
            this._placerNumber = new ClearCanvas.Controls.WinForms.TextField();
            this._accessionNumber = new ClearCanvas.Controls.WinForms.TextField();
            this._reasonForStudy = new ClearCanvas.Controls.WinForms.TextField();
            this._schedulingRequestDateTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._mpsState = new ClearCanvas.Controls.WinForms.TextField();
            this._mpsPerformerStaff = new ClearCanvas.Controls.WinForms.TextField();
            this._mpsScheduledPerformerStaff = new ClearCanvas.Controls.WinForms.TextField();
            this._diagnosticServiceBreakdown = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
            this._modality = new ClearCanvas.Controls.WinForms.TextField();
            this._mpsEndTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._mpsStartTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._mpsScheduledEndTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._mpsScheduledStartTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._ambulatoryStatus = new ClearCanvas.Controls.WinForms.TextField();
            this._admissionType = new ClearCanvas.Controls.WinForms.TextField();
            this._site = new ClearCanvas.Controls.WinForms.TextField();
            this._patientType = new ClearCanvas.Controls.WinForms.TextField();
            this._visitStatus = new ClearCanvas.Controls.WinForms.TextField();
            this._patientClass = new ClearCanvas.Controls.WinForms.TextField();
            this._visitNumber = new ClearCanvas.Controls.WinForms.TextField();
            this._dischargeDateTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._preAdmitNumber = new ClearCanvas.Controls.WinForms.TextField();
            this._admitDateTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._vip = new System.Windows.Forms.CheckBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _orderList
            // 
            this._orderList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._orderList.Location = new System.Drawing.Point(0, 0);
            this._orderList.MenuModel = null;
            this._orderList.Name = "_orderList";
            this._orderList.ReadOnly = false;
            this._orderList.Selection = selection2;
            this._orderList.Size = new System.Drawing.Size(731, 239);
            this._orderList.TabIndex = 0;
            this._orderList.Table = null;
            this._orderList.ToolbarModel = null;
            this._orderList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._orderList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
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
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(731, 676);
            this.splitContainer1.SplitterDistance = 239;
            this.splitContainer1.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this._orderingFacility);
            this.groupBox3.Controls.Add(this._orderingPhysician);
            this.groupBox3.Controls.Add(this._priority);
            this.groupBox3.Controls.Add(this._cancelReason);
            this.groupBox3.Controls.Add(this._placerNumber);
            this.groupBox3.Controls.Add(this._accessionNumber);
            this.groupBox3.Controls.Add(this._reasonForStudy);
            this.groupBox3.Controls.Add(this._schedulingRequestDateTime);
            this.groupBox3.Location = new System.Drawing.Point(3, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(288, 203);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Order Info";
            // 
            // _orderingFacility
            // 
            this._orderingFacility.LabelText = "Ordering Facility";
            this._orderingFacility.Location = new System.Drawing.Point(145, 152);
            this._orderingFacility.Margin = new System.Windows.Forms.Padding(2);
            this._orderingFacility.Mask = "";
            this._orderingFacility.Name = "_orderingFacility";
            this._orderingFacility.Size = new System.Drawing.Size(136, 41);
            this._orderingFacility.TabIndex = 26;
            this._orderingFacility.Value = null;
            // 
            // _orderingPhysician
            // 
            this._orderingPhysician.LabelText = "Ordering Physician";
            this._orderingPhysician.Location = new System.Drawing.Point(145, 107);
            this._orderingPhysician.Margin = new System.Windows.Forms.Padding(2);
            this._orderingPhysician.Mask = "";
            this._orderingPhysician.Name = "_orderingPhysician";
            this._orderingPhysician.Size = new System.Drawing.Size(136, 41);
            this._orderingPhysician.TabIndex = 25;
            this._orderingPhysician.Value = null;
            // 
            // _priority
            // 
            this._priority.LabelText = "Priority";
            this._priority.Location = new System.Drawing.Point(145, 18);
            this._priority.Margin = new System.Windows.Forms.Padding(2);
            this._priority.Mask = "";
            this._priority.Name = "_priority";
            this._priority.Size = new System.Drawing.Size(136, 41);
            this._priority.TabIndex = 24;
            this._priority.Value = null;
            // 
            // _cancelReason
            // 
            this._cancelReason.LabelText = "Cancel Reason";
            this._cancelReason.Location = new System.Drawing.Point(5, 152);
            this._cancelReason.Margin = new System.Windows.Forms.Padding(2);
            this._cancelReason.Mask = "";
            this._cancelReason.Name = "_cancelReason";
            this._cancelReason.Size = new System.Drawing.Size(136, 41);
            this._cancelReason.TabIndex = 23;
            this._cancelReason.Value = null;
            // 
            // _placerNumber
            // 
            this._placerNumber.LabelText = "Placer Number";
            this._placerNumber.Location = new System.Drawing.Point(5, 18);
            this._placerNumber.Margin = new System.Windows.Forms.Padding(2);
            this._placerNumber.Mask = "";
            this._placerNumber.Name = "_placerNumber";
            this._placerNumber.Size = new System.Drawing.Size(136, 41);
            this._placerNumber.TabIndex = 19;
            this._placerNumber.Value = null;
            // 
            // _accessionNumber
            // 
            this._accessionNumber.LabelText = "Accession Number";
            this._accessionNumber.Location = new System.Drawing.Point(5, 63);
            this._accessionNumber.Margin = new System.Windows.Forms.Padding(2);
            this._accessionNumber.Mask = "";
            this._accessionNumber.Name = "_accessionNumber";
            this._accessionNumber.Size = new System.Drawing.Size(136, 41);
            this._accessionNumber.TabIndex = 20;
            this._accessionNumber.Value = null;
            // 
            // _reasonForStudy
            // 
            this._reasonForStudy.LabelText = "Reason For Study";
            this._reasonForStudy.Location = new System.Drawing.Point(5, 108);
            this._reasonForStudy.Margin = new System.Windows.Forms.Padding(2);
            this._reasonForStudy.Mask = "";
            this._reasonForStudy.Name = "_reasonForStudy";
            this._reasonForStudy.Size = new System.Drawing.Size(136, 41);
            this._reasonForStudy.TabIndex = 21;
            this._reasonForStudy.Value = null;
            // 
            // _schedulingRequestDateTime
            // 
            this._schedulingRequestDateTime.LabelText = "Schedule For";
            this._schedulingRequestDateTime.Location = new System.Drawing.Point(144, 63);
            this._schedulingRequestDateTime.Margin = new System.Windows.Forms.Padding(2);
            this._schedulingRequestDateTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._schedulingRequestDateTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._schedulingRequestDateTime.Name = "_schedulingRequestDateTime";
            this._schedulingRequestDateTime.Nullable = false;
            this._schedulingRequestDateTime.ShowTime = true;
            this._schedulingRequestDateTime.Size = new System.Drawing.Size(136, 41);
            this._schedulingRequestDateTime.TabIndex = 15;
            this._schedulingRequestDateTime.Value = null;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._mpsState);
            this.groupBox2.Controls.Add(this._mpsPerformerStaff);
            this.groupBox2.Controls.Add(this._mpsScheduledPerformerStaff);
            this.groupBox2.Controls.Add(this._diagnosticServiceBreakdown);
            this.groupBox2.Controls.Add(this._modality);
            this.groupBox2.Controls.Add(this._mpsEndTime);
            this.groupBox2.Controls.Add(this._mpsStartTime);
            this.groupBox2.Controls.Add(this._mpsScheduledEndTime);
            this.groupBox2.Controls.Add(this._mpsScheduledStartTime);
            this.groupBox2.Location = new System.Drawing.Point(3, 213);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(722, 212);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Requested Procedures and Procedure Steps";
            // 
            // _mpsState
            // 
            this._mpsState.LabelText = "State";
            this._mpsState.Location = new System.Drawing.Point(581, 18);
            this._mpsState.Margin = new System.Windows.Forms.Padding(2);
            this._mpsState.Mask = "";
            this._mpsState.Name = "_mpsState";
            this._mpsState.Size = new System.Drawing.Size(136, 41);
            this._mpsState.TabIndex = 12;
            this._mpsState.Value = null;
            // 
            // _mpsPerformerStaff
            // 
            this._mpsPerformerStaff.LabelText = "Performer Staff";
            this._mpsPerformerStaff.Location = new System.Drawing.Point(581, 63);
            this._mpsPerformerStaff.Margin = new System.Windows.Forms.Padding(2);
            this._mpsPerformerStaff.Mask = "";
            this._mpsPerformerStaff.Name = "_mpsPerformerStaff";
            this._mpsPerformerStaff.Size = new System.Drawing.Size(136, 41);
            this._mpsPerformerStaff.TabIndex = 11;
            this._mpsPerformerStaff.Value = null;
            // 
            // _mpsScheduledPerformerStaff
            // 
            this._mpsScheduledPerformerStaff.LabelText = "Scheduled Performer Staff";
            this._mpsScheduledPerformerStaff.Location = new System.Drawing.Point(439, 63);
            this._mpsScheduledPerformerStaff.Margin = new System.Windows.Forms.Padding(2);
            this._mpsScheduledPerformerStaff.Mask = "";
            this._mpsScheduledPerformerStaff.Name = "_mpsScheduledPerformerStaff";
            this._mpsScheduledPerformerStaff.Size = new System.Drawing.Size(136, 41);
            this._mpsScheduledPerformerStaff.TabIndex = 10;
            this._mpsScheduledPerformerStaff.Value = null;
            // 
            // _diagnosticServiceBreakdown
            // 
            this._diagnosticServiceBreakdown.AllowDrop = true;
            this._diagnosticServiceBreakdown.Location = new System.Drawing.Point(5, 18);
            this._diagnosticServiceBreakdown.Margin = new System.Windows.Forms.Padding(2);
            this._diagnosticServiceBreakdown.MenuModel = null;
            this._diagnosticServiceBreakdown.Name = "_diagnosticServiceBreakdown";
            this._diagnosticServiceBreakdown.Selection = selection5;
            this._diagnosticServiceBreakdown.ShowRootLines = false;
            this._diagnosticServiceBreakdown.ShowToolbar = false;
            this._diagnosticServiceBreakdown.Size = new System.Drawing.Size(430, 183);
            this._diagnosticServiceBreakdown.TabIndex = 10;
            this._diagnosticServiceBreakdown.ToolbarModel = null;
            this._diagnosticServiceBreakdown.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._diagnosticServiceBreakdown.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._diagnosticServiceBreakdown.Tree = null;
            // 
            // _modality
            // 
            this._modality.LabelText = "Modality";
            this._modality.Location = new System.Drawing.Point(439, 18);
            this._modality.Margin = new System.Windows.Forms.Padding(2);
            this._modality.Mask = "";
            this._modality.Name = "_modality";
            this._modality.Size = new System.Drawing.Size(136, 41);
            this._modality.TabIndex = 6;
            this._modality.Value = null;
            // 
            // _mpsEndTime
            // 
            this._mpsEndTime.LabelText = "End Time";
            this._mpsEndTime.Location = new System.Drawing.Point(581, 160);
            this._mpsEndTime.Margin = new System.Windows.Forms.Padding(2);
            this._mpsEndTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._mpsEndTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._mpsEndTime.Name = "_mpsEndTime";
            this._mpsEndTime.Nullable = true;
            this._mpsEndTime.ShowTime = true;
            this._mpsEndTime.Size = new System.Drawing.Size(136, 41);
            this._mpsEndTime.TabIndex = 4;
            this._mpsEndTime.Value = null;
            // 
            // _mpsStartTime
            // 
            this._mpsStartTime.LabelText = "Start Time";
            this._mpsStartTime.Location = new System.Drawing.Point(581, 108);
            this._mpsStartTime.Margin = new System.Windows.Forms.Padding(2);
            this._mpsStartTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._mpsStartTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._mpsStartTime.Name = "_mpsStartTime";
            this._mpsStartTime.Nullable = true;
            this._mpsStartTime.ShowTime = true;
            this._mpsStartTime.Size = new System.Drawing.Size(136, 41);
            this._mpsStartTime.TabIndex = 3;
            this._mpsStartTime.Value = null;
            // 
            // _mpsScheduledEndTime
            // 
            this._mpsScheduledEndTime.LabelText = "Scheduled End Time";
            this._mpsScheduledEndTime.Location = new System.Drawing.Point(439, 160);
            this._mpsScheduledEndTime.Margin = new System.Windows.Forms.Padding(2);
            this._mpsScheduledEndTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._mpsScheduledEndTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._mpsScheduledEndTime.Name = "_mpsScheduledEndTime";
            this._mpsScheduledEndTime.Nullable = true;
            this._mpsScheduledEndTime.ShowTime = true;
            this._mpsScheduledEndTime.Size = new System.Drawing.Size(136, 41);
            this._mpsScheduledEndTime.TabIndex = 1;
            this._mpsScheduledEndTime.Value = null;
            // 
            // _mpsScheduledStartTime
            // 
            this._mpsScheduledStartTime.LabelText = "Scheduled Start Time";
            this._mpsScheduledStartTime.Location = new System.Drawing.Point(439, 108);
            this._mpsScheduledStartTime.Margin = new System.Windows.Forms.Padding(2);
            this._mpsScheduledStartTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._mpsScheduledStartTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._mpsScheduledStartTime.Name = "_mpsScheduledStartTime";
            this._mpsScheduledStartTime.Nullable = true;
            this._mpsScheduledStartTime.ShowTime = true;
            this._mpsScheduledStartTime.Size = new System.Drawing.Size(136, 41);
            this._mpsScheduledStartTime.TabIndex = 0;
            this._mpsScheduledStartTime.Value = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._ambulatoryStatus);
            this.groupBox1.Controls.Add(this._admissionType);
            this.groupBox1.Controls.Add(this._site);
            this.groupBox1.Controls.Add(this._patientType);
            this.groupBox1.Controls.Add(this._visitStatus);
            this.groupBox1.Controls.Add(this._patientClass);
            this.groupBox1.Controls.Add(this._visitNumber);
            this.groupBox1.Controls.Add(this._dischargeDateTime);
            this.groupBox1.Controls.Add(this._preAdmitNumber);
            this.groupBox1.Controls.Add(this._admitDateTime);
            this.groupBox1.Controls.Add(this._vip);
            this.groupBox1.Location = new System.Drawing.Point(297, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(428, 204);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Visit Info";
            // 
            // _ambulatoryStatus
            // 
            this._ambulatoryStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ambulatoryStatus.AutoSize = true;
            this._ambulatoryStatus.LabelText = "Ambulatory Status";
            this._ambulatoryStatus.Location = new System.Drawing.Point(5, 154);
            this._ambulatoryStatus.Margin = new System.Windows.Forms.Padding(2);
            this._ambulatoryStatus.Mask = "";
            this._ambulatoryStatus.Name = "_ambulatoryStatus";
            this._ambulatoryStatus.Size = new System.Drawing.Size(363, 40);
            this._ambulatoryStatus.TabIndex = 28;
            this._ambulatoryStatus.Value = null;
            // 
            // _admissionType
            // 
            this._admissionType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._admissionType.AutoSize = true;
            this._admissionType.LabelText = "Admission Type";
            this._admissionType.Location = new System.Drawing.Point(287, 64);
            this._admissionType.Margin = new System.Windows.Forms.Padding(2);
            this._admissionType.Mask = "";
            this._admissionType.Name = "_admissionType";
            this._admissionType.Size = new System.Drawing.Size(136, 40);
            this._admissionType.TabIndex = 27;
            this._admissionType.Value = null;
            // 
            // _site
            // 
            this._site.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._site.AutoSize = true;
            this._site.LabelText = "Site";
            this._site.Location = new System.Drawing.Point(145, 18);
            this._site.Margin = new System.Windows.Forms.Padding(2);
            this._site.Mask = "";
            this._site.Name = "_site";
            this._site.Size = new System.Drawing.Size(136, 40);
            this._site.TabIndex = 26;
            this._site.Value = null;
            // 
            // _patientType
            // 
            this._patientType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._patientType.AutoSize = true;
            this._patientType.LabelText = "Patient Type";
            this._patientType.Location = new System.Drawing.Point(145, 64);
            this._patientType.Margin = new System.Windows.Forms.Padding(2);
            this._patientType.Mask = "";
            this._patientType.Name = "_patientType";
            this._patientType.Size = new System.Drawing.Size(136, 40);
            this._patientType.TabIndex = 25;
            this._patientType.Value = null;
            // 
            // _visitStatus
            // 
            this._visitStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._visitStatus.AutoSize = true;
            this._visitStatus.LabelText = "Visit Status";
            this._visitStatus.Location = new System.Drawing.Point(5, 109);
            this._visitStatus.Margin = new System.Windows.Forms.Padding(2);
            this._visitStatus.Mask = "";
            this._visitStatus.Name = "_visitStatus";
            this._visitStatus.Size = new System.Drawing.Size(136, 40);
            this._visitStatus.TabIndex = 24;
            this._visitStatus.Value = null;
            // 
            // _patientClass
            // 
            this._patientClass.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._patientClass.AutoSize = true;
            this._patientClass.LabelText = "Patient Class";
            this._patientClass.Location = new System.Drawing.Point(5, 64);
            this._patientClass.Margin = new System.Windows.Forms.Padding(2);
            this._patientClass.Mask = "";
            this._patientClass.Name = "_patientClass";
            this._patientClass.Size = new System.Drawing.Size(136, 40);
            this._patientClass.TabIndex = 23;
            this._patientClass.Value = null;
            // 
            // _visitNumber
            // 
            this._visitNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._visitNumber.AutoSize = true;
            this._visitNumber.LabelText = "Visit Number";
            this._visitNumber.Location = new System.Drawing.Point(5, 18);
            this._visitNumber.Margin = new System.Windows.Forms.Padding(2);
            this._visitNumber.Mask = "";
            this._visitNumber.Name = "_visitNumber";
            this._visitNumber.Size = new System.Drawing.Size(136, 40);
            this._visitNumber.TabIndex = 12;
            this._visitNumber.Value = null;
            // 
            // _dischargeDateTime
            // 
            this._dischargeDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dischargeDateTime.LabelText = "Discharge Date/Time";
            this._dischargeDateTime.Location = new System.Drawing.Point(287, 108);
            this._dischargeDateTime.Margin = new System.Windows.Forms.Padding(2);
            this._dischargeDateTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._dischargeDateTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._dischargeDateTime.Name = "_dischargeDateTime";
            this._dischargeDateTime.Nullable = true;
            this._dischargeDateTime.ShowTime = true;
            this._dischargeDateTime.Size = new System.Drawing.Size(136, 41);
            this._dischargeDateTime.TabIndex = 15;
            this._dischargeDateTime.Value = null;
            // 
            // _preAdmitNumber
            // 
            this._preAdmitNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._preAdmitNumber.AutoSize = true;
            this._preAdmitNumber.LabelText = "Pre-Admit Number";
            this._preAdmitNumber.Location = new System.Drawing.Point(287, 18);
            this._preAdmitNumber.Margin = new System.Windows.Forms.Padding(2);
            this._preAdmitNumber.Mask = "";
            this._preAdmitNumber.Name = "_preAdmitNumber";
            this._preAdmitNumber.Size = new System.Drawing.Size(136, 40);
            this._preAdmitNumber.TabIndex = 16;
            this._preAdmitNumber.Value = null;
            // 
            // _admitDateTime
            // 
            this._admitDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._admitDateTime.AutoSize = true;
            this._admitDateTime.LabelText = "Admit Date/Time";
            this._admitDateTime.Location = new System.Drawing.Point(145, 108);
            this._admitDateTime.Margin = new System.Windows.Forms.Padding(2);
            this._admitDateTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._admitDateTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._admitDateTime.Name = "_admitDateTime";
            this._admitDateTime.Nullable = true;
            this._admitDateTime.ShowTime = true;
            this._admitDateTime.Size = new System.Drawing.Size(136, 42);
            this._admitDateTime.TabIndex = 14;
            this._admitDateTime.Value = null;
            // 
            // _vip
            // 
            this._vip.AutoSize = true;
            this._vip.Location = new System.Drawing.Point(373, 154);
            this._vip.Name = "_vip";
            this._vip.Padding = new System.Windows.Forms.Padding(0, 16, 0, 0);
            this._vip.Size = new System.Drawing.Size(49, 33);
            this._vip.TabIndex = 17;
            this._vip.Text = "VIP?";
            this._vip.UseVisualStyleBackColor = true;
            // 
            // BiographyOrderHistoryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "BiographyOrderHistoryComponentControl";
            this.Size = new System.Drawing.Size(731, 676);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _orderList;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ClearCanvas.Desktop.View.WinForms.BindingTreeView _diagnosticServiceBreakdown;
        private ClearCanvas.Controls.WinForms.DateTimeField _schedulingRequestDateTime;
        private System.Windows.Forms.GroupBox groupBox1;
        private ClearCanvas.Controls.WinForms.TextField _visitNumber;
        private ClearCanvas.Controls.WinForms.DateTimeField _dischargeDateTime;
        private ClearCanvas.Controls.WinForms.TextField _preAdmitNumber;
        private ClearCanvas.Controls.WinForms.DateTimeField _admitDateTime;
        private System.Windows.Forms.CheckBox _vip;
        private System.Windows.Forms.GroupBox groupBox2;
        private ClearCanvas.Controls.WinForms.DateTimeField _mpsScheduledStartTime;
        private ClearCanvas.Controls.WinForms.TextField _modality;
        private ClearCanvas.Controls.WinForms.DateTimeField _mpsEndTime;
        private ClearCanvas.Controls.WinForms.DateTimeField _mpsStartTime;
        private ClearCanvas.Controls.WinForms.DateTimeField _mpsScheduledEndTime;
        private ClearCanvas.Controls.WinForms.TextField _mpsPerformerStaff;
        private ClearCanvas.Controls.WinForms.TextField _mpsScheduledPerformerStaff;
        private ClearCanvas.Controls.WinForms.TextField _reasonForStudy;
        private ClearCanvas.Controls.WinForms.TextField _accessionNumber;
        private ClearCanvas.Controls.WinForms.TextField _placerNumber;
        private System.Windows.Forms.GroupBox groupBox3;
        private ClearCanvas.Controls.WinForms.TextField _orderingFacility;
        private ClearCanvas.Controls.WinForms.TextField _orderingPhysician;
        private ClearCanvas.Controls.WinForms.TextField _priority;
        private ClearCanvas.Controls.WinForms.TextField _cancelReason;
        private ClearCanvas.Controls.WinForms.TextField _admissionType;
        private ClearCanvas.Controls.WinForms.TextField _site;
        private ClearCanvas.Controls.WinForms.TextField _patientType;
        private ClearCanvas.Controls.WinForms.TextField _visitStatus;
        private ClearCanvas.Controls.WinForms.TextField _patientClass;
        private ClearCanvas.Controls.WinForms.TextField _ambulatoryStatus;
        private ClearCanvas.Controls.WinForms.TextField _mpsState;
    }
}
