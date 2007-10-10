namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class SearchComponentControl
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
            this._accessionNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._mrn = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._healthcard = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._familyName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._givenName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._searchButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._showActive = new System.Windows.Forms.CheckBox();
            this._keepOpen = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this._accessionNumber, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this._mrn, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._healthcard, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._familyName, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._givenName, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._searchButton, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(219, 293);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // _accessionNumber
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._accessionNumber, 2);
            this._accessionNumber.LabelText = "Accession #";
            this._accessionNumber.Location = new System.Drawing.Point(2, 182);
            this._accessionNumber.Margin = new System.Windows.Forms.Padding(2);
            this._accessionNumber.Mask = "";
            this._accessionNumber.Name = "_accessionNumber";
            this._accessionNumber.Size = new System.Drawing.Size(195, 41);
            this._accessionNumber.TabIndex = 8;
            this._accessionNumber.Value = null;
            // 
            // _mrn
            // 
            this._mrn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._mrn, 2);
            this._mrn.LabelText = "MRN";
            this._mrn.Location = new System.Drawing.Point(2, 2);
            this._mrn.Margin = new System.Windows.Forms.Padding(2);
            this._mrn.Mask = "";
            this._mrn.Name = "_mrn";
            this._mrn.Size = new System.Drawing.Size(200, 41);
            this._mrn.TabIndex = 0;
            this._mrn.Value = null;
            // 
            // _healthcard
            // 
            this._healthcard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._healthcard, 2);
            this._healthcard.LabelText = "Healthcard";
            this._healthcard.Location = new System.Drawing.Point(2, 47);
            this._healthcard.Margin = new System.Windows.Forms.Padding(2);
            this._healthcard.Mask = "";
            this._healthcard.Name = "_healthcard";
            this._healthcard.Size = new System.Drawing.Size(200, 41);
            this._healthcard.TabIndex = 1;
            this._healthcard.Value = null;
            // 
            // _familyName
            // 
            this._familyName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._familyName, 2);
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(2, 92);
            this._familyName.Margin = new System.Windows.Forms.Padding(2);
            this._familyName.Mask = "";
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(200, 41);
            this._familyName.TabIndex = 2;
            this._familyName.Value = null;
            // 
            // _givenName
            // 
            this._givenName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._givenName, 2);
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(2, 137);
            this._givenName.Margin = new System.Windows.Forms.Padding(2);
            this._givenName.Mask = "";
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(200, 41);
            this._givenName.TabIndex = 3;
            this._givenName.Value = null;
            // 
            // _searchButton
            // 
            this._searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._searchButton.Location = new System.Drawing.Point(142, 227);
            this._searchButton.Margin = new System.Windows.Forms.Padding(2);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(60, 27);
            this._searchButton.TabIndex = 7;
            this._searchButton.Text = "Search";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this._showActive);
            this.flowLayoutPanel1.Controls.Add(this._keepOpen);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 228);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(96, 62);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // _showActive
            // 
            this._showActive.AutoSize = true;
            this._showActive.Checked = true;
            this._showActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this._showActive.Location = new System.Drawing.Point(3, 3);
            this._showActive.Name = "_showActive";
            this._showActive.Size = new System.Drawing.Size(80, 17);
            this._showActive.TabIndex = 10;
            this._showActive.Text = "Active Only";
            this._showActive.UseVisualStyleBackColor = true;
            // 
            // _keepOpen
            // 
            this._keepOpen.AutoSize = true;
            this._keepOpen.Location = new System.Drawing.Point(2, 25);
            this._keepOpen.Margin = new System.Windows.Forms.Padding(2);
            this._keepOpen.Name = "_keepOpen";
            this._keepOpen.Size = new System.Drawing.Size(80, 17);
            this._keepOpen.TabIndex = 11;
            this._keepOpen.Text = "Keep Open";
            this._keepOpen.UseVisualStyleBackColor = true;
            // 
            // SearchComponentControl
            // 
            this.AcceptButton = this._searchButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SearchComponentControl";
            this.Size = new System.Drawing.Size(219, 293);
            this.Load += new System.EventHandler(this.SearchComponentControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _familyName;
        private ClearCanvas.Desktop.View.WinForms.TextField _givenName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.TextField _healthcard;
        private ClearCanvas.Desktop.View.WinForms.TextField _mrn;
        private System.Windows.Forms.Button _searchButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _accessionNumber;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox _showActive;
        private System.Windows.Forms.CheckBox _keepOpen;
    }
}
