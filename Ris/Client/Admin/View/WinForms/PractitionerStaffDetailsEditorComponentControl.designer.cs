namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class PractitionerStaffDetailsEditorComponentControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._prefix = new ClearCanvas.Controls.WinForms.TextField();
            this._suffix = new ClearCanvas.Controls.WinForms.TextField();
            this._degree = new ClearCanvas.Controls.WinForms.TextField();
            this._givenName = new ClearCanvas.Controls.WinForms.TextField();
            this._middleName = new ClearCanvas.Controls.WinForms.TextField();
            this._familyName = new ClearCanvas.Controls.WinForms.TextField();
            this._licenseNumber = new ClearCanvas.Controls.WinForms.TextField();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(471, 143);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this._prefix);
            this.flowLayoutPanel1.Controls.Add(this._suffix);
            this.flowLayoutPanel1.Controls.Add(this._degree);
            this.flowLayoutPanel1.Controls.Add(this._givenName);
            this.flowLayoutPanel1.Controls.Add(this._middleName);
            this.flowLayoutPanel1.Controls.Add(this._familyName);
            this.flowLayoutPanel1.Controls.Add(this._licenseNumber);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(465, 137);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // _prefix
            // 
            this._prefix.LabelText = "Prefix";
            this._prefix.Location = new System.Drawing.Point(2, 2);
            this._prefix.Margin = new System.Windows.Forms.Padding(2);
            this._prefix.Mask = "";
            this._prefix.Name = "_prefix";
            this._prefix.Size = new System.Drawing.Size(150, 41);
            this._prefix.TabIndex = 0;
            this._prefix.Value = null;
            // 
            // _suffix
            // 
            this._suffix.LabelText = "Suffix";
            this._suffix.Location = new System.Drawing.Point(156, 2);
            this._suffix.Margin = new System.Windows.Forms.Padding(2);
            this._suffix.Mask = "";
            this._suffix.Name = "_suffix";
            this._suffix.Size = new System.Drawing.Size(150, 41);
            this._suffix.TabIndex = 1;
            this._suffix.Value = null;
            // 
            // _degree
            // 
            this._degree.LabelText = "Degree";
            this._degree.Location = new System.Drawing.Point(310, 2);
            this._degree.Margin = new System.Windows.Forms.Padding(2);
            this._degree.Mask = "";
            this._degree.Name = "_degree";
            this._degree.Size = new System.Drawing.Size(150, 41);
            this._degree.TabIndex = 2;
            this._degree.Value = null;
            // 
            // _givenName
            // 
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(2, 47);
            this._givenName.Margin = new System.Windows.Forms.Padding(2);
            this._givenName.Mask = "";
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(150, 41);
            this._givenName.TabIndex = 3;
            this._givenName.Value = null;
            // 
            // _middleName
            // 
            this._middleName.LabelText = "Middle Name";
            this._middleName.Location = new System.Drawing.Point(156, 47);
            this._middleName.Margin = new System.Windows.Forms.Padding(2);
            this._middleName.Mask = "";
            this._middleName.Name = "_middleName";
            this._middleName.Size = new System.Drawing.Size(150, 41);
            this._middleName.TabIndex = 4;
            this._middleName.Value = null;
            // 
            // _familyName
            // 
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(310, 47);
            this._familyName.Margin = new System.Windows.Forms.Padding(2);
            this._familyName.Mask = "";
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(150, 41);
            this._familyName.TabIndex = 5;
            this._familyName.Value = null;
            // 
            // _licenseNumber
            // 
            this._licenseNumber.LabelText = "License Number";
            this._licenseNumber.Location = new System.Drawing.Point(2, 92);
            this._licenseNumber.Margin = new System.Windows.Forms.Padding(2);
            this._licenseNumber.Mask = "";
            this._licenseNumber.Name = "_licenseNumber";
            this._licenseNumber.Size = new System.Drawing.Size(150, 41);
            this._licenseNumber.TabIndex = 6;
            this._licenseNumber.Value = null;
            // 
            // PractitionerStaffDetailsEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PractitionerStaffDetailsEditorComponentControl";
            this.Size = new System.Drawing.Size(471, 143);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ClearCanvas.Controls.WinForms.TextField _prefix;
        private ClearCanvas.Controls.WinForms.TextField _suffix;
        private ClearCanvas.Controls.WinForms.TextField _degree;
        private ClearCanvas.Controls.WinForms.TextField _givenName;
        private ClearCanvas.Controls.WinForms.TextField _middleName;
        private ClearCanvas.Controls.WinForms.TextField _familyName;
        private ClearCanvas.Controls.WinForms.TextField _licenseNumber;
    }
}
