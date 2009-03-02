#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
			this._patientID = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._accessionNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._patientsName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._studyDateFrom = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._studyDateTo = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._studyDescription = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._searchButton = new System.Windows.Forms.Button();
			this._searchLastWeekButton = new System.Windows.Forms.Button();
			this._clearButton = new System.Windows.Forms.Button();
			this._searchTodayButton = new System.Windows.Forms.Button();
			this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this._modalityPicker = new ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms.ModalityPicker();
			this.SuspendLayout();
			// 
			// _patientID
			// 
			this._patientID.LabelText = "Patient ID";
			this._patientID.Location = new System.Drawing.Point(17, 46);
			this._patientID.Margin = new System.Windows.Forms.Padding(2);
			this._patientID.Mask = "";
			this._patientID.Name = "_patientID";
			this._patientID.PasswordChar = '\0';
			this._patientID.Size = new System.Drawing.Size(156, 41);
			this._patientID.TabIndex = 0;
			this._patientID.ToolTip = null;
			this._patientID.Value = null;
			// 
			// _accessionNumber
			// 
			this._accessionNumber.LabelText = "Accession#";
			this._accessionNumber.Location = new System.Drawing.Point(359, 46);
			this._accessionNumber.Margin = new System.Windows.Forms.Padding(2);
			this._accessionNumber.Mask = "";
			this._accessionNumber.Name = "_accessionNumber";
			this._accessionNumber.PasswordChar = '\0';
			this._accessionNumber.Size = new System.Drawing.Size(156, 41);
			this._accessionNumber.TabIndex = 2;
			this._accessionNumber.ToolTip = null;
			this._accessionNumber.Value = null;
			// 
			// _patientsName
			// 
			this._patientsName.LabelText = "Name";
			this._patientsName.Location = new System.Drawing.Point(188, 46);
			this._patientsName.Margin = new System.Windows.Forms.Padding(2);
			this._patientsName.Mask = "";
			this._patientsName.Name = "_patientsName";
			this._patientsName.PasswordChar = '\0';
			this._patientsName.Size = new System.Drawing.Size(156, 41);
			this._patientsName.TabIndex = 1;
			this._patientsName.ToolTip = null;
			this._patientsName.Value = null;
			// 
			// _studyDateFrom
			// 
			this._studyDateFrom.LabelText = "Study Date (From)";
			this._studyDateFrom.Location = new System.Drawing.Point(17, 95);
			this._studyDateFrom.Margin = new System.Windows.Forms.Padding(2);
			this._studyDateFrom.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._studyDateFrom.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._studyDateFrom.Name = "_studyDateFrom";
			this._studyDateFrom.Nullable = true;
			this._studyDateFrom.Size = new System.Drawing.Size(154, 41);
			this._studyDateFrom.TabIndex = 4;
			this._studyDateFrom.Value = null;
			// 
			// _studyDateTo
			// 
			this._studyDateTo.LabelText = "Study Date (To)";
			this._studyDateTo.Location = new System.Drawing.Point(188, 95);
			this._studyDateTo.Margin = new System.Windows.Forms.Padding(2);
			this._studyDateTo.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._studyDateTo.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._studyDateTo.Name = "_studyDateTo";
			this._studyDateTo.Nullable = true;
			this._studyDateTo.Size = new System.Drawing.Size(155, 41);
			this._studyDateTo.TabIndex = 5;
			this._studyDateTo.Value = null;
			// 
			// _studyDescription
			// 
			this._studyDescription.LabelText = "Study Description";
			this._studyDescription.Location = new System.Drawing.Point(530, 46);
			this._studyDescription.Margin = new System.Windows.Forms.Padding(2);
			this._studyDescription.Mask = "";
			this._studyDescription.Name = "_studyDescription";
			this._studyDescription.PasswordChar = '\0';
			this._studyDescription.Size = new System.Drawing.Size(156, 41);
			this._studyDescription.TabIndex = 3;
			this._studyDescription.ToolTip = null;
			this._studyDescription.Value = null;
			// 
			// _searchButton
			// 
			this._searchButton.Image = ((System.Drawing.Image)(resources.GetObject("_searchButton.Image")));
			this._searchButton.Location = new System.Drawing.Point(710, 63);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(90, 22);
			this._searchButton.TabIndex = 7;
			this._searchButton.Text = "Search";
			this._searchButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this.OnSearchButtonClicked);
			// 
			// _searchLastWeekButton
			// 
			this._searchLastWeekButton.Image = ((System.Drawing.Image)(resources.GetObject("_searchLastWeekButton.Image")));
			this._searchLastWeekButton.Location = new System.Drawing.Point(806, 93);
			this._searchLastWeekButton.Name = "_searchLastWeekButton";
			this._searchLastWeekButton.Size = new System.Drawing.Size(90, 22);
			this._searchLastWeekButton.TabIndex = 10;
			this._searchLastWeekButton.Text = "Last 7 days";
			this._searchLastWeekButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._searchLastWeekButton.UseVisualStyleBackColor = true;
			this._searchLastWeekButton.Click += new System.EventHandler(this.OnSearchLastWeekButtonClick);
			// 
			// _clearButton
			// 
			this._clearButton.Image = ((System.Drawing.Image)(resources.GetObject("_clearButton.Image")));
			this._clearButton.Location = new System.Drawing.Point(710, 93);
			this._clearButton.Name = "_clearButton";
			this._clearButton.Size = new System.Drawing.Size(90, 22);
			this._clearButton.TabIndex = 9;
			this._clearButton.Text = "Clear";
			this._clearButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._clearButton.UseVisualStyleBackColor = true;
			this._clearButton.Click += new System.EventHandler(this.OnClearButonClicked);
			// 
			// _searchTodayButton
			// 
			this._searchTodayButton.Image = ((System.Drawing.Image)(resources.GetObject("_searchTodayButton.Image")));
			this._searchTodayButton.Location = new System.Drawing.Point(806, 63);
			this._searchTodayButton.Name = "_searchTodayButton";
			this._searchTodayButton.Size = new System.Drawing.Size(90, 22);
			this._searchTodayButton.TabIndex = 8;
			this._searchTodayButton.Text = "Today";
			this._searchTodayButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._searchTodayButton.UseVisualStyleBackColor = true;
			this._searchTodayButton.Click += new System.EventHandler(this.OnSearchTodayButtonClicked);
			// 
			// _titleBar
			// 
			this._titleBar.Dock = System.Windows.Forms.DockStyle.Top;
			this._titleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToDarkBack;
			this._titleBar.Location = new System.Drawing.Point(0, 0);
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Size = new System.Drawing.Size(925, 30);
			this._titleBar.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this._titleBar.TabIndex = 20;
			this._titleBar.Text = "Search";
			// 
			// _modalityPicker
			// 
			this._modalityPicker.AutoSize = true;
			this._modalityPicker.LabelText = "Modality";
			this._modalityPicker.Location = new System.Drawing.Point(359, 95);
			this._modalityPicker.Margin = new System.Windows.Forms.Padding(2);
			this._modalityPicker.Name = "_modalityPicker";
			this._modalityPicker.Size = new System.Drawing.Size(156, 41);
			this._modalityPicker.TabIndex = 6;
			// 
			// SearchPanelComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._titleBar);
			this.Controls.Add(this._patientID);
			this.Controls.Add(this._patientsName);
			this.Controls.Add(this._accessionNumber);
			this.Controls.Add(this._studyDateFrom);
			this.Controls.Add(this._studyDateTo);
			this.Controls.Add(this._studyDescription);
			this.Controls.Add(this._modalityPicker);
			this.Controls.Add(this._searchButton);
			this.Controls.Add(this._searchTodayButton);
			this.Controls.Add(this._searchLastWeekButton);
			this.Controls.Add(this._clearButton);
			this.Name = "SearchPanelComponentControl";
			this.Size = new System.Drawing.Size(925, 151);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private Crownwood.DotNetMagic.Controls.TitleBar _titleBar;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _studyDateTo;
		private System.Windows.Forms.Button _searchButton;
		private System.Windows.Forms.Button _searchLastWeekButton;
		private System.Windows.Forms.Button _clearButton;
		private System.Windows.Forms.Button _searchTodayButton;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _studyDateFrom;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientID;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientsName;
		private ClearCanvas.Desktop.View.WinForms.TextField _studyDescription;
		private ClearCanvas.Desktop.View.WinForms.TextField _accessionNumber;
		private ModalityPicker _modalityPicker;
    }
}
