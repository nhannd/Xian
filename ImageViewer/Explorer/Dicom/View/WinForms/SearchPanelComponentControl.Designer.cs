namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    partial class SearchPanelComponentControl
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
            this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._patientIDLabel = new System.Windows.Forms.Label();
            this._clearButton = new System.Windows.Forms.Button();
            this._accessionNumberLabel = new System.Windows.Forms.Label();
            this._accessionNumber = new System.Windows.Forms.TextBox();
            this._lastNameLabel = new System.Windows.Forms.Label();
            this._lastName = new System.Windows.Forms.TextBox();
            this._firstName = new System.Windows.Forms.TextBox();
            this._studyDescriptionLabel = new System.Windows.Forms.Label();
            this._studyDescription = new System.Windows.Forms.TextBox();
            this._studyDateLabel = new System.Windows.Forms.Label();
            this._dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this._firstNameLabel = new System.Windows.Forms.Label();
            this._searchButton = new System.Windows.Forms.Button();
            this._patientID = new System.Windows.Forms.TextBox();
            this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
            this._searchTodayButton = new System.Windows.Forms.Button();
            this._tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tableLayoutPanel
            // 
            this._tableLayoutPanel.ColumnCount = 5;
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanel.Controls.Add(this._patientIDLabel, 0, 0);
            this._tableLayoutPanel.Controls.Add(this._accessionNumberLabel, 2, 0);
            this._tableLayoutPanel.Controls.Add(this._searchTodayButton, 4, 3);
            this._tableLayoutPanel.Controls.Add(this._lastNameLabel, 0, 1);
            this._tableLayoutPanel.Controls.Add(this._lastName, 1, 1);
            this._tableLayoutPanel.Controls.Add(this._studyDescriptionLabel, 0, 3);
            this._tableLayoutPanel.Controls.Add(this._studyDescription, 1, 3);
            this._tableLayoutPanel.Controls.Add(this._studyDateLabel, 0, 2);
            this._tableLayoutPanel.Controls.Add(this._dateTimePicker1, 1, 2);
            this._tableLayoutPanel.Controls.Add(this._firstNameLabel, 2, 1);
            this._tableLayoutPanel.Controls.Add(this._patientID, 1, 0);
            this._tableLayoutPanel.Controls.Add(this._clearButton, 3, 2);
            this._tableLayoutPanel.Controls.Add(this._searchButton, 3, 3);
            this._tableLayoutPanel.Controls.Add(this._firstName, 3, 1);
            this._tableLayoutPanel.Controls.Add(this._accessionNumber, 3, 0);
            this._tableLayoutPanel.Location = new System.Drawing.Point(8, 36);
            this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._tableLayoutPanel.Name = "_tableLayoutPanel";
            this._tableLayoutPanel.RowCount = 4;
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanel.Size = new System.Drawing.Size(722, 130);
            this._tableLayoutPanel.TabIndex = 2;
            // 
            // _patientIDLabel
            // 
            this._patientIDLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._patientIDLabel.AutoSize = true;
            this._patientIDLabel.Location = new System.Drawing.Point(4, 6);
            this._patientIDLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._patientIDLabel.Name = "_patientIDLabel";
            this._patientIDLabel.Size = new System.Drawing.Size(69, 17);
            this._patientIDLabel.TabIndex = 1;
            this._patientIDLabel.Text = "Patient ID";
            // 
            // _clearButton
            // 
            this._clearButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._clearButton.Location = new System.Drawing.Point(414, 64);
            this._clearButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(100, 28);
            this._clearButton.TabIndex = 7;
            this._clearButton.Text = "Clear";
            this._clearButton.UseVisualStyleBackColor = true;
            // 
            // _accessionNumberLabel
            // 
            this._accessionNumberLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._accessionNumberLabel.AutoSize = true;
            this._accessionNumberLabel.Location = new System.Drawing.Point(326, 6);
            this._accessionNumberLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._accessionNumberLabel.Name = "_accessionNumberLabel";
            this._accessionNumberLabel.Size = new System.Drawing.Size(80, 17);
            this._accessionNumberLabel.TabIndex = 2;
            this._accessionNumberLabel.Text = "Accession#";
            // 
            // _accessionNumber
            // 
            this._accessionNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._accessionNumber.Location = new System.Drawing.Point(414, 4);
            this._accessionNumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._accessionNumber.Name = "_accessionNumber";
            this._accessionNumber.Size = new System.Drawing.Size(194, 22);
            this._accessionNumber.TabIndex = 1;
            // 
            // _lastNameLabel
            // 
            this._lastNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lastNameLabel.AutoSize = true;
            this._lastNameLabel.Location = new System.Drawing.Point(4, 36);
            this._lastNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._lastNameLabel.Name = "_lastNameLabel";
            this._lastNameLabel.Size = new System.Drawing.Size(76, 17);
            this._lastNameLabel.TabIndex = 2;
            this._lastNameLabel.Text = "Last Name";
            // 
            // _lastName
            // 
            this._lastName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._lastName.Location = new System.Drawing.Point(131, 34);
            this._lastName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._lastName.Name = "_lastName";
            this._lastName.Size = new System.Drawing.Size(187, 22);
            this._lastName.TabIndex = 2;
            // 
            // _firstName
            // 
            this._firstName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._firstName.Location = new System.Drawing.Point(414, 34);
            this._firstName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._firstName.Name = "_firstName";
            this._firstName.Size = new System.Drawing.Size(194, 22);
            this._firstName.TabIndex = 3;
            // 
            // _studyDescriptionLabel
            // 
            this._studyDescriptionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._studyDescriptionLabel.AutoSize = true;
            this._studyDescriptionLabel.Location = new System.Drawing.Point(4, 105);
            this._studyDescriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._studyDescriptionLabel.Name = "_studyDescriptionLabel";
            this._studyDescriptionLabel.Size = new System.Drawing.Size(119, 17);
            this._studyDescriptionLabel.TabIndex = 6;
            this._studyDescriptionLabel.Text = "Study Description";
            // 
            // _studyDescription
            // 
            this._studyDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._studyDescription.Location = new System.Drawing.Point(131, 102);
            this._studyDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._studyDescription.Name = "_studyDescription";
            this._studyDescription.Size = new System.Drawing.Size(187, 22);
            this._studyDescription.TabIndex = 5;
            // 
            // _studyDateLabel
            // 
            this._studyDateLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._studyDateLabel.AutoSize = true;
            this._studyDateLabel.Location = new System.Drawing.Point(4, 69);
            this._studyDateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._studyDateLabel.Name = "_studyDateLabel";
            this._studyDateLabel.Size = new System.Drawing.Size(78, 17);
            this._studyDateLabel.TabIndex = 3;
            this._studyDateLabel.Text = "Study Date";
            // 
            // _dateTimePicker1
            // 
            this._dateTimePicker1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._dateTimePicker1.Enabled = false;
            this._dateTimePicker1.Location = new System.Drawing.Point(131, 67);
            this._dateTimePicker1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._dateTimePicker1.Name = "_dateTimePicker1";
            this._dateTimePicker1.Size = new System.Drawing.Size(187, 22);
            this._dateTimePicker1.TabIndex = 4;
            // 
            // _firstNameLabel
            // 
            this._firstNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._firstNameLabel.AutoSize = true;
            this._firstNameLabel.Location = new System.Drawing.Point(326, 36);
            this._firstNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._firstNameLabel.Name = "_firstNameLabel";
            this._firstNameLabel.Size = new System.Drawing.Size(76, 17);
            this._firstNameLabel.TabIndex = 4;
            this._firstNameLabel.Text = "First Name";
            // 
            // _searchButton
            // 
            this._searchButton.Location = new System.Drawing.Point(414, 100);
            this._searchButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(100, 27);
            this._searchButton.TabIndex = 6;
            this._searchButton.Text = "Search";
            this._searchButton.UseVisualStyleBackColor = true;
            // 
            // _patientID
            // 
            this._patientID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._patientID.Location = new System.Drawing.Point(131, 4);
            this._patientID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._patientID.Name = "_patientID";
            this._patientID.Size = new System.Drawing.Size(187, 22);
            this._patientID.TabIndex = 0;
            // 
            // _titleBar
            // 
            this._titleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this._titleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._titleBar.Location = new System.Drawing.Point(0, 0);
            this._titleBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
            this._titleBar.Name = "_titleBar";
            this._titleBar.Size = new System.Drawing.Size(1053, 28);
            this._titleBar.TabIndex = 3;
            this._titleBar.Text = "Search";
            // 
            // _searchTodayButton
            // 
            this._searchTodayButton.Location = new System.Drawing.Point(615, 99);
            this._searchTodayButton.Name = "_searchTodayButton";
            this._searchTodayButton.Size = new System.Drawing.Size(100, 29);
            this._searchTodayButton.TabIndex = 4;
            this._searchTodayButton.Text = "Today";
            this._searchTodayButton.UseVisualStyleBackColor = true;
            this._searchTodayButton.Click += new System.EventHandler(this.OnSearchTodayButtonClicked);
            // 
            // SearchPanelComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this._titleBar);
            this.Controls.Add(this._tableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SearchPanelComponentControl";
            this.Size = new System.Drawing.Size(1053, 351);
            this._tableLayoutPanel.ResumeLayout(false);
            this._tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private System.Windows.Forms.Label _patientIDLabel;
		private System.Windows.Forms.Button _clearButton;
		private System.Windows.Forms.Label _accessionNumberLabel;
		private System.Windows.Forms.TextBox _accessionNumber;
		private System.Windows.Forms.Label _lastNameLabel;
		private System.Windows.Forms.TextBox _lastName;
		private System.Windows.Forms.TextBox _firstName;
		private System.Windows.Forms.Label _studyDescriptionLabel;
		private System.Windows.Forms.TextBox _studyDescription;
		private System.Windows.Forms.Label _studyDateLabel;
		private System.Windows.Forms.DateTimePicker _dateTimePicker1;
		private System.Windows.Forms.Label _firstNameLabel;
		private System.Windows.Forms.Button _searchButton;
		private System.Windows.Forms.TextBox _patientID;
		private Crownwood.DotNetMagic.Controls.TitleBar _titleBar;
        private System.Windows.Forms.Button _searchTodayButton;
    }
}
