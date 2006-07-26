namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class PatientIdentifierEditorControl
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
            this._identifierID = new ClearCanvas.Controls.WinForms.TextField();
            this._identifierAssigningAuthority = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._identifierType = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._acceptButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _identifierID
            // 
            this._identifierID.LabelText = "Identifier";
            this._identifierID.Location = new System.Drawing.Point(29, 75);
            this._identifierID.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._identifierID.Name = "_identifierID";
            this._identifierID.Size = new System.Drawing.Size(192, 41);
            this._identifierID.TabIndex = 0;
            this._identifierID.Value = "";
            // 
            // _identifierAssigningAuthority
            // 
            this._identifierAssigningAuthority.DataSource = null;
            this._identifierAssigningAuthority.LabelText = "Assigning Authority";
            this._identifierAssigningAuthority.Location = new System.Drawing.Point(29, 133);
            this._identifierAssigningAuthority.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._identifierAssigningAuthority.Name = "_identifierAssigningAuthority";
            this._identifierAssigningAuthority.Size = new System.Drawing.Size(192, 41);
            this._identifierAssigningAuthority.TabIndex = 1;
            this._identifierAssigningAuthority.Value = null;
            // 
            // _identifierType
            // 
            this._identifierType.DataSource = null;
            this._identifierType.LabelText = "Identifier Type";
            this._identifierType.Location = new System.Drawing.Point(29, 17);
            this._identifierType.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._identifierType.Name = "_identifierType";
            this._identifierType.Size = new System.Drawing.Size(192, 41);
            this._identifierType.TabIndex = 2;
            this._identifierType.Value = null;
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(251, 35);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 3;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(251, 75);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 4;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // PatientIdentifierEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._acceptButton);
            this.Controls.Add(this._identifierType);
            this.Controls.Add(this._identifierAssigningAuthority);
            this.Controls.Add(this._identifierID);
            this.Name = "PatientIdentifierEditorControl";
            this.Size = new System.Drawing.Size(367, 195);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Controls.WinForms.TextField _identifierID;
        private ClearCanvas.Controls.WinForms.ComboBoxField _identifierAssigningAuthority;
        private ClearCanvas.Controls.WinForms.ComboBoxField _identifierType;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.Button _cancelButton;
    }
}
