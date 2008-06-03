namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class ExternalPractitionerMergeComponentControl
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
			this._duplicates = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._duplicateGroupBox = new System.Windows.Forms.GroupBox();
			this._previewGroupBox = new System.Windows.Forms.GroupBox();
			this._acceptButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._billingNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._middleName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._givenName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._familyName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._licenseNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._duplicateGroupBox.SuspendLayout();
			this._previewGroupBox.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _duplicates
			// 
			this._duplicates.Dock = System.Windows.Forms.DockStyle.Fill;
			this._duplicates.Location = new System.Drawing.Point(3, 16);
			this._duplicates.MultiSelect = false;
			this._duplicates.Name = "_duplicates";
			this._duplicates.ReadOnly = false;
			this._duplicates.Size = new System.Drawing.Size(336, 203);
			this._duplicates.TabIndex = 0;
			this._duplicates.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			// 
			// _duplicateGroupBox
			// 
			this._duplicateGroupBox.Controls.Add(this._duplicates);
			this._duplicateGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._duplicateGroupBox.Location = new System.Drawing.Point(0, 0);
			this._duplicateGroupBox.Name = "_duplicateGroupBox";
			this._duplicateGroupBox.Size = new System.Drawing.Size(342, 222);
			this._duplicateGroupBox.TabIndex = 1;
			this._duplicateGroupBox.TabStop = false;
			this._duplicateGroupBox.Text = "List of duplicate practitioners";
			// 
			// _previewGroupBox
			// 
			this._previewGroupBox.Controls.Add(this._billingNumber);
			this._previewGroupBox.Controls.Add(this._middleName);
			this._previewGroupBox.Controls.Add(this._givenName);
			this._previewGroupBox.Controls.Add(this._familyName);
			this._previewGroupBox.Controls.Add(this._licenseNumber);
			this._previewGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._previewGroupBox.Location = new System.Drawing.Point(0, 0);
			this._previewGroupBox.Name = "_previewGroupBox";
			this._previewGroupBox.Size = new System.Drawing.Size(342, 256);
			this._previewGroupBox.TabIndex = 2;
			this._previewGroupBox.TabStop = false;
			this._previewGroupBox.Text = "Preview of the unique record";
			// 
			// _acceptButton
			// 
			this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._acceptButton.Location = new System.Drawing.Point(189, 491);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 3;
			this._acceptButton.Text = "Merge";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(270, 491);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 4;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._duplicateGroupBox);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._previewGroupBox);
			this.splitContainer1.Size = new System.Drawing.Size(342, 482);
			this.splitContainer1.SplitterDistance = 222;
			this.splitContainer1.TabIndex = 5;
			// 
			// _billingNumber
			// 
			this._billingNumber.LabelText = "Billing Number";
			this._billingNumber.Location = new System.Drawing.Point(171, 127);
			this._billingNumber.Margin = new System.Windows.Forms.Padding(2);
			this._billingNumber.Mask = "";
			this._billingNumber.Name = "_billingNumber";
			this._billingNumber.PasswordChar = '\0';
			this._billingNumber.ReadOnly = true;
			this._billingNumber.Size = new System.Drawing.Size(150, 41);
			this._billingNumber.TabIndex = 10;
			this._billingNumber.ToolTip = null;
			this._billingNumber.Value = null;
			// 
			// _middleName
			// 
			this._middleName.LabelText = "Middle Name";
			this._middleName.Location = new System.Drawing.Point(5, 73);
			this._middleName.Margin = new System.Windows.Forms.Padding(2);
			this._middleName.Mask = "";
			this._middleName.Name = "_middleName";
			this._middleName.PasswordChar = '\0';
			this._middleName.ReadOnly = true;
			this._middleName.Size = new System.Drawing.Size(150, 41);
			this._middleName.TabIndex = 8;
			this._middleName.ToolTip = null;
			this._middleName.Value = null;
			// 
			// _givenName
			// 
			this._givenName.LabelText = "Given Name";
			this._givenName.Location = new System.Drawing.Point(171, 18);
			this._givenName.Margin = new System.Windows.Forms.Padding(2);
			this._givenName.Mask = "";
			this._givenName.Name = "_givenName";
			this._givenName.PasswordChar = '\0';
			this._givenName.ReadOnly = true;
			this._givenName.Size = new System.Drawing.Size(150, 41);
			this._givenName.TabIndex = 7;
			this._givenName.ToolTip = null;
			this._givenName.Value = null;
			// 
			// _familyName
			// 
			this._familyName.LabelText = "Family Name";
			this._familyName.Location = new System.Drawing.Point(5, 18);
			this._familyName.Margin = new System.Windows.Forms.Padding(2);
			this._familyName.Mask = "";
			this._familyName.Name = "_familyName";
			this._familyName.PasswordChar = '\0';
			this._familyName.ReadOnly = true;
			this._familyName.Size = new System.Drawing.Size(150, 41);
			this._familyName.TabIndex = 6;
			this._familyName.ToolTip = null;
			this._familyName.Value = null;
			// 
			// _licenseNumber
			// 
			this._licenseNumber.LabelText = "License Number";
			this._licenseNumber.Location = new System.Drawing.Point(5, 127);
			this._licenseNumber.Margin = new System.Windows.Forms.Padding(2);
			this._licenseNumber.Mask = "";
			this._licenseNumber.Name = "_licenseNumber";
			this._licenseNumber.PasswordChar = '\0';
			this._licenseNumber.ReadOnly = true;
			this._licenseNumber.Size = new System.Drawing.Size(150, 41);
			this._licenseNumber.TabIndex = 9;
			this._licenseNumber.ToolTip = null;
			this._licenseNumber.Value = null;
			// 
			// ExternalPractitionerMergeComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Name = "ExternalPractitionerMergeComponentControl";
			this.Size = new System.Drawing.Size(351, 527);
			this._duplicateGroupBox.ResumeLayout(false);
			this._previewGroupBox.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _duplicates;
		private System.Windows.Forms.GroupBox _duplicateGroupBox;
		private System.Windows.Forms.GroupBox _previewGroupBox;
		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private ClearCanvas.Desktop.View.WinForms.TextField _billingNumber;
		private ClearCanvas.Desktop.View.WinForms.TextField _middleName;
		private ClearCanvas.Desktop.View.WinForms.TextField _givenName;
		private ClearCanvas.Desktop.View.WinForms.TextField _familyName;
		private ClearCanvas.Desktop.View.WinForms.TextField _licenseNumber;
    }
}
