namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class PatientSearchComponentControl
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
            this._mrn = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._healthcard = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._familyName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._givenName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._dateOfBirth = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._site = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._sex = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._searchButton = new System.Windows.Forms.Button();
            this._searchResults = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _mrn
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._mrn, 2);
            this._mrn.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mrn.LabelText = "MRN";
            this._mrn.Location = new System.Drawing.Point(2, 2);
            this._mrn.Margin = new System.Windows.Forms.Padding(2);
            this._mrn.Mask = "";
            this._mrn.Name = "_mrn";
            this._mrn.Size = new System.Drawing.Size(304, 41);
            this._mrn.TabIndex = 0;
            this._mrn.Value = null;
            // 
            // _healthcard
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._healthcard, 2);
            this._healthcard.Dock = System.Windows.Forms.DockStyle.Fill;
            this._healthcard.LabelText = "Healthcard";
            this._healthcard.Location = new System.Drawing.Point(2, 92);
            this._healthcard.Margin = new System.Windows.Forms.Padding(2);
            this._healthcard.Mask = "";
            this._healthcard.Name = "_healthcard";
            this._healthcard.Size = new System.Drawing.Size(304, 41);
            this._healthcard.TabIndex = 2;
            this._healthcard.Value = null;
            // 
            // _familyName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._familyName, 2);
            this._familyName.Dock = System.Windows.Forms.DockStyle.Fill;
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(2, 137);
            this._familyName.Margin = new System.Windows.Forms.Padding(2);
            this._familyName.Mask = "";
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(304, 41);
            this._familyName.TabIndex = 3;
            this._familyName.Value = null;
            // 
            // _givenName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._givenName, 2);
            this._givenName.Dock = System.Windows.Forms.DockStyle.Fill;
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(2, 182);
            this._givenName.Margin = new System.Windows.Forms.Padding(2);
            this._givenName.Mask = "";
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(304, 41);
            this._givenName.TabIndex = 4;
            this._givenName.Value = null;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._searchResults);
            this.splitContainer1.Size = new System.Drawing.Size(308, 629);
            this.splitContainer1.SplitterDistance = 305;
            this.splitContainer1.TabIndex = 13;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this._mrn, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._dateOfBirth, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this._site, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._sex, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this._healthcard, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._familyName, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._givenName, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this._searchButton, 1, 7);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(308, 305);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // _dateOfBirth
            // 
            this._dateOfBirth.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dateOfBirth.LabelText = "Date Of Birth";
            this._dateOfBirth.Location = new System.Drawing.Point(156, 227);
            this._dateOfBirth.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._dateOfBirth.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._dateOfBirth.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._dateOfBirth.Name = "_dateOfBirth";
            this._dateOfBirth.Nullable = true;
            this._dateOfBirth.Size = new System.Drawing.Size(150, 41);
            this._dateOfBirth.TabIndex = 6;
            this._dateOfBirth.Value = null;
            // 
            // _site
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._site, 2);
            this._site.Dock = System.Windows.Forms.DockStyle.Fill;
            this._site.LabelText = "Site";
            this._site.Location = new System.Drawing.Point(2, 47);
            this._site.Margin = new System.Windows.Forms.Padding(2);
            this._site.Mask = "";
            this._site.Name = "_site";
            this._site.Size = new System.Drawing.Size(304, 41);
            this._site.TabIndex = 1;
            this._site.Value = null;
            // 
            // _sex
            // 
            this._sex.DataSource = null;
            this._sex.DisplayMember = "";
            this._sex.Dock = System.Windows.Forms.DockStyle.Fill;
            this._sex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._sex.LabelText = "Sex";
            this._sex.Location = new System.Drawing.Point(2, 227);
            this._sex.Margin = new System.Windows.Forms.Padding(2);
            this._sex.Name = "_sex";
            this._sex.Size = new System.Drawing.Size(150, 41);
            this._sex.TabIndex = 5;
            this._sex.Value = null;
            // 
            // _searchButton
            // 
            this._searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._searchButton.Location = new System.Drawing.Point(246, 272);
            this._searchButton.Margin = new System.Windows.Forms.Padding(2);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(60, 27);
            this._searchButton.TabIndex = 7;
            this._searchButton.Text = "Search";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
            // 
            // _searchResults
            // 
            this._searchResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this._searchResults.Location = new System.Drawing.Point(0, 0);
            this._searchResults.Name = "_searchResults";
            this._searchResults.ReadOnly = false;
            this._searchResults.Size = new System.Drawing.Size(308, 320);
            this._searchResults.TabIndex = 0;
            this._searchResults.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this._searchResults.ItemDoubleClicked += new System.EventHandler(this._searchResults_ItemDoubleClicked);
            // 
            // PatientSearchComponentControl
            // 
            this.AcceptButton = this._searchButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PatientSearchComponentControl";
            this.Size = new System.Drawing.Size(308, 629);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _mrn;
        private ClearCanvas.Desktop.View.WinForms.TextField _healthcard;
        private ClearCanvas.Desktop.View.WinForms.TextField _familyName;
        private ClearCanvas.Desktop.View.WinForms.TextField _givenName;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ClearCanvas.Desktop.View.WinForms.TableView _searchResults;
        private System.Windows.Forms.Button _searchButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _site;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _dateOfBirth;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _sex;
    }
}
