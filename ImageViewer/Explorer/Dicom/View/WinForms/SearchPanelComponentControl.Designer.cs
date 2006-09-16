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
			this._tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.ColumnCount = 4;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.Controls.Add(this._patientIDLabel, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._clearButton, 3, 2);
			this._tableLayoutPanel.Controls.Add(this._accessionNumberLabel, 2, 0);
			this._tableLayoutPanel.Controls.Add(this._accessionNumber, 3, 0);
			this._tableLayoutPanel.Controls.Add(this._lastNameLabel, 0, 1);
			this._tableLayoutPanel.Controls.Add(this._lastName, 1, 1);
			this._tableLayoutPanel.Controls.Add(this._firstName, 3, 1);
			this._tableLayoutPanel.Controls.Add(this._studyDescriptionLabel, 0, 3);
			this._tableLayoutPanel.Controls.Add(this._studyDescription, 1, 3);
			this._tableLayoutPanel.Controls.Add(this._studyDateLabel, 0, 2);
			this._tableLayoutPanel.Controls.Add(this._dateTimePicker1, 1, 2);
			this._tableLayoutPanel.Controls.Add(this._firstNameLabel, 2, 1);
			this._tableLayoutPanel.Controls.Add(this._searchButton, 3, 3);
			this._tableLayoutPanel.Controls.Add(this._patientID, 1, 0);
			this._tableLayoutPanel.Location = new System.Drawing.Point(6, 29);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 4;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.Size = new System.Drawing.Size(469, 106);
			this._tableLayoutPanel.TabIndex = 2;
			// 
			// _patientIDLabel
			// 
			this._patientIDLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._patientIDLabel.AutoSize = true;
			this._patientIDLabel.Location = new System.Drawing.Point(3, 6);
			this._patientIDLabel.Name = "_patientIDLabel";
			this._patientIDLabel.Size = new System.Drawing.Size(54, 13);
			this._patientIDLabel.TabIndex = 1;
			this._patientIDLabel.Text = "Patient ID";
			// 
			// _clearButton
			// 
			this._clearButton.Anchor = System.Windows.Forms.AnchorStyles.None;
			this._clearButton.Location = new System.Drawing.Point(353, 55);
			this._clearButton.Name = "_clearButton";
			this._clearButton.Size = new System.Drawing.Size(75, 23);
			this._clearButton.TabIndex = 7;
			this._clearButton.Text = "Clear";
			this._clearButton.UseVisualStyleBackColor = true;
			// 
			// _accessionNumberLabel
			// 
			this._accessionNumberLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._accessionNumberLabel.AutoSize = true;
			this._accessionNumberLabel.Location = new System.Drawing.Point(246, 6);
			this._accessionNumberLabel.Name = "_accessionNumberLabel";
			this._accessionNumberLabel.Size = new System.Drawing.Size(63, 13);
			this._accessionNumberLabel.TabIndex = 2;
			this._accessionNumberLabel.Text = "Accession#";
			// 
			// _accessionNumber
			// 
			this._accessionNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._accessionNumber.Location = new System.Drawing.Point(315, 3);
			this._accessionNumber.Name = "_accessionNumber";
			this._accessionNumber.Size = new System.Drawing.Size(151, 20);
			this._accessionNumber.TabIndex = 1;
			// 
			// _lastNameLabel
			// 
			this._lastNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._lastNameLabel.AutoSize = true;
			this._lastNameLabel.Location = new System.Drawing.Point(3, 32);
			this._lastNameLabel.Name = "_lastNameLabel";
			this._lastNameLabel.Size = new System.Drawing.Size(58, 13);
			this._lastNameLabel.TabIndex = 2;
			this._lastNameLabel.Text = "Last Name";
			// 
			// _lastName
			// 
			this._lastName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._lastName.Location = new System.Drawing.Point(99, 29);
			this._lastName.Name = "_lastName";
			this._lastName.Size = new System.Drawing.Size(141, 20);
			this._lastName.TabIndex = 2;
			// 
			// _firstName
			// 
			this._firstName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._firstName.Location = new System.Drawing.Point(315, 29);
			this._firstName.Name = "_firstName";
			this._firstName.Size = new System.Drawing.Size(151, 20);
			this._firstName.TabIndex = 3;
			// 
			// _studyDescriptionLabel
			// 
			this._studyDescriptionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._studyDescriptionLabel.AutoSize = true;
			this._studyDescriptionLabel.Location = new System.Drawing.Point(3, 88);
			this._studyDescriptionLabel.Name = "_studyDescriptionLabel";
			this._studyDescriptionLabel.Size = new System.Drawing.Size(90, 13);
			this._studyDescriptionLabel.TabIndex = 6;
			this._studyDescriptionLabel.Text = "Study Description";
			// 
			// _studyDescription
			// 
			this._studyDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._studyDescription.Location = new System.Drawing.Point(99, 85);
			this._studyDescription.Name = "_studyDescription";
			this._studyDescription.Size = new System.Drawing.Size(141, 20);
			this._studyDescription.TabIndex = 5;
			// 
			// _studyDateLabel
			// 
			this._studyDateLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._studyDateLabel.AutoSize = true;
			this._studyDateLabel.Location = new System.Drawing.Point(3, 60);
			this._studyDateLabel.Name = "_studyDateLabel";
			this._studyDateLabel.Size = new System.Drawing.Size(60, 13);
			this._studyDateLabel.TabIndex = 3;
			this._studyDateLabel.Text = "Study Date";
			// 
			// _dateTimePicker1
			// 
			this._dateTimePicker1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._dateTimePicker1.Enabled = false;
			this._dateTimePicker1.Location = new System.Drawing.Point(99, 56);
			this._dateTimePicker1.Name = "_dateTimePicker1";
			this._dateTimePicker1.Size = new System.Drawing.Size(141, 20);
			this._dateTimePicker1.TabIndex = 4;
			// 
			// _firstNameLabel
			// 
			this._firstNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._firstNameLabel.AutoSize = true;
			this._firstNameLabel.Location = new System.Drawing.Point(246, 32);
			this._firstNameLabel.Name = "_firstNameLabel";
			this._firstNameLabel.Size = new System.Drawing.Size(57, 13);
			this._firstNameLabel.TabIndex = 4;
			this._firstNameLabel.Text = "First Name";
			// 
			// _searchButton
			// 
			this._searchButton.Anchor = System.Windows.Forms.AnchorStyles.None;
			this._searchButton.Location = new System.Drawing.Point(353, 84);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(75, 22);
			this._searchButton.TabIndex = 6;
			this._searchButton.Text = "Search";
			this._searchButton.UseVisualStyleBackColor = true;
			// 
			// _patientID
			// 
			this._patientID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._patientID.Location = new System.Drawing.Point(99, 3);
			this._patientID.Name = "_patientID";
			this._patientID.Size = new System.Drawing.Size(141, 20);
			this._patientID.TabIndex = 0;
			// 
			// _titleBar
			// 
			this._titleBar.Dock = System.Windows.Forms.DockStyle.Top;
			this._titleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._titleBar.Location = new System.Drawing.Point(0, 0);
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Size = new System.Drawing.Size(480, 23);
			this._titleBar.TabIndex = 3;
			this._titleBar.Text = "Search";
			// 
			// SearchPanelComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._titleBar);
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "SearchPanelComponentControl";
			this.Size = new System.Drawing.Size(480, 146);
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
    }
}
