namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class PatientReconciliationComponentControl
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
            this._mrnField = new ClearCanvas.Controls.WinForms.TextField();
            this._healthcardField = new ClearCanvas.Controls.WinForms.TextField();
            this._familyNameField = new ClearCanvas.Controls.WinForms.TextField();
            this._givenNameField = new ClearCanvas.Controls.WinForms.TextField();
            this._searchButton = new System.Windows.Forms.Button();
            this._searchResultsTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._alternateProfilesTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._reconciliationCandidateTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _mrnField
            // 
            this._mrnField.LabelText = "MRN";
            this._mrnField.Location = new System.Drawing.Point(19, 15);
            this._mrnField.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._mrnField.Name = "_mrnField";
            this._mrnField.Size = new System.Drawing.Size(200, 50);
            this._mrnField.TabIndex = 0;
            this._mrnField.Value = null;
            // 
            // _healthcardField
            // 
            this._healthcardField.LabelText = "Healthcard";
            this._healthcardField.Location = new System.Drawing.Point(296, 15);
            this._healthcardField.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._healthcardField.Name = "_healthcardField";
            this._healthcardField.Size = new System.Drawing.Size(200, 50);
            this._healthcardField.TabIndex = 1;
            this._healthcardField.Value = null;
            // 
            // _familyNameField
            // 
            this._familyNameField.LabelText = "Family Name";
            this._familyNameField.Location = new System.Drawing.Point(19, 93);
            this._familyNameField.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._familyNameField.Name = "_familyNameField";
            this._familyNameField.Size = new System.Drawing.Size(200, 50);
            this._familyNameField.TabIndex = 2;
            this._familyNameField.Value = null;
            // 
            // _givenNameField
            // 
            this._givenNameField.LabelText = "Given Name";
            this._givenNameField.Location = new System.Drawing.Point(296, 93);
            this._givenNameField.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._givenNameField.Name = "_givenNameField";
            this._givenNameField.Size = new System.Drawing.Size(200, 50);
            this._givenNameField.TabIndex = 3;
            this._givenNameField.Value = null;
            // 
            // _searchButton
            // 
            this._searchButton.Location = new System.Drawing.Point(558, 42);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(75, 23);
            this._searchButton.TabIndex = 4;
            this._searchButton.Text = "Search";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
            // 
            // _searchResultsTable
            // 
            this._searchResultsTable.DataSource = null;
            this._searchResultsTable.Location = new System.Drawing.Point(19, 197);
            this._searchResultsTable.Margin = new System.Windows.Forms.Padding(4);
            this._searchResultsTable.MenuModel = null;
            this._searchResultsTable.Name = "_searchResultsTable";
            this._searchResultsTable.Size = new System.Drawing.Size(477, 370);
            this._searchResultsTable.TabIndex = 5;
            this._searchResultsTable.ToolbarModel = null;
            this._searchResultsTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._searchResultsTable.SelectionChanged += new System.EventHandler(this._searchResultsTable_SelectionChanged);
            // 
            // _alternateProfilesTable
            // 
            this._alternateProfilesTable.DataSource = null;
            this._alternateProfilesTable.Location = new System.Drawing.Point(544, 197);
            this._alternateProfilesTable.Margin = new System.Windows.Forms.Padding(4);
            this._alternateProfilesTable.MenuModel = null;
            this._alternateProfilesTable.Name = "_alternateProfilesTable";
            this._alternateProfilesTable.Size = new System.Drawing.Size(477, 169);
            this._alternateProfilesTable.TabIndex = 6;
            this._alternateProfilesTable.ToolbarModel = null;
            this._alternateProfilesTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _reconciliationCandidateTable
            // 
            this._reconciliationCandidateTable.DataSource = null;
            this._reconciliationCandidateTable.Location = new System.Drawing.Point(544, 398);
            this._reconciliationCandidateTable.Margin = new System.Windows.Forms.Padding(4);
            this._reconciliationCandidateTable.MenuModel = null;
            this._reconciliationCandidateTable.Name = "_reconciliationCandidateTable";
            this._reconciliationCandidateTable.Size = new System.Drawing.Size(477, 169);
            this._reconciliationCandidateTable.TabIndex = 7;
            this._reconciliationCandidateTable.ToolbarModel = null;
            this._reconciliationCandidateTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(541, 377);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Reconciliation Candidates";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(541, 176);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Alternate Profiles";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "Search Results";
            // 
            // PatientReconciliationComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._reconciliationCandidateTable);
            this.Controls.Add(this._alternateProfilesTable);
            this.Controls.Add(this._searchResultsTable);
            this.Controls.Add(this._searchButton);
            this.Controls.Add(this._givenNameField);
            this.Controls.Add(this._familyNameField);
            this.Controls.Add(this._healthcardField);
            this.Controls.Add(this._mrnField);
            this.Name = "PatientReconciliationComponentControl";
            this.Size = new System.Drawing.Size(1080, 609);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Controls.WinForms.TextField _mrnField;
        private ClearCanvas.Controls.WinForms.TextField _healthcardField;
        private ClearCanvas.Controls.WinForms.TextField _familyNameField;
        private ClearCanvas.Controls.WinForms.TextField _givenNameField;
        private System.Windows.Forms.Button _searchButton;
        private ClearCanvas.Desktop.View.WinForms.TableView _searchResultsTable;
        private ClearCanvas.Desktop.View.WinForms.TableView _alternateProfilesTable;
        private ClearCanvas.Desktop.View.WinForms.TableView _reconciliationCandidateTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
