namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class StaffDetailsEditorComponentControl
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
            this._licenseNumber = new ClearCanvas.Controls.WinForms.TextField();
            this._staffType = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._middleName = new ClearCanvas.Controls.WinForms.TextField();
            this._givenName = new ClearCanvas.Controls.WinForms.TextField();
            this._familyName = new ClearCanvas.Controls.WinForms.TextField();
            this.SuspendLayout();
            // 
            // _licenseNumber
            // 
            this._licenseNumber.LabelText = "License Number";
            this._licenseNumber.Location = new System.Drawing.Point(239, 101);
            this._licenseNumber.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._licenseNumber.Mask = "";
            this._licenseNumber.Name = "_licenseNumber";
            this._licenseNumber.Size = new System.Drawing.Size(200, 50);
            this._licenseNumber.TabIndex = 4;
            this._licenseNumber.Value = null;
            // 
            // _staffType
            // 
            this._staffType.DataSource = null;
            this._staffType.DisplayMember = "";
            this._staffType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._staffType.LabelText = "StaffType";
            this._staffType.Location = new System.Drawing.Point(17, 101);
            this._staffType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._staffType.Name = "_staffType";
            this._staffType.Size = new System.Drawing.Size(200, 50);
            this._staffType.TabIndex = 3;
            this._staffType.Value = null;
            // 
            // _middleName
            // 
            this._middleName.LabelText = "Middle Name";
            this._middleName.Location = new System.Drawing.Point(462, 25);
            this._middleName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._middleName.Mask = "";
            this._middleName.Name = "_middleName";
            this._middleName.Size = new System.Drawing.Size(200, 50);
            this._middleName.TabIndex = 2;
            this._middleName.Value = null;
            // 
            // _givenName
            // 
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(239, 25);
            this._givenName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._givenName.Mask = "";
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(200, 50);
            this._givenName.TabIndex = 1;
            this._givenName.Value = null;
            // 
            // _familyName
            // 
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(17, 25);
            this._familyName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._familyName.Mask = "";
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(200, 50);
            this._familyName.TabIndex = 0;
            this._familyName.Value = null;
            // 
            // StaffDetailsEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._middleName);
            this.Controls.Add(this._givenName);
            this.Controls.Add(this._familyName);
            this.Controls.Add(this._licenseNumber);
            this.Controls.Add(this._staffType);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "StaffDetailsEditorComponentControl";
            this.Size = new System.Drawing.Size(671, 181);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Controls.WinForms.TextField _licenseNumber;
        private ClearCanvas.Controls.WinForms.ComboBoxField _staffType;
        private ClearCanvas.Controls.WinForms.TextField _middleName;
        private ClearCanvas.Controls.WinForms.TextField _givenName;
        private ClearCanvas.Controls.WinForms.TextField _familyName;

    }
}
