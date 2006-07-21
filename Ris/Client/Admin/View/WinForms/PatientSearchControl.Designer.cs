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
            this._mrn = new ClearCanvas.Controls.WinForms.TextField();
            this.SuspendLayout();
            // 
            // _searchButton
            // 
            this._searchButton.Location = new System.Drawing.Point(168, 333);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(75, 23);
            this._searchButton.TabIndex = 7;
            this._searchButton.Text = "Search";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
            // 
            // _familyName
            // 
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(43, 156);
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(200, 50);
            this._familyName.TabIndex = 8;
            this._familyName.Value = "";
            // 
            // _givenName
            // 
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(43, 235);
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(200, 50);
            this._givenName.TabIndex = 9;
            this._givenName.Value = "";
            // 
            // _mrn
            // 
            this._mrn.LabelText = "MRN";
            this._mrn.Location = new System.Drawing.Point(43, 76);
            this._mrn.Name = "_mrn";
            this._mrn.Size = new System.Drawing.Size(200, 50);
            this._mrn.TabIndex = 10;
            this._mrn.Value = "";
            // 
            // PatientSearchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._mrn);
            this.Controls.Add(this._givenName);
            this.Controls.Add(this._familyName);
            this.Controls.Add(this._searchButton);
            this.Name = "PatientSearchControl";
            this.Size = new System.Drawing.Size(300, 435);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _searchButton;
        private ClearCanvas.Controls.WinForms.TextField _familyName;
        private ClearCanvas.Controls.WinForms.TextField _givenName;
        private ClearCanvas.Controls.WinForms.TextField _mrn;
    }
}
