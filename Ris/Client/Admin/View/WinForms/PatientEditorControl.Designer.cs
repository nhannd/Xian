namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class PatientEditorControl
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
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._middleName = new ClearCanvas.Controls.WinForms.TextField();
            this._givenName = new ClearCanvas.Controls.WinForms.TextField();
            this._familyName = new ClearCanvas.Controls.WinForms.TextField();
            this._sex = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._patientIdentifierList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._identifierAddButton = new System.Windows.Forms.Button();
            this._identifierDeleteButton = new System.Windows.Forms.Button();
            this._identifierUpdateButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(436, 433);
            this._okButton.Margin = new System.Windows.Forms.Padding(2);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(56, 19);
            this._okButton.TabIndex = 12;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(505, 433);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(56, 19);
            this._cancelButton.TabIndex = 13;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _middleName
            // 
            this._middleName.LabelText = "Middle Name";
            this._middleName.Location = new System.Drawing.Point(366, 24);
            this._middleName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._middleName.Name = "_middleName";
            this._middleName.Size = new System.Drawing.Size(139, 38);
            this._middleName.TabIndex = 25;
            this._middleName.Value = "";
            // 
            // _givenName
            // 
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(196, 24);
            this._givenName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(139, 38);
            this._givenName.TabIndex = 24;
            this._givenName.Value = "";
            // 
            // _familyName
            // 
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(23, 24);
            this._familyName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(139, 38);
            this._familyName.TabIndex = 23;
            this._familyName.Value = "";
            // 
            // _sex
            // 
            this._sex.DataSource = null;
            this._sex.LabelText = "Sex";
            this._sex.Location = new System.Drawing.Point(23, 82);
            this._sex.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._sex.Name = "_sex";
            this._sex.Size = new System.Drawing.Size(139, 41);
            this._sex.TabIndex = 26;
            this._sex.Value = null;
            // 
            // _patientIdentifierList
            // 
            this._patientIdentifierList.DataSource = null;
            this._patientIdentifierList.Location = new System.Drawing.Point(23, 154);
            this._patientIdentifierList.Name = "_patientIdentifierList";
            this._patientIdentifierList.Size = new System.Drawing.Size(510, 170);
            this._patientIdentifierList.TabIndex = 27;
            this._patientIdentifierList.ItemDoubleClicked += new System.EventHandler(this._patientIdentifierList_ItemDoubleClicked);
            this._patientIdentifierList.SelectionChanged += new System.EventHandler(this._identifierTableView_SelectionChanged);
            // 
            // _identifierAddButton
            // 
            this._identifierAddButton.Location = new System.Drawing.Point(275, 346);
            this._identifierAddButton.Name = "_identifierAddButton";
            this._identifierAddButton.Size = new System.Drawing.Size(75, 23);
            this._identifierAddButton.TabIndex = 32;
            this._identifierAddButton.Text = "Add";
            this._identifierAddButton.UseVisualStyleBackColor = true;
            this._identifierAddButton.Click += new System.EventHandler(this._identiferAddButton_Click);
            // 
            // _identifierDeleteButton
            // 
            this._identifierDeleteButton.Location = new System.Drawing.Point(458, 346);
            this._identifierDeleteButton.Name = "_identifierDeleteButton";
            this._identifierDeleteButton.Size = new System.Drawing.Size(75, 23);
            this._identifierDeleteButton.TabIndex = 33;
            this._identifierDeleteButton.Text = "Delete";
            this._identifierDeleteButton.UseVisualStyleBackColor = true;
            this._identifierDeleteButton.Click += new System.EventHandler(this._identifierDeleteButton_Click);
            // 
            // _identifierUpdateButton
            // 
            this._identifierUpdateButton.Location = new System.Drawing.Point(366, 346);
            this._identifierUpdateButton.Name = "_identifierUpdateButton";
            this._identifierUpdateButton.Size = new System.Drawing.Size(75, 23);
            this._identifierUpdateButton.TabIndex = 34;
            this._identifierUpdateButton.Text = "Update";
            this._identifierUpdateButton.UseVisualStyleBackColor = true;
            this._identifierUpdateButton.Click += new System.EventHandler(this._identifierUpdateButton_Click);
            // 
            // PatientEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._identifierUpdateButton);
            this.Controls.Add(this._identifierDeleteButton);
            this.Controls.Add(this._identifierAddButton);
            this.Controls.Add(this._patientIdentifierList);
            this.Controls.Add(this._sex);
            this.Controls.Add(this._middleName);
            this.Controls.Add(this._givenName);
            this.Controls.Add(this._familyName);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PatientEditorControl";
            this.Size = new System.Drawing.Size(582, 468);
            this.Load += new System.EventHandler(this.PatientEditorControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private ClearCanvas.Controls.WinForms.TextField _familyName;
        private ClearCanvas.Controls.WinForms.TextField _givenName;
        private ClearCanvas.Controls.WinForms.TextField _middleName;
        private ClearCanvas.Controls.WinForms.ComboBoxField _sex;
        private ClearCanvas.Desktop.View.WinForms.TableView _patientIdentifierList;
        private System.Windows.Forms.Button _identifierAddButton;
        private System.Windows.Forms.Button _identifierDeleteButton;
        private System.Windows.Forms.Button _identifierUpdateButton;
    }
}
