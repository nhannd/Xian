namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class UserEditorComponentControl
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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._authorityGroups = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._userId = new ClearCanvas.Controls.WinForms.TextField();
            this._familyName = new ClearCanvas.Controls.WinForms.TextField();
            this._givenName = new ClearCanvas.Controls.WinForms.TextField();
            this._middleName = new ClearCanvas.Controls.WinForms.TextField();
            this._suffix = new ClearCanvas.Controls.WinForms.TextField();
            this._prefix = new ClearCanvas.Controls.WinForms.TextField();
            this._degree = new ClearCanvas.Controls.WinForms.TextField();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this._authorityGroups, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._userId, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._familyName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._givenName, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this._middleName, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this._suffix, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this._prefix, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._degree, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(515, 410);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _authorityGroups
            // 
            this._authorityGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._authorityGroups, 3);
            this._authorityGroups.Location = new System.Drawing.Point(3, 138);
            this._authorityGroups.MenuModel = null;
            this._authorityGroups.Name = "_authorityGroups";
            this._authorityGroups.ReadOnly = false;
            this._authorityGroups.Selection = selection1;
            this._authorityGroups.Size = new System.Drawing.Size(509, 234);
            this._authorityGroups.TabIndex = 0;
            this._authorityGroups.Table = null;
            this._authorityGroups.ToolbarModel = null;
            this._authorityGroups.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._authorityGroups.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _userId
            // 
            this._userId.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._userId.LabelText = "User ID";
            this._userId.Location = new System.Drawing.Point(2, 2);
            this._userId.Margin = new System.Windows.Forms.Padding(2);
            this._userId.Mask = "";
            this._userId.Name = "_userId";
            this._userId.Size = new System.Drawing.Size(167, 41);
            this._userId.TabIndex = 1;
            this._userId.Value = null;
            // 
            // _familyName
            // 
            this._familyName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(2, 47);
            this._familyName.Margin = new System.Windows.Forms.Padding(2);
            this._familyName.Mask = "";
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(167, 41);
            this._familyName.TabIndex = 2;
            this._familyName.Value = null;
            // 
            // _givenName
            // 
            this._givenName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(173, 47);
            this._givenName.Margin = new System.Windows.Forms.Padding(2);
            this._givenName.Mask = "";
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(167, 41);
            this._givenName.TabIndex = 3;
            this._givenName.Value = null;
            // 
            // _middleName
            // 
            this._middleName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._middleName.LabelText = "Middle Name";
            this._middleName.Location = new System.Drawing.Point(344, 47);
            this._middleName.Margin = new System.Windows.Forms.Padding(2);
            this._middleName.Mask = "";
            this._middleName.Name = "_middleName";
            this._middleName.Size = new System.Drawing.Size(169, 41);
            this._middleName.TabIndex = 4;
            this._middleName.Value = null;
            // 
            // _suffix
            // 
            this._suffix.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._suffix.LabelText = "Suffix";
            this._suffix.Location = new System.Drawing.Point(173, 92);
            this._suffix.Margin = new System.Windows.Forms.Padding(2);
            this._suffix.Mask = "";
            this._suffix.Name = "_suffix";
            this._suffix.Size = new System.Drawing.Size(167, 41);
            this._suffix.TabIndex = 6;
            this._suffix.Value = null;
            // 
            // _prefix
            // 
            this._prefix.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._prefix.LabelText = "Prefix";
            this._prefix.Location = new System.Drawing.Point(2, 92);
            this._prefix.Margin = new System.Windows.Forms.Padding(2);
            this._prefix.Mask = "";
            this._prefix.Name = "_prefix";
            this._prefix.Size = new System.Drawing.Size(167, 41);
            this._prefix.TabIndex = 7;
            this._prefix.Value = null;
            // 
            // _degree
            // 
            this._degree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._degree.LabelText = "Degree";
            this._degree.Location = new System.Drawing.Point(344, 92);
            this._degree.Margin = new System.Windows.Forms.Padding(2);
            this._degree.Mask = "";
            this._degree.Name = "_degree";
            this._degree.Size = new System.Drawing.Size(169, 41);
            this._degree.TabIndex = 8;
            this._degree.Value = null;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 3);
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._acceptButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 378);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(509, 29);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(431, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 0;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(350, 3);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 1;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // UserEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UserEditorComponentControl";
            this.Size = new System.Drawing.Size(521, 416);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.TableView _authorityGroups;
        private ClearCanvas.Controls.WinForms.TextField _userId;
        private ClearCanvas.Controls.WinForms.TextField _familyName;
        private ClearCanvas.Controls.WinForms.TextField _givenName;
        private ClearCanvas.Controls.WinForms.TextField _middleName;
        private ClearCanvas.Controls.WinForms.TextField _suffix;
        private ClearCanvas.Controls.WinForms.TextField _prefix;
        private ClearCanvas.Controls.WinForms.TextField _degree;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;

    }
}
