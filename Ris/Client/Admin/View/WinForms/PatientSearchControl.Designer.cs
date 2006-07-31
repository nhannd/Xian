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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._familyName = new ClearCanvas.Controls.WinForms.TextField();
            this._patientIdentifierType = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._dateOfBirth = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._patientIdentifier = new ClearCanvas.Controls.WinForms.TextField();
            this._givenName = new ClearCanvas.Controls.WinForms.TextField();
            this._sex = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _searchButton
            // 
            this._searchButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._searchButton.Location = new System.Drawing.Point(207, 291);
            this._searchButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(75, 23);
            this._searchButton.TabIndex = 6;
            this._searchButton.Text = "Search";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.Controls.Add(this._familyName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._patientIdentifierType, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this._dateOfBirth, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this._patientIdentifier, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._givenName, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._sex, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._searchButton, 1, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 21.53846F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 78.46154F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(285, 591);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // _familyName
            // 
            this._familyName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._familyName, 2);
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(3, 56);
            this._familyName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(279, 50);
            this._familyName.TabIndex = 2;
            this._familyName.Value = null;
            // 
            // _patientIdentifierType
            // 
            this._patientIdentifierType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._patientIdentifierType.DataSource = null;
            this._patientIdentifierType.LabelText = "Type";
            this._patientIdentifierType.Location = new System.Drawing.Point(174, 2);
            this._patientIdentifierType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._patientIdentifierType.Name = "_patientIdentifierType";
            this._patientIdentifierType.Size = new System.Drawing.Size(108, 50);
            this._patientIdentifierType.TabIndex = 1;
            this._patientIdentifierType.Value = null;
            // 
            // _dateOfBirth
            // 
            this._dateOfBirth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._dateOfBirth, 2);
            this._dateOfBirth.LabelText = "Date of Birth";
            this._dateOfBirth.Location = new System.Drawing.Point(3, 218);
            this._dateOfBirth.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._dateOfBirth.Name = "_dateOfBirth";
            this._dateOfBirth.Nullable = true;
            this._dateOfBirth.Size = new System.Drawing.Size(279, 48);
            this._dateOfBirth.TabIndex = 5;
            this._dateOfBirth.Value = null;
            // 
            // _patientIdentifier
            // 
            this._patientIdentifier.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._patientIdentifier.LabelText = "Patient ID";
            this._patientIdentifier.Location = new System.Drawing.Point(3, 2);
            this._patientIdentifier.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._patientIdentifier.Name = "_patientIdentifier";
            this._patientIdentifier.Size = new System.Drawing.Size(165, 50);
            this._patientIdentifier.TabIndex = 0;
            this._patientIdentifier.Value = null;
            // 
            // _givenName
            // 
            this._givenName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._givenName, 2);
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(3, 110);
            this._givenName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(279, 50);
            this._givenName.TabIndex = 3;
            this._givenName.Value = null;
            // 
            // _sex
            // 
            this._sex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._sex, 2);
            this._sex.DataSource = null;
            this._sex.LabelText = "Sex";
            this._sex.Location = new System.Drawing.Point(3, 164);
            this._sex.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._sex.Name = "_sex";
            this._sex.Size = new System.Drawing.Size(279, 50);
            this._sex.TabIndex = 4;
            this._sex.Value = null;
            // 
            // PatientSearchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PatientSearchControl";
            this.Size = new System.Drawing.Size(285, 591);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _searchButton;
        private ClearCanvas.Controls.WinForms.TextField _familyName;
        private ClearCanvas.Controls.WinForms.TextField _givenName;
        private ClearCanvas.Controls.WinForms.TextField _patientIdentifier;
        private ClearCanvas.Controls.WinForms.ComboBoxField _sex;
        private ClearCanvas.Controls.WinForms.ComboBoxField _patientIdentifierType;
        private ClearCanvas.Controls.WinForms.DateTimeField _dateOfBirth;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
