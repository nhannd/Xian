namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class PatientSearchControl
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
            this._searchButton = new System.Windows.Forms.Button();
            this._familyName = new ClearCanvas.Controls.WinForms.TextField();
            this._givenName = new ClearCanvas.Controls.WinForms.TextField();
            this._patientIdentifier = new ClearCanvas.Controls.WinForms.TextField();
            this._sex = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._patientIdentifierType = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this.SuspendLayout();
            // 
            // _searchButton
            // 
            this._searchButton.Location = new System.Drawing.Point(168, 442);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(75, 23);
            this._searchButton.TabIndex = 3;
            this._searchButton.Text = "Search";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
            // 
            // _familyName
            // 
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(21, 156);
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(272, 50);
            this._familyName.TabIndex = 1;
            this._familyName.Value = "";
            // 
            // _givenName
            // 
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(21, 235);
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(272, 50);
            this._givenName.TabIndex = 2;
            this._givenName.Value = "";
            // 
            // _patientIdentifier
            // 
            this._patientIdentifier.LabelText = "Patient ID";
            this._patientIdentifier.Location = new System.Drawing.Point(21, 76);
            this._patientIdentifier.Name = "_patientIdentifier";
            this._patientIdentifier.Size = new System.Drawing.Size(160, 50);
            this._patientIdentifier.TabIndex = 0;
            this._patientIdentifier.Value = "";
            // 
            // _sex
            // 
            this._sex.DataSource = null;
            this._sex.LabelText = "Sex";
            this._sex.Location = new System.Drawing.Point(21, 310);
            this._sex.Name = "_sex";
            this._sex.Size = new System.Drawing.Size(272, 50);
            this._sex.TabIndex = 4;
            this._sex.Value = null;
            // 
            // _patientIdentifierType
            // 
            this._patientIdentifierType.DataSource = null;
            this._patientIdentifierType.LabelText = "Type";
            this._patientIdentifierType.Location = new System.Drawing.Point(183, 73);
            this._patientIdentifierType.Name = "_patientIdentifierType";
            this._patientIdentifierType.Size = new System.Drawing.Size(110, 50);
            this._patientIdentifierType.TabIndex = 5;
            this._patientIdentifierType.Value = null;
            // 
            // PatientSearchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._patientIdentifierType);
            this.Controls.Add(this._sex);
            this.Controls.Add(this._patientIdentifier);
            this.Controls.Add(this._givenName);
            this.Controls.Add(this._familyName);
            this.Controls.Add(this._searchButton);
            this.Name = "PatientSearchControl";
            this.Size = new System.Drawing.Size(310, 543);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _searchButton;
        private ClearCanvas.Controls.WinForms.TextField _familyName;
        private ClearCanvas.Controls.WinForms.TextField _givenName;
        private ClearCanvas.Controls.WinForms.TextField _patientIdentifier;
        private ClearCanvas.Controls.WinForms.ComboBoxField _sex;
        private ClearCanvas.Controls.WinForms.ComboBoxField _patientIdentifierType;
    }
}
