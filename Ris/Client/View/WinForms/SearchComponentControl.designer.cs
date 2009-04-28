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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchComponentControl));
			this._searchField = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._activeOnly = new System.Windows.Forms.CheckBox();
			this._keepOpen = new System.Windows.Forms.CheckBox();
			this._searchButton = new System.Windows.Forms.Button();
			this._accession = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._filterGroupBox = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._patientIdMrn = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._healthcard = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._patientName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._startDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._stopDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._orderingPractitioner = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._procedureType = new ClearCanvas.Desktop.View.WinForms.SuggestComboField();
			this._filterGroupBox.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _searchField
			// 
			this._searchField.LabelText = "Search Criteria:";
			this._searchField.Location = new System.Drawing.Point(0, 2);
			this._searchField.Margin = new System.Windows.Forms.Padding(2);
			this._searchField.Mask = "";
			this._searchField.Name = "_searchField";
			this._searchField.PasswordChar = '\0';
			this._searchField.ReadOnly = true;
			this._searchField.Size = new System.Drawing.Size(409, 41);
			this._searchField.TabIndex = 0;
			this._searchField.ToolTip = "Enter name, mrn, healthcard or accession # as criteria";
			this._searchField.Value = null;
			// 
			// _activeOnly
			// 
			this._activeOnly.AutoSize = true;
			this._activeOnly.Checked = true;
			this._activeOnly.CheckState = System.Windows.Forms.CheckState.Checked;
			this._activeOnly.Location = new System.Drawing.Point(337, 3);
			this._activeOnly.Name = "_activeOnly";
			this._activeOnly.Size = new System.Drawing.Size(80, 17);
			this._activeOnly.TabIndex = 1;
			this._activeOnly.Text = "Active Only";
			this._activeOnly.UseVisualStyleBackColor = true;
			// 
			// _keepOpen
			// 
			this._keepOpen.AutoSize = true;
			this._keepOpen.Location = new System.Drawing.Point(251, 3);
			this._keepOpen.Name = "_keepOpen";
			this._keepOpen.Size = new System.Drawing.Size(80, 17);
			this._keepOpen.TabIndex = 0;
			this._keepOpen.Text = "Keep Open";
			this._keepOpen.UseVisualStyleBackColor = true;
			// 
			// _searchButton
			// 
			this._searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._searchButton.Image = ((System.Drawing.Image)(resources.GetObject("_searchButton.Image")));
			this._searchButton.Location = new System.Drawing.Point(413, 17);
			this._searchButton.Margin = new System.Windows.Forms.Padding(2);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(24, 24);
			this._searchButton.TabIndex = 1;
			this._searchButton.TabStop = false;
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// _accession
			// 
			this._accession.Dock = System.Windows.Forms.DockStyle.Fill;
			this._accession.LabelText = "Accession #";
			this._accession.Location = new System.Drawing.Point(2, 2);
			this._accession.Margin = new System.Windows.Forms.Padding(2);
			this._accession.Mask = "";
			this._accession.Name = "_accession";
			this._accession.PasswordChar = '\0';
			this._accession.Size = new System.Drawing.Size(209, 41);
			this._accession.TabIndex = 0;
			this._accession.ToolTip = null;
			this._accession.Value = null;
			// 
			// _filterGroupBox
			// 
			this._filterGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._filterGroupBox.Controls.Add(this.tableLayoutPanel1);
			this._filterGroupBox.Location = new System.Drawing.Point(4, 49);
			this._filterGroupBox.Name = "_filterGroupBox";
			this._filterGroupBox.Size = new System.Drawing.Size(432, 234);
			this._filterGroupBox.TabIndex = 2;
			this._filterGroupBox.TabStop = false;
			this._filterGroupBox.Text = "Filters";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this._orderingPractitioner, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._patientName, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this._healthcard, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._patientIdMrn, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this._accession, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this._startDate, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this._stopDate, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this._procedureType, 0, 3);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(426, 215);
			this.tableLayoutPanel1.TabIndex = 7;
			// 
			// flowLayoutPanel1
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
			this.flowLayoutPanel1.Controls.Add(this._activeOnly);
			this.flowLayoutPanel1.Controls.Add(this._keepOpen);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 183);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.flowLayoutPanel1.Size = new System.Drawing.Size(420, 29);
			this.flowLayoutPanel1.TabIndex = 8;
			// 
			// _patientIdMrn
			// 
			this._patientIdMrn.Dock = System.Windows.Forms.DockStyle.Fill;
			this._patientIdMrn.LabelText = "Patient ID/MRN";
			this._patientIdMrn.Location = new System.Drawing.Point(215, 2);
			this._patientIdMrn.Margin = new System.Windows.Forms.Padding(2);
			this._patientIdMrn.Mask = "";
			this._patientIdMrn.Name = "_patientIdMrn";
			this._patientIdMrn.PasswordChar = '\0';
			this._patientIdMrn.Size = new System.Drawing.Size(209, 41);
			this._patientIdMrn.TabIndex = 1;
			this._patientIdMrn.ToolTip = null;
			this._patientIdMrn.Value = null;
			// 
			// _healthcard
			// 
			this._healthcard.Dock = System.Windows.Forms.DockStyle.Fill;
			this._healthcard.LabelText = "Healthcard #";
			this._healthcard.Location = new System.Drawing.Point(2, 47);
			this._healthcard.Margin = new System.Windows.Forms.Padding(2);
			this._healthcard.Mask = "";
			this._healthcard.Name = "_healthcard";
			this._healthcard.PasswordChar = '\0';
			this._healthcard.Size = new System.Drawing.Size(209, 41);
			this._healthcard.TabIndex = 2;
			this._healthcard.ToolTip = null;
			this._healthcard.Value = null;
			// 
			// _patientName
			// 
			this._patientName.Dock = System.Windows.Forms.DockStyle.Fill;
			this._patientName.LabelText = "Patient Name";
			this._patientName.Location = new System.Drawing.Point(215, 47);
			this._patientName.Margin = new System.Windows.Forms.Padding(2);
			this._patientName.Mask = "";
			this._patientName.Name = "_patientName";
			this._patientName.PasswordChar = '\0';
			this._patientName.Size = new System.Drawing.Size(209, 41);
			this._patientName.TabIndex = 3;
			this._patientName.ToolTip = null;
			this._patientName.Value = null;
			// 
			// _startDate
			// 
			this._startDate.Dock = System.Windows.Forms.DockStyle.Fill;
			this._startDate.LabelText = "Start Date";
			this._startDate.Location = new System.Drawing.Point(215, 92);
			this._startDate.Margin = new System.Windows.Forms.Padding(2);
			this._startDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._startDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._startDate.Name = "_startDate";
			this._startDate.Nullable = true;
			this._startDate.Size = new System.Drawing.Size(209, 41);
			this._startDate.TabIndex = 5;
			this._startDate.Value = new System.DateTime(2009, 4, 28, 16, 56, 44, 343);
			// 
			// _stopDate
			// 
			this._stopDate.Dock = System.Windows.Forms.DockStyle.Fill;
			this._stopDate.LabelText = "Stop Date";
			this._stopDate.Location = new System.Drawing.Point(215, 137);
			this._stopDate.Margin = new System.Windows.Forms.Padding(2);
			this._stopDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._stopDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._stopDate.Name = "_stopDate";
			this._stopDate.Nullable = true;
			this._stopDate.Size = new System.Drawing.Size(209, 41);
			this._stopDate.TabIndex = 7;
			this._stopDate.Value = new System.DateTime(2009, 4, 28, 16, 56, 47, 203);
			// 
			// _orderingPractitioner
			// 
			this._orderingPractitioner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._orderingPractitioner.LabelText = "Ordered By";
			this._orderingPractitioner.Location = new System.Drawing.Point(0, 90);
			this._orderingPractitioner.Margin = new System.Windows.Forms.Padding(0);
			this._orderingPractitioner.Name = "_orderingPractitioner";
			this._orderingPractitioner.Size = new System.Drawing.Size(213, 41);
			this._orderingPractitioner.TabIndex = 4;
			this._orderingPractitioner.Value = null;
			// 
			// _procedureType
			// 
			this._procedureType.Dock = System.Windows.Forms.DockStyle.Fill;
			this._procedureType.LabelText = "Procedure Type";
			this._procedureType.Location = new System.Drawing.Point(2, 137);
			this._procedureType.Margin = new System.Windows.Forms.Padding(2);
			this._procedureType.Name = "_procedureType";
			this._procedureType.Size = new System.Drawing.Size(209, 41);
			this._procedureType.TabIndex = 6;
			this._procedureType.Value = null;
			// 
			// SearchComponentControl
			// 
			this.AcceptButton = this._searchButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._filterGroupBox);
			this.Controls.Add(this._searchButton);
			this.Controls.Add(this._searchField);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "SearchComponentControl";
			this.Size = new System.Drawing.Size(439, 286);
			this._filterGroupBox.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TextField _searchField;
		private System.Windows.Forms.Button _searchButton;
        private System.Windows.Forms.CheckBox _keepOpen;
		private System.Windows.Forms.CheckBox _activeOnly;
		private ClearCanvas.Desktop.View.WinForms.TextField _accession;
		private System.Windows.Forms.GroupBox _filterGroupBox;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private LookupField _orderingPractitioner;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientName;
		private ClearCanvas.Desktop.View.WinForms.TextField _healthcard;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientIdMrn;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _startDate;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _stopDate;
		private ClearCanvas.Desktop.View.WinForms.SuggestComboField _procedureType;
    }
}
