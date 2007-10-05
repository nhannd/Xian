namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class VisitDetailsEditorComponentControl
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
            this._visitNumber = new ClearCanvas.Controls.WinForms.TextField();
            this._visitNumberAssigningAuthority = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._admitDateTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._dischargeDateTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._dischargeDisposition = new ClearCanvas.Controls.WinForms.TextField();
            this._preadmitNumber = new ClearCanvas.Controls.WinForms.TextField();
            this._vip = new System.Windows.Forms.CheckBox();
            this._patientClass = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._visitStatus = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._patientType = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._ambulatoryStatus = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._admissionType = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _visitNumber
            // 
            this._visitNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._visitNumber.AutoSize = true;
            this._visitNumber.LabelText = "Visit Number";
            this._visitNumber.Location = new System.Drawing.Point(2, 2);
            this._visitNumber.Margin = new System.Windows.Forms.Padding(2);
            this._visitNumber.Name = "_visitNumber";
            this._visitNumber.Size = new System.Drawing.Size(136, 41);
            this._visitNumber.TabIndex = 0;
            this._visitNumber.Value = null;
            // 
            // _visitNumberAssigningAuthority
            // 
            this._visitNumberAssigningAuthority.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._visitNumberAssigningAuthority.AutoSize = true;
            this._visitNumberAssigningAuthority.DataSource = null;
            this._visitNumberAssigningAuthority.LabelText = "Site";
            this._visitNumberAssigningAuthority.Location = new System.Drawing.Point(142, 2);
            this._visitNumberAssigningAuthority.Margin = new System.Windows.Forms.Padding(2);
            this._visitNumberAssigningAuthority.Name = "_visitNumberAssigningAuthority";
            this._visitNumberAssigningAuthority.Size = new System.Drawing.Size(136, 41);
            this._visitNumberAssigningAuthority.TabIndex = 1;
            this._visitNumberAssigningAuthority.Value = null;
            // 
            // _admitDateTime
            // 
            this._admitDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._admitDateTime.AutoSize = true;
            this._admitDateTime.LabelText = "Admit Date/Time";
            this._admitDateTime.Location = new System.Drawing.Point(142, 92);
            this._admitDateTime.Margin = new System.Windows.Forms.Padding(2);
            this._admitDateTime.Name = "_admitDateTime";
            this._admitDateTime.Nullable = true;
            this._admitDateTime.ShowTime = false;
            this._admitDateTime.Size = new System.Drawing.Size(136, 42);
            this._admitDateTime.TabIndex = 2;
            this._admitDateTime.Value = null;
            // 
            // _dischargeDateTime
            // 
            this._dischargeDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dischargeDateTime.LabelText = "Discharge Date/Time";
            this._dischargeDateTime.Location = new System.Drawing.Point(282, 92);
            this._dischargeDateTime.Margin = new System.Windows.Forms.Padding(2);
            this._dischargeDateTime.Name = "_dischargeDateTime";
            this._dischargeDateTime.Nullable = true;
            this._dischargeDateTime.ShowTime = false;
            this._dischargeDateTime.Size = new System.Drawing.Size(136, 42);
            this._dischargeDateTime.TabIndex = 3;
            this._dischargeDateTime.Value = null;
            // 
            // _dischargeDisposition
            // 
            this._dischargeDisposition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dischargeDisposition.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this._dischargeDisposition, 3);
            this._dischargeDisposition.LabelText = "Discharge Disposition";
            this._dischargeDisposition.Location = new System.Drawing.Point(2, 138);
            this._dischargeDisposition.Margin = new System.Windows.Forms.Padding(2);
            this._dischargeDisposition.Name = "_dischargeDisposition";
            this._dischargeDisposition.Size = new System.Drawing.Size(416, 40);
            this._dischargeDisposition.TabIndex = 4;
            this._dischargeDisposition.Value = null;
            // 
            // _preadmitNumber
            // 
            this._preadmitNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._preadmitNumber.AutoSize = true;
            this._preadmitNumber.LabelText = "Pre-Admit Number";
            this._preadmitNumber.Location = new System.Drawing.Point(282, 2);
            this._preadmitNumber.Margin = new System.Windows.Forms.Padding(2);
            this._preadmitNumber.Name = "_preadmitNumber";
            this._preadmitNumber.Size = new System.Drawing.Size(136, 41);
            this._preadmitNumber.TabIndex = 5;
            this._preadmitNumber.Value = null;
            // 
            // _vip
            // 
            this._vip.AutoSize = true;
            this._vip.Location = new System.Drawing.Point(283, 183);
            this._vip.Name = "_vip";
            this._vip.Padding = new System.Windows.Forms.Padding(0, 16, 0, 0);
            this._vip.Size = new System.Drawing.Size(49, 33);
            this._vip.TabIndex = 6;
            this._vip.Text = "VIP?";
            this._vip.UseVisualStyleBackColor = true;
            // 
            // _patientClass
            // 
            this._patientClass.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._patientClass.AutoSize = true;
            this._patientClass.DataSource = null;
            this._patientClass.LabelText = "Patient Class";
            this._patientClass.Location = new System.Drawing.Point(2, 47);
            this._patientClass.Margin = new System.Windows.Forms.Padding(2);
            this._patientClass.Name = "_patientClass";
            this._patientClass.Size = new System.Drawing.Size(136, 41);
            this._patientClass.TabIndex = 7;
            this._patientClass.Value = null;
            // 
            // _visitStatus
            // 
            this._visitStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._visitStatus.AutoSize = true;
            this._visitStatus.DataSource = null;
            this._visitStatus.LabelText = "Visit Status";
            this._visitStatus.Location = new System.Drawing.Point(2, 92);
            this._visitStatus.Margin = new System.Windows.Forms.Padding(2);
            this._visitStatus.Name = "_visitStatus";
            this._visitStatus.Size = new System.Drawing.Size(136, 42);
            this._visitStatus.TabIndex = 8;
            this._visitStatus.Value = null;
            // 
            // _patientType
            // 
            this._patientType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._patientType.AutoSize = true;
            this._patientType.DataSource = null;
            this._patientType.LabelText = "Patient Type";
            this._patientType.Location = new System.Drawing.Point(142, 47);
            this._patientType.Margin = new System.Windows.Forms.Padding(2);
            this._patientType.Name = "_patientType";
            this._patientType.Size = new System.Drawing.Size(136, 41);
            this._patientType.TabIndex = 9;
            this._patientType.Value = null;
            // 
            // _ambulatoryStatus
            // 
            this._ambulatoryStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ambulatoryStatus.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this._ambulatoryStatus, 2);
            this._ambulatoryStatus.DataSource = null;
            this._ambulatoryStatus.LabelText = "Ambulatory Status";
            this._ambulatoryStatus.Location = new System.Drawing.Point(2, 182);
            this._ambulatoryStatus.Margin = new System.Windows.Forms.Padding(2);
            this._ambulatoryStatus.Name = "_ambulatoryStatus";
            this._ambulatoryStatus.Size = new System.Drawing.Size(276, 41);
            this._ambulatoryStatus.TabIndex = 10;
            this._ambulatoryStatus.Value = null;
            // 
            // _admissionType
            // 
            this._admissionType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._admissionType.DataSource = null;
            this._admissionType.LabelText = "Admission Type";
            this._admissionType.Location = new System.Drawing.Point(282, 47);
            this._admissionType.Margin = new System.Windows.Forms.Padding(2);
            this._admissionType.Name = "_admissionType";
            this._admissionType.Size = new System.Drawing.Size(136, 41);
            this._admissionType.TabIndex = 11;
            this._admissionType.Value = null;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this._visitNumber, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._ambulatoryStatus, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this._admissionType, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this._visitStatus, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._dischargeDisposition, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._visitNumberAssigningAuthority, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this._dischargeDateTime, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this._preadmitNumber, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this._admitDateTime, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this._patientType, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this._patientClass, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._vip, 2, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(420, 265);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // VisitDetailsEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "VisitDetailsEditorComponentControl";
            this.Size = new System.Drawing.Size(430, 274);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Controls.WinForms.TextField _visitNumber;
        private ClearCanvas.Controls.WinForms.ComboBoxField _visitNumberAssigningAuthority;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _admitDateTime;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _dischargeDateTime;
        private ClearCanvas.Controls.WinForms.TextField _dischargeDisposition;
        private ClearCanvas.Controls.WinForms.TextField _preadmitNumber;
        private System.Windows.Forms.CheckBox _vip;
        private ClearCanvas.Controls.WinForms.ComboBoxField _patientClass;
        private ClearCanvas.Controls.WinForms.ComboBoxField _visitStatus;
        private ClearCanvas.Controls.WinForms.ComboBoxField _patientType;
        private ClearCanvas.Controls.WinForms.ComboBoxField _ambulatoryStatus;
        private ClearCanvas.Controls.WinForms.ComboBoxField _admissionType;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
