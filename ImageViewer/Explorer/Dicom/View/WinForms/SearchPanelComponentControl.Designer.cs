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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchPanelComponentControl));
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
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
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
			this._tableLayoutPanel.Location = new System.Drawing.Point(8, 44);
			this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 4;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.Size = new System.Drawing.Size(761, 167);
			this._tableLayoutPanel.TabIndex = 2;
			// 
			// _searchButton
			// 
			this._searchButton.Location = new System.Drawing.Point(655, 4);
			this._searchButton.Margin = new System.Windows.Forms.Padding(4);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(100, 27);
			this._searchButton.TabIndex = 16;
			this._searchButton.Text = "Search";
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this.OnSearchButtonClicked);
			// 
			// _searchLastWeekButton
			// 
			this._searchLastWeekButton.Location = new System.Drawing.Point(655, 39);
			this._searchLastWeekButton.Margin = new System.Windows.Forms.Padding(4);
			this._searchLastWeekButton.Name = "_searchLastWeekButton";
			this._searchLastWeekButton.Size = new System.Drawing.Size(100, 27);
			this._searchLastWeekButton.TabIndex = 17;
			this._searchLastWeekButton.Text = "Last 7 days";
			this._searchLastWeekButton.UseVisualStyleBackColor = true;
			this._searchLastWeekButton.Click += new System.EventHandler(this.OnSearchLastWeekButtonClick);
			// 
			// _patientIDLabel
			// 
			this._patientIDLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._patientIDLabel.AutoSize = true;
			this._patientIDLabel.Location = new System.Drawing.Point(4, 9);
			this._patientIDLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._patientIDLabel.Name = "_patientIDLabel";
			this._patientIDLabel.Size = new System.Drawing.Size(69, 17);
			this._patientIDLabel.TabIndex = 0;
			this._patientIDLabel.Text = "Patient ID";
			// 
			// _accessionNumberLabel
			// 
			this._accessionNumberLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._accessionNumberLabel.AutoSize = true;
			this._accessionNumberLabel.Location = new System.Drawing.Point(350, 9);
			this._accessionNumberLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._accessionNumberLabel.Name = "_accessionNumberLabel";
			this._accessionNumberLabel.Size = new System.Drawing.Size(80, 17);
			this._accessionNumberLabel.TabIndex = 2;
			this._accessionNumberLabel.Text = "Accession#";
			// 
			// _lastNameLabel
			// 
			this._lastNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._lastNameLabel.AutoSize = true;
			this._lastNameLabel.Location = new System.Drawing.Point(4, 44);
			this._lastNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._lastNameLabel.Name = "_lastNameLabel";
			this._lastNameLabel.Size = new System.Drawing.Size(76, 17);
			this._lastNameLabel.TabIndex = 4;
			this._lastNameLabel.Text = "Last Name";
			// 
			// _lastName
			// 
			this._lastName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._lastName.Location = new System.Drawing.Point(136, 41);
			this._lastName.Margin = new System.Windows.Forms.Padding(4);
			this._lastName.Name = "_lastName";
			this._lastName.Size = new System.Drawing.Size(206, 22);
			this._lastName.TabIndex = 5;
			// 
			// _studyDateFromLabel
			// 
			this._studyDateFromLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._studyDateFromLabel.AutoSize = true;
			this._studyDateFromLabel.Location = new System.Drawing.Point(4, 91);
			this._studyDateFromLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._studyDateFromLabel.Name = "_studyDateFromLabel";
			this._studyDateFromLabel.Size = new System.Drawing.Size(124, 17);
			this._studyDateFromLabel.TabIndex = 8;
			this._studyDateFromLabel.Text = "Study Date (From)";
			// 
			// _firstNameLabel
			// 
			this._firstNameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._firstNameLabel.AutoSize = true;
			this._firstNameLabel.Location = new System.Drawing.Point(350, 44);
			this._firstNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._firstNameLabel.Name = "_firstNameLabel";
			this._firstNameLabel.Size = new System.Drawing.Size(76, 17);
			this._firstNameLabel.TabIndex = 6;
			this._firstNameLabel.Text = "First Name";
			// 
			// _patientID
			// 
			this._patientID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._patientID.Location = new System.Drawing.Point(136, 6);
			this._patientID.Margin = new System.Windows.Forms.Padding(4);
			this._patientID.Name = "_patientID";
			this._patientID.Size = new System.Drawing.Size(206, 22);
			this._patientID.TabIndex = 1;
			// 
			// _firstName
			// 
			this._firstName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._firstName.Location = new System.Drawing.Point(438, 41);
			this._firstName.Margin = new System.Windows.Forms.Padding(4);
			this._firstName.Name = "_firstName";
			this._firstName.Size = new System.Drawing.Size(209, 22);
			this._firstName.TabIndex = 7;
			// 
			// _accessionNumber
			// 
			this._accessionNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._accessionNumber.Location = new System.Drawing.Point(438, 6);
			this._accessionNumber.Margin = new System.Windows.Forms.Padding(4);
			this._accessionNumber.Name = "_accessionNumber";
			this._accessionNumber.Size = new System.Drawing.Size(209, 22);
			this._accessionNumber.TabIndex = 3;
			// 
			// _studyDateToLabel
			// 
			this._studyDateToLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._studyDateToLabel.Location = new System.Drawing.Point(350, 91);
			this._studyDateToLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._studyDateToLabel.Name = "_studyDateToLabel";
			this._studyDateToLabel.Size = new System.Drawing.Size(78, 17);
			this._studyDateToLabel.TabIndex = 10;
			this._studyDateToLabel.Text = "(To)";
			this._studyDateToLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// _studyDateTo
			// 
			this._studyDateTo.LabelText = "";
			this._studyDateTo.Location = new System.Drawing.Point(437, 72);
			this._studyDateTo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this._studyDateTo.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._studyDateTo.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._studyDateTo.Name = "_studyDateTo";
			this._studyDateTo.Nullable = true;
			this._studyDateTo.ShowTime = false;
			this._studyDateTo.Size = new System.Drawing.Size(211, 55);
			this._studyDateTo.TabIndex = 11;
			this._studyDateTo.Value = null;
			// 
			// _studyDateFrom
			// 
			this._studyDateFrom.LabelText = "";
			this._studyDateFrom.Location = new System.Drawing.Point(135, 72);
			this._studyDateFrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this._studyDateFrom.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._studyDateFrom.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._studyDateFrom.Name = "_studyDateFrom";
			this._studyDateFrom.Nullable = true;
			this._studyDateFrom.ShowTime = false;
			this._studyDateFrom.Size = new System.Drawing.Size(208, 55);
			this._studyDateFrom.TabIndex = 9;
			this._studyDateFrom.Value = null;
			// 
			// _studyDescription
			// 
			this._studyDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._studyDescription.Location = new System.Drawing.Point(136, 137);
			this._studyDescription.Margin = new System.Windows.Forms.Padding(4);
			this._studyDescription.Name = "_studyDescription";
			this._studyDescription.Size = new System.Drawing.Size(206, 22);
			this._studyDescription.TabIndex = 13;
			// 
			// _studyDescriptionLabel
			// 
			this._studyDescriptionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._studyDescriptionLabel.AutoSize = true;
			this._studyDescriptionLabel.Location = new System.Drawing.Point(4, 139);
			this._studyDescriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._studyDescriptionLabel.Name = "_studyDescriptionLabel";
			this._studyDescriptionLabel.Size = new System.Drawing.Size(119, 17);
			this._studyDescriptionLabel.TabIndex = 12;
			this._studyDescriptionLabel.Text = "Study Description";
			// 
			// _clearButton
			// 
			this._clearButton.Location = new System.Drawing.Point(655, 133);
			this._clearButton.Margin = new System.Windows.Forms.Padding(4);
			this._clearButton.Name = "_clearButton";
			this._clearButton.Size = new System.Drawing.Size(100, 27);
			this._clearButton.TabIndex = 19;
			this._clearButton.Text = "Clear";
			this._clearButton.UseVisualStyleBackColor = true;
			this._clearButton.Click += new System.EventHandler(this.OnClearButonClicked);
			// 
			// _searchTodayButton
			// 
			this._searchTodayButton.Location = new System.Drawing.Point(654, 72);
			this._searchTodayButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this._searchTodayButton.Name = "_searchTodayButton";
			this._searchTodayButton.Size = new System.Drawing.Size(100, 27);
			this._searchTodayButton.TabIndex = 18;
			this._searchTodayButton.Text = "Today";
			this._searchTodayButton.UseVisualStyleBackColor = true;
			this._searchTodayButton.Click += new System.EventHandler(this.OnSearchTodayButtonClicked);
			// 
			// _modalityLabel
			// 
			this._modalityLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._modalityLabel.AutoSize = true;
			this._modalityLabel.Location = new System.Drawing.Point(349, 139);
			this._modalityLabel.Name = "_modalityLabel";
			this._modalityLabel.Size = new System.Drawing.Size(60, 17);
			this._modalityLabel.TabIndex = 14;
			this._modalityLabel.Text = "Modality";
			// 
			// _modalityPicker
			// 
			this._modalityPicker.AutoSize = true;
			this._modalityPicker.Location = new System.Drawing.Point(437, 132);
			this._modalityPicker.Name = "_modalityPicker";
			this._modalityPicker.Size = new System.Drawing.Size(210, 28);
			this._modalityPicker.TabIndex = 15;
			// 
			// _titleBar
			// 
			this._titleBar.Dock = System.Windows.Forms.DockStyle.Top;
			this._titleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._titleBar.Location = new System.Drawing.Point(0, 0);
			this._titleBar.Margin = new System.Windows.Forms.Padding(4);
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Size = new System.Drawing.Size(778, 36);
			this._titleBar.Style = Crownwood.DotNetMagic.Common.VisualStyle.Office2007Black;
			this._titleBar.TabIndex = 20;
			this._titleBar.Text = "Search";
			// 
			// SearchPanelComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._titleBar);
			this.Controls.Add(this._tableLayoutPanel);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "SearchPanelComponentControl";
			this.Size = new System.Drawing.Size(778, 219);
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
