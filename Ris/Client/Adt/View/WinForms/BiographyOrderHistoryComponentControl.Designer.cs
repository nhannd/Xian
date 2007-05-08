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
            ClearCanvas.Desktop.Selection selection3 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection4 = new ClearCanvas.Desktop.Selection();
            this._orderList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textField2 = new ClearCanvas.Controls.WinForms.TextField();
            this.comboBoxField2 = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this.textField3 = new ClearCanvas.Controls.WinForms.TextField();
            this.textField8 = new ClearCanvas.Controls.WinForms.TextField();
            this._orderingFacility = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._schedulingRequestDateTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._orderingPhysician = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._priority = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textField7 = new ClearCanvas.Controls.WinForms.TextField();
            this.textField5 = new ClearCanvas.Controls.WinForms.TextField();
            this._diagnosticServiceBreakdown = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
            this.comboBoxField1 = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this.textField4 = new ClearCanvas.Controls.WinForms.TextField();
            this.dateTimeField3 = new ClearCanvas.Controls.WinForms.DateTimeField();
            this.dateTimeField4 = new ClearCanvas.Controls.WinForms.DateTimeField();
            this.dateTimeField2 = new ClearCanvas.Controls.WinForms.DateTimeField();
            this.dateTimeField1 = new ClearCanvas.Controls.WinForms.DateTimeField();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._visitNumber = new ClearCanvas.Controls.WinForms.TextField();
            this._ambulatoryStatus = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._admissionType = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._visitStatus = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._visitNumberAssigningAuthority = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._dischargeDateTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._preadmitNumber = new ClearCanvas.Controls.WinForms.TextField();
            this._admitDateTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._patientType = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._patientClass = new ClearCanvas.Controls.WinForms.ComboBoxField();
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
            this._orderList.Selection = selection3;
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
            this.groupBox3.Controls.Add(this.textField2);
            this.groupBox3.Controls.Add(this.comboBoxField2);
            this.groupBox3.Controls.Add(this.textField3);
            this.groupBox3.Controls.Add(this.textField8);
            this.groupBox3.Controls.Add(this._orderingFacility);
            this.groupBox3.Controls.Add(this._schedulingRequestDateTime);
            this.groupBox3.Controls.Add(this._orderingPhysician);
            this.groupBox3.Controls.Add(this._priority);
            this.groupBox3.Location = new System.Drawing.Point(3, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(288, 203);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Order Info";
            // 
            // textField2
            // 
            this.textField2.LabelText = "Placer Number";
            this.textField2.Location = new System.Drawing.Point(5, 18);
            this.textField2.Margin = new System.Windows.Forms.Padding(2);
            this.textField2.Mask = "";
            this.textField2.Name = "textField2";
            this.textField2.Size = new System.Drawing.Size(136, 41);
            this.textField2.TabIndex = 19;
            this.textField2.Value = null;
            // 
            // comboBoxField2
            // 
            this.comboBoxField2.DataSource = null;
            this.comboBoxField2.DisplayMember = "";
            this.comboBoxField2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxField2.LabelText = "Cancel Reason";
            this.comboBoxField2.Location = new System.Drawing.Point(5, 153);
            this.comboBoxField2.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxField2.Name = "comboBoxField2";
            this.comboBoxField2.Size = new System.Drawing.Size(136, 41);
            this.comboBoxField2.TabIndex = 22;
            this.comboBoxField2.Value = null;
            // 
            // textField3
            // 
            this.textField3.LabelText = "Accession Number";
            this.textField3.Location = new System.Drawing.Point(5, 63);
            this.textField3.Margin = new System.Windows.Forms.Padding(2);
            this.textField3.Mask = "";
            this.textField3.Name = "textField3";
            this.textField3.Size = new System.Drawing.Size(136, 41);
            this.textField3.TabIndex = 20;
            this.textField3.Value = null;
            // 
            // textField8
            // 
            this.textField8.LabelText = "Reason For Study";
            this.textField8.Location = new System.Drawing.Point(5, 108);
            this.textField8.Margin = new System.Windows.Forms.Padding(2);
            this.textField8.Mask = "";
            this.textField8.Name = "textField8";
            this.textField8.Size = new System.Drawing.Size(136, 41);
            this.textField8.TabIndex = 21;
            this.textField8.Value = null;
            // 
            // _orderingFacility
            // 
            this._orderingFacility.DataSource = null;
            this._orderingFacility.DisplayMember = "";
            this._orderingFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._orderingFacility.LabelText = "Ordering Facility";
            this._orderingFacility.Location = new System.Drawing.Point(145, 153);
            this._orderingFacility.Margin = new System.Windows.Forms.Padding(2);
            this._orderingFacility.Name = "_orderingFacility";
            this._orderingFacility.Size = new System.Drawing.Size(136, 41);
            this._orderingFacility.TabIndex = 13;
            this._orderingFacility.Value = null;
            // 
            // _schedulingRequestDateTime
            // 
            this._schedulingRequestDateTime.LabelText = "Schedule For";
            this._schedulingRequestDateTime.Location = new System.Drawing.Point(145, 63);
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
            // _orderingPhysician
            // 
            this._orderingPhysician.DataSource = null;
            this._orderingPhysician.DisplayMember = "";
            this._orderingPhysician.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._orderingPhysician.LabelText = "Ordering Physician";
            this._orderingPhysician.Location = new System.Drawing.Point(145, 108);
            this._orderingPhysician.Margin = new System.Windows.Forms.Padding(2);
            this._orderingPhysician.Name = "_orderingPhysician";
            this._orderingPhysician.Size = new System.Drawing.Size(136, 41);
            this._orderingPhysician.TabIndex = 12;
            this._orderingPhysician.Value = null;
            // 
            // _priority
            // 
            this._priority.DataSource = null;
            this._priority.DisplayMember = "";
            this._priority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._priority.LabelText = "Priority";
            this._priority.Location = new System.Drawing.Point(145, 18);
            this._priority.Margin = new System.Windows.Forms.Padding(2);
            this._priority.Name = "_priority";
            this._priority.Size = new System.Drawing.Size(136, 41);
            this._priority.TabIndex = 14;
            this._priority.Value = null;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textField7);
            this.groupBox2.Controls.Add(this.textField5);
            this.groupBox2.Controls.Add(this._diagnosticServiceBreakdown);
            this.groupBox2.Controls.Add(this.comboBoxField1);
            this.groupBox2.Controls.Add(this.textField4);
            this.groupBox2.Controls.Add(this.dateTimeField3);
            this.groupBox2.Controls.Add(this.dateTimeField4);
            this.groupBox2.Controls.Add(this.dateTimeField2);
            this.groupBox2.Controls.Add(this.dateTimeField1);
            this.groupBox2.Location = new System.Drawing.Point(3, 213);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(722, 212);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Modality Procedure Steps";
            // 
            // textField7
            // 
            this.textField7.LabelText = "Performer Staff";
            this.textField7.Location = new System.Drawing.Point(581, 63);
            this.textField7.Margin = new System.Windows.Forms.Padding(2);
            this.textField7.Mask = "";
            this.textField7.Name = "textField7";
            this.textField7.Size = new System.Drawing.Size(136, 41);
            this.textField7.TabIndex = 11;
            this.textField7.Value = null;
            // 
            // textField5
            // 
            this.textField5.LabelText = "Scheduled Performer Staff";
            this.textField5.Location = new System.Drawing.Point(439, 63);
            this.textField5.Margin = new System.Windows.Forms.Padding(2);
            this.textField5.Mask = "";
            this.textField5.Name = "textField5";
            this.textField5.Size = new System.Drawing.Size(136, 41);
            this.textField5.TabIndex = 10;
            this.textField5.Value = null;
            // 
            // _diagnosticServiceBreakdown
            // 
            this._diagnosticServiceBreakdown.AllowDrop = true;
            this._diagnosticServiceBreakdown.Location = new System.Drawing.Point(5, 18);
            this._diagnosticServiceBreakdown.Margin = new System.Windows.Forms.Padding(2);
            this._diagnosticServiceBreakdown.MenuModel = null;
            this._diagnosticServiceBreakdown.Name = "_diagnosticServiceBreakdown";
            this._diagnosticServiceBreakdown.Selection = selection4;
            this._diagnosticServiceBreakdown.ShowToolbar = false;
            this._diagnosticServiceBreakdown.Size = new System.Drawing.Size(430, 183);
            this._diagnosticServiceBreakdown.TabIndex = 10;
            this._diagnosticServiceBreakdown.ToolbarModel = null;
            this._diagnosticServiceBreakdown.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._diagnosticServiceBreakdown.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._diagnosticServiceBreakdown.Tree = null;
            // 
            // comboBoxField1
            // 
            this.comboBoxField1.DataSource = null;
            this.comboBoxField1.DisplayMember = "";
            this.comboBoxField1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxField1.LabelText = "State";
            this.comboBoxField1.Location = new System.Drawing.Point(581, 18);
            this.comboBoxField1.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxField1.Name = "comboBoxField1";
            this.comboBoxField1.Size = new System.Drawing.Size(136, 41);
            this.comboBoxField1.TabIndex = 8;
            this.comboBoxField1.Value = null;
            // 
            // textField4
            // 
            this.textField4.LabelText = "Modality";
            this.textField4.Location = new System.Drawing.Point(439, 18);
            this.textField4.Margin = new System.Windows.Forms.Padding(2);
            this.textField4.Mask = "";
            this.textField4.Name = "textField4";
            this.textField4.Size = new System.Drawing.Size(136, 41);
            this.textField4.TabIndex = 6;
            this.textField4.Value = null;
            // 
            // dateTimeField3
            // 
            this.dateTimeField3.LabelText = "End Time";
            this.dateTimeField3.Location = new System.Drawing.Point(581, 160);
            this.dateTimeField3.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimeField3.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dateTimeField3.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dateTimeField3.Name = "dateTimeField3";
            this.dateTimeField3.Nullable = true;
            this.dateTimeField3.ShowTime = false;
            this.dateTimeField3.Size = new System.Drawing.Size(136, 41);
            this.dateTimeField3.TabIndex = 4;
            this.dateTimeField3.Value = null;
            // 
            // dateTimeField4
            // 
            this.dateTimeField4.LabelText = "Start Time";
            this.dateTimeField4.Location = new System.Drawing.Point(581, 108);
            this.dateTimeField4.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimeField4.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dateTimeField4.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dateTimeField4.Name = "dateTimeField4";
            this.dateTimeField4.Nullable = true;
            this.dateTimeField4.ShowTime = false;
            this.dateTimeField4.Size = new System.Drawing.Size(136, 41);
            this.dateTimeField4.TabIndex = 3;
            this.dateTimeField4.Value = null;
            // 
            // dateTimeField2
            // 
            this.dateTimeField2.LabelText = "Scheduled End Time";
            this.dateTimeField2.Location = new System.Drawing.Point(439, 160);
            this.dateTimeField2.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimeField2.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dateTimeField2.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dateTimeField2.Name = "dateTimeField2";
            this.dateTimeField2.Nullable = true;
            this.dateTimeField2.ShowTime = false;
            this.dateTimeField2.Size = new System.Drawing.Size(136, 41);
            this.dateTimeField2.TabIndex = 1;
            this.dateTimeField2.Value = null;
            // 
            // dateTimeField1
            // 
            this.dateTimeField1.LabelText = "Scheduled Start Time";
            this.dateTimeField1.Location = new System.Drawing.Point(439, 108);
            this.dateTimeField1.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimeField1.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dateTimeField1.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dateTimeField1.Name = "dateTimeField1";
            this.dateTimeField1.Nullable = true;
            this.dateTimeField1.ShowTime = false;
            this.dateTimeField1.Size = new System.Drawing.Size(136, 41);
            this.dateTimeField1.TabIndex = 0;
            this.dateTimeField1.Value = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._visitNumber);
            this.groupBox1.Controls.Add(this._ambulatoryStatus);
            this.groupBox1.Controls.Add(this._admissionType);
            this.groupBox1.Controls.Add(this._visitStatus);
            this.groupBox1.Controls.Add(this._visitNumberAssigningAuthority);
            this.groupBox1.Controls.Add(this._dischargeDateTime);
            this.groupBox1.Controls.Add(this._preadmitNumber);
            this.groupBox1.Controls.Add(this._admitDateTime);
            this.groupBox1.Controls.Add(this._patientType);
            this.groupBox1.Controls.Add(this._patientClass);
            this.groupBox1.Controls.Add(this._vip);
            this.groupBox1.Location = new System.Drawing.Point(297, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(428, 204);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Visit Info";
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
            // _ambulatoryStatus
            // 
            this._ambulatoryStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ambulatoryStatus.AutoSize = true;
            this._ambulatoryStatus.DataSource = null;
            this._ambulatoryStatus.DisplayMember = "";
            this._ambulatoryStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._ambulatoryStatus.LabelText = "Ambulatory Status";
            this._ambulatoryStatus.Location = new System.Drawing.Point(5, 153);
            this._ambulatoryStatus.Margin = new System.Windows.Forms.Padding(2);
            this._ambulatoryStatus.Name = "_ambulatoryStatus";
            this._ambulatoryStatus.Size = new System.Drawing.Size(324, 41);
            this._ambulatoryStatus.TabIndex = 21;
            this._ambulatoryStatus.Value = null;
            // 
            // _admissionType
            // 
            this._admissionType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._admissionType.DataSource = null;
            this._admissionType.DisplayMember = "";
            this._admissionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._admissionType.LabelText = "Admission Type";
            this._admissionType.Location = new System.Drawing.Point(287, 63);
            this._admissionType.Margin = new System.Windows.Forms.Padding(2);
            this._admissionType.Name = "_admissionType";
            this._admissionType.Size = new System.Drawing.Size(136, 41);
            this._admissionType.TabIndex = 22;
            this._admissionType.Value = null;
            // 
            // _visitStatus
            // 
            this._visitStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._visitStatus.AutoSize = true;
            this._visitStatus.DataSource = null;
            this._visitStatus.DisplayMember = "";
            this._visitStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._visitStatus.LabelText = "Visit Status";
            this._visitStatus.Location = new System.Drawing.Point(5, 108);
            this._visitStatus.Margin = new System.Windows.Forms.Padding(2);
            this._visitStatus.Name = "_visitStatus";
            this._visitStatus.Size = new System.Drawing.Size(136, 41);
            this._visitStatus.TabIndex = 19;
            this._visitStatus.Value = null;
            // 
            // _visitNumberAssigningAuthority
            // 
            this._visitNumberAssigningAuthority.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._visitNumberAssigningAuthority.AutoSize = true;
            this._visitNumberAssigningAuthority.DataSource = null;
            this._visitNumberAssigningAuthority.DisplayMember = "";
            this._visitNumberAssigningAuthority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._visitNumberAssigningAuthority.LabelText = "Site";
            this._visitNumberAssigningAuthority.Location = new System.Drawing.Point(145, 18);
            this._visitNumberAssigningAuthority.Margin = new System.Windows.Forms.Padding(2);
            this._visitNumberAssigningAuthority.Name = "_visitNumberAssigningAuthority";
            this._visitNumberAssigningAuthority.Size = new System.Drawing.Size(136, 41);
            this._visitNumberAssigningAuthority.TabIndex = 13;
            this._visitNumberAssigningAuthority.Value = null;
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
            this._dischargeDateTime.ShowTime = false;
            this._dischargeDateTime.Size = new System.Drawing.Size(136, 41);
            this._dischargeDateTime.TabIndex = 15;
            this._dischargeDateTime.Value = null;
            // 
            // _preadmitNumber
            // 
            this._preadmitNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._preadmitNumber.AutoSize = true;
            this._preadmitNumber.LabelText = "Pre-Admit Number";
            this._preadmitNumber.Location = new System.Drawing.Point(287, 18);
            this._preadmitNumber.Margin = new System.Windows.Forms.Padding(2);
            this._preadmitNumber.Mask = "";
            this._preadmitNumber.Name = "_preadmitNumber";
            this._preadmitNumber.Size = new System.Drawing.Size(136, 40);
            this._preadmitNumber.TabIndex = 16;
            this._preadmitNumber.Value = null;
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
            this._admitDateTime.ShowTime = false;
            this._admitDateTime.Size = new System.Drawing.Size(136, 42);
            this._admitDateTime.TabIndex = 14;
            this._admitDateTime.Value = null;
            // 
            // _patientType
            // 
            this._patientType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._patientType.AutoSize = true;
            this._patientType.DataSource = null;
            this._patientType.DisplayMember = "";
            this._patientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._patientType.LabelText = "Patient Type";
            this._patientType.Location = new System.Drawing.Point(145, 63);
            this._patientType.Margin = new System.Windows.Forms.Padding(2);
            this._patientType.Name = "_patientType";
            this._patientType.Size = new System.Drawing.Size(136, 41);
            this._patientType.TabIndex = 20;
            this._patientType.Value = null;
            // 
            // _patientClass
            // 
            this._patientClass.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._patientClass.AutoSize = true;
            this._patientClass.DataSource = null;
            this._patientClass.DisplayMember = "";
            this._patientClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._patientClass.LabelText = "Patient Class";
            this._patientClass.Location = new System.Drawing.Point(5, 63);
            this._patientClass.Margin = new System.Windows.Forms.Padding(2);
            this._patientClass.Name = "_patientClass";
            this._patientClass.Size = new System.Drawing.Size(136, 41);
            this._patientClass.TabIndex = 18;
            this._patientClass.Value = null;
            // 
            // _vip
            // 
            this._vip.AutoSize = true;
            this._vip.Location = new System.Drawing.Point(334, 153);
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
        private ClearCanvas.Controls.WinForms.ComboBoxField _priority;
        private ClearCanvas.Controls.WinForms.ComboBoxField _orderingFacility;
        private ClearCanvas.Controls.WinForms.ComboBoxField _orderingPhysician;
        private System.Windows.Forms.GroupBox groupBox1;
        private ClearCanvas.Controls.WinForms.TextField _visitNumber;
        private ClearCanvas.Controls.WinForms.ComboBoxField _ambulatoryStatus;
        private ClearCanvas.Controls.WinForms.ComboBoxField _admissionType;
        private ClearCanvas.Controls.WinForms.ComboBoxField _visitStatus;
        private ClearCanvas.Controls.WinForms.ComboBoxField _visitNumberAssigningAuthority;
        private ClearCanvas.Controls.WinForms.DateTimeField _dischargeDateTime;
        private ClearCanvas.Controls.WinForms.TextField _preadmitNumber;
        private ClearCanvas.Controls.WinForms.DateTimeField _admitDateTime;
        private ClearCanvas.Controls.WinForms.ComboBoxField _patientType;
        private ClearCanvas.Controls.WinForms.ComboBoxField _patientClass;
        private System.Windows.Forms.CheckBox _vip;
        private System.Windows.Forms.GroupBox groupBox2;
        private ClearCanvas.Controls.WinForms.DateTimeField dateTimeField1;
        private ClearCanvas.Controls.WinForms.TextField textField4;
        private ClearCanvas.Controls.WinForms.DateTimeField dateTimeField3;
        private ClearCanvas.Controls.WinForms.DateTimeField dateTimeField4;
        private ClearCanvas.Controls.WinForms.DateTimeField dateTimeField2;
        private ClearCanvas.Controls.WinForms.ComboBoxField comboBoxField1;
        private ClearCanvas.Controls.WinForms.TextField textField7;
        private ClearCanvas.Controls.WinForms.TextField textField5;
        private ClearCanvas.Controls.WinForms.ComboBoxField comboBoxField2;
        private ClearCanvas.Controls.WinForms.TextField textField8;
        private ClearCanvas.Controls.WinForms.TextField textField3;
        private ClearCanvas.Controls.WinForms.TextField textField2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}
