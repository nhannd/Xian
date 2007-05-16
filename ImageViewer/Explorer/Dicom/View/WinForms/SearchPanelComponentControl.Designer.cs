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
			this._searchButton = new System.Windows.Forms.Button();
			this._searchLastWeekButton = new System.Windows.Forms.Button();
			this._patientIDLabel = new System.Windows.Forms.Label();
			this._accessionNumberLabel = new System.Windows.Forms.Label();
			this._lastNameLabel = new System.Windows.Forms.Label();
			this._lastName = new System.Windows.Forms.TextBox();
			this._studyDateFromLabel = new System.Windows.Forms.Label();
			this._firstNameLabel = new System.Windows.Forms.Label();
			this._patientID = new System.Windows.Forms.TextBox();
			this._firstName = new System.Windows.Forms.TextBox();
			this._accessionNumber = new System.Windows.Forms.TextBox();
			this._studyDateToLabel = new System.Windows.Forms.Label();
			this._studyDateTo = new ClearCanvas.Controls.WinForms.DateTimeField();
			this._studyDateFrom = new ClearCanvas.Controls.WinForms.DateTimeField();
			this._studyDescription = new System.Windows.Forms.TextBox();
			this._studyDescriptionLabel = new System.Windows.Forms.Label();
			this._clearButton = new System.Windows.Forms.Button();
			this._searchTodayButton = new System.Windows.Forms.Button();
			this._modalityLabel = new System.Windows.Forms.Label();
			this._modalityPicker = new ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms.ModalityPicker();
			this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
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
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
			this._tableLayoutPanel.Controls.Add(this._searchButton, 4, 0);
			this._tableLayoutPanel.Controls.Add(this._searchLastWeekButton, 4, 1);
			this._tableLayoutPanel.Controls.Add(this._patientIDLabel, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._accessionNumberLabel, 2, 0);
			this._tableLayoutPanel.Controls.Add(this._lastNameLabel, 0, 1);
			this._tableLayoutPanel.Controls.Add(this._lastName, 1, 1);
			this._tableLayoutPanel.Controls.Add(this._studyDateFromLabel, 0, 2);
			this._tableLayoutPanel.Controls.Add(this._firstNameLabel, 2, 1);
			this._tableLayoutPanel.Controls.Add(this._patientID, 1, 0);
			this._tableLayoutPanel.Controls.Add(this._firstName, 3, 1);
			this._tableLayoutPanel.Controls.Add(this._accessionNumber, 3, 0);
			this._tableLayoutPanel.Controls.Add(this._studyDateToLabel, 2, 2);
			this._tableLayoutPanel.Controls.Add(this._studyDateTo, 3, 2);
			this._tableLayoutPanel.Controls.Add(this._studyDateFrom, 1, 2);
			this._tableLayoutPanel.Controls.Add(this._studyDescription, 1, 3);
			this._tableLayoutPanel.Controls.Add(this._studyDescriptionLabel, 0, 3);
			this._tableLayoutPanel.Controls.Add(this._clearButton, 4, 3);
			this._tableLayoutPanel.Controls.Add(this._searchTodayButton, 4, 2);
			this._tableLayoutPanel.Controls.Add(this._modalityLabel, 2, 3);
			this._tableLayoutPanel.Controls.Add(this._modalityPicker, 3, 3);
			this._tableLayoutPanel.Location = new System.Drawing.Point(6, 36);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 4;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.Size = new System.Drawing.Size(574, 136);
			this._tableLayoutPanel.TabIndex = 2;
			// 
			// _searchButton
			// 
			this._searchButton.Location = new System.Drawing.Point(496, 3);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(75, 22);
			this._searchButton.TabIndex = 16;
			this._searchButton.Text = "Search";
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this.OnSearchButtonClicked);
			// 
			// _searchLastWeekButton
			// 
			this._searchLastWeekButton.Location = new System.Drawing.Point(496, 31);
			this._searchLastWeekButton.Name = "_searchLastWeekButton";
			this._searchLastWeekButton.Size = new System.Drawing.Size(75, 22);
			this._searchLastWeekButton.TabIndex = 17;
			this._searchLastWeekButton.Text = "Last 7 days";
			this._searchLastWeekButton.UseVisualStyleBackColor = true;
			this._searchLastWeekButton.Click += new System.EventHandler(this.OnSearchLastWeekButtonClick);
			// 
			// _patientIDLabel
			// 
			this._patientIDLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._patientIDLabel.AutoSize = true;
			this._patientIDLabel.Location = new System.Drawing.Point(3, 7);
			this._patientIDLabel.Name = "_patientIDLabel";
			this._patientIDLabel.Size = new System.Drawing.Size(54, 13);
			this._patientIDLabel.TabIndex = 0;
			this._patientIDLabel.Text = "Patient ID";
			// 
			// _accessionNumberLabel
			// 
			this._accessionNumberLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._accessionNumberLabel.AutoSize = true;
			this._accessionNumberLabel.Location = new System.Drawing.Point(263, 7);
			this._accessionNumberLabel.Name = "_accessionNumberLabel";
			this._accessionNumberLabel.Size = new System.Drawing.Size(63, 13);
			this._accessionNumberLabel.TabIndex = 2;
			this._accessionNumberLabel.Text = "Accession#";
			// 
			// _lastNameLabel
			// 
			this._lastNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._lastNameLabel.AutoSize = true;
			this._lastNameLabel.Location = new System.Drawing.Point(3, 35);
			this._lastNameLabel.Name = "_lastNameLabel";
			this._lastNameLabel.Size = new System.Drawing.Size(58, 13);
			this._lastNameLabel.TabIndex = 4;
			this._lastNameLabel.Text = "Last Name";
			// 
			// _lastName
			// 
			this._lastName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._lastName.Location = new System.Drawing.Point(101, 32);
			this._lastName.Name = "_lastName";
			this._lastName.Size = new System.Drawing.Size(156, 20);
			this._lastName.TabIndex = 5;
			// 
			// _studyDateFromLabel
			// 
			this._studyDateFromLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._studyDateFromLabel.AutoSize = true;
			this._studyDateFromLabel.Location = new System.Drawing.Point(3, 74);
			this._studyDateFromLabel.Name = "_studyDateFromLabel";
			this._studyDateFromLabel.Size = new System.Drawing.Size(92, 13);
			this._studyDateFromLabel.TabIndex = 8;
			this._studyDateFromLabel.Text = "Study Date (From)";
			// 
			// _firstNameLabel
			// 
			this._firstNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._firstNameLabel.AutoSize = true;
			this._firstNameLabel.Location = new System.Drawing.Point(263, 35);
			this._firstNameLabel.Name = "_firstNameLabel";
			this._firstNameLabel.Size = new System.Drawing.Size(57, 13);
			this._firstNameLabel.TabIndex = 6;
			this._firstNameLabel.Text = "First Name";
			// 
			// _patientID
			// 
			this._patientID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._patientID.Location = new System.Drawing.Point(101, 4);
			this._patientID.Name = "_patientID";
			this._patientID.Size = new System.Drawing.Size(156, 20);
			this._patientID.TabIndex = 1;
			// 
			// _firstName
			// 
			this._firstName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._firstName.Location = new System.Drawing.Point(332, 32);
			this._firstName.Name = "_firstName";
			this._firstName.Size = new System.Drawing.Size(158, 20);
			this._firstName.TabIndex = 7;
			// 
			// _accessionNumber
			// 
			this._accessionNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._accessionNumber.Location = new System.Drawing.Point(332, 4);
			this._accessionNumber.Name = "_accessionNumber";
			this._accessionNumber.Size = new System.Drawing.Size(158, 20);
			this._accessionNumber.TabIndex = 3;
			// 
			// _studyDateToLabel
			// 
			this._studyDateToLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._studyDateToLabel.Location = new System.Drawing.Point(263, 73);
			this._studyDateToLabel.Name = "_studyDateToLabel";
			this._studyDateToLabel.Size = new System.Drawing.Size(58, 14);
			this._studyDateToLabel.TabIndex = 10;
			this._studyDateToLabel.Text = "(To)";
			this._studyDateToLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// _studyDateTo
			// 
			this._studyDateTo.LabelText = "";
			this._studyDateTo.Location = new System.Drawing.Point(331, 58);
			this._studyDateTo.Margin = new System.Windows.Forms.Padding(2);
			this._studyDateTo.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._studyDateTo.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._studyDateTo.Name = "_studyDateTo";
			this._studyDateTo.Nullable = true;
			this._studyDateTo.ShowTime = false;
			this._studyDateTo.Size = new System.Drawing.Size(158, 45);
			this._studyDateTo.TabIndex = 11;
			this._studyDateTo.Value = null;
			// 
			// _studyDateFrom
			// 
			this._studyDateFrom.LabelText = "";
			this._studyDateFrom.Location = new System.Drawing.Point(100, 58);
			this._studyDateFrom.Margin = new System.Windows.Forms.Padding(2);
			this._studyDateFrom.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._studyDateFrom.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._studyDateFrom.Name = "_studyDateFrom";
			this._studyDateFrom.Nullable = true;
			this._studyDateFrom.ShowTime = false;
			this._studyDateFrom.Size = new System.Drawing.Size(156, 45);
			this._studyDateFrom.TabIndex = 9;
			this._studyDateFrom.Value = null;
			// 
			// _studyDescription
			// 
			this._studyDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._studyDescription.Location = new System.Drawing.Point(101, 110);
			this._studyDescription.Name = "_studyDescription";
			this._studyDescription.Size = new System.Drawing.Size(156, 20);
			this._studyDescription.TabIndex = 13;
			// 
			// _studyDescriptionLabel
			// 
			this._studyDescriptionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._studyDescriptionLabel.AutoSize = true;
			this._studyDescriptionLabel.Location = new System.Drawing.Point(3, 114);
			this._studyDescriptionLabel.Name = "_studyDescriptionLabel";
			this._studyDescriptionLabel.Size = new System.Drawing.Size(90, 13);
			this._studyDescriptionLabel.TabIndex = 12;
			this._studyDescriptionLabel.Text = "Study Description";
			// 
			// _clearButton
			// 
			this._clearButton.Location = new System.Drawing.Point(496, 108);
			this._clearButton.Name = "_clearButton";
			this._clearButton.Size = new System.Drawing.Size(75, 22);
			this._clearButton.TabIndex = 19;
			this._clearButton.Text = "Clear";
			this._clearButton.UseVisualStyleBackColor = true;
			this._clearButton.Click += new System.EventHandler(this.OnClearButonClicked);
			// 
			// _searchTodayButton
			// 
			this._searchTodayButton.Location = new System.Drawing.Point(496, 59);
			this._searchTodayButton.Name = "_searchTodayButton";
			this._searchTodayButton.Size = new System.Drawing.Size(75, 22);
			this._searchTodayButton.TabIndex = 18;
			this._searchTodayButton.Text = "Today";
			this._searchTodayButton.UseVisualStyleBackColor = true;
			this._searchTodayButton.Click += new System.EventHandler(this.OnSearchTodayButtonClicked);
			// 
			// _modalityLabel
			// 
			this._modalityLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._modalityLabel.AutoSize = true;
			this._modalityLabel.Location = new System.Drawing.Point(262, 114);
			this._modalityLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this._modalityLabel.Name = "_modalityLabel";
			this._modalityLabel.Size = new System.Drawing.Size(46, 13);
			this._modalityLabel.TabIndex = 14;
			this._modalityLabel.Text = "Modality";
			// 
			// _modalityPicker
			// 
			this._modalityPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._modalityPicker.AutoSize = true;
			this._modalityPicker.Location = new System.Drawing.Point(331, 110);
			this._modalityPicker.Margin = new System.Windows.Forms.Padding(2);
			this._modalityPicker.Name = "_modalityPicker";
			this._modalityPicker.Size = new System.Drawing.Size(160, 20);
			this._modalityPicker.TabIndex = 15;
			// 
			// _titleBar
			// 
			this._titleBar.Dock = System.Windows.Forms.DockStyle.Top;
			this._titleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToDarkBack;
			this._titleBar.Location = new System.Drawing.Point(0, 0);
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Size = new System.Drawing.Size(587, 30);
			this._titleBar.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this._titleBar.TabIndex = 20;
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
			this.Size = new System.Drawing.Size(587, 178);
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private System.Windows.Forms.Label _patientIDLabel;
		private System.Windows.Forms.Label _accessionNumberLabel;
		private System.Windows.Forms.Label _lastNameLabel;
		private System.Windows.Forms.TextBox _lastName;
		private System.Windows.Forms.Label _studyDescriptionLabel;
		private System.Windows.Forms.TextBox _studyDescription;
		private System.Windows.Forms.Label _studyDateFromLabel;
		private System.Windows.Forms.Label _firstNameLabel;
		private System.Windows.Forms.TextBox _patientID;
		private Crownwood.DotNetMagic.Controls.TitleBar _titleBar;
		private System.Windows.Forms.Label _studyDateToLabel;
		private System.Windows.Forms.TextBox _firstName;
		private System.Windows.Forms.TextBox _accessionNumber;
		private ClearCanvas.Controls.WinForms.DateTimeField _studyDateTo;
		private System.Windows.Forms.Button _searchButton;
		private System.Windows.Forms.Button _searchLastWeekButton;
		private System.Windows.Forms.Button _clearButton;
		private System.Windows.Forms.Button _searchTodayButton;
		private System.Windows.Forms.Label _modalityLabel;
		private ClearCanvas.Controls.WinForms.DateTimeField _studyDateFrom;
		private ModalityPicker _modalityPicker;
    }
}
