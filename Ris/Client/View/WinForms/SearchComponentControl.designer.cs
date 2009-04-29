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
			this._accession = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._filterGroupBox = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._mrn = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._healthcard = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._familyName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._givenName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._orderingPractitioner = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._fromDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._untilDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._keepOpen = new System.Windows.Forms.CheckBox();
			this._searchButton = new System.Windows.Forms.Button();
			this._procedureType = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._filterGroupBox.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _accession
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._accession, 2);
			this._accession.LabelText = "Accession #";
			this._accession.Location = new System.Drawing.Point(2, 2);
			this._accession.Margin = new System.Windows.Forms.Padding(2);
			this._accession.Mask = "";
			this._accession.Name = "_accession";
			this._accession.PasswordChar = '\0';
			this._accession.Size = new System.Drawing.Size(136, 41);
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
			this._filterGroupBox.Location = new System.Drawing.Point(4, 3);
			this._filterGroupBox.Name = "_filterGroupBox";
			this._filterGroupBox.Size = new System.Drawing.Size(432, 205);
			this._filterGroupBox.TabIndex = 0;
			this._filterGroupBox.TabStop = false;
			this._filterGroupBox.Text = "Filters";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 6;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this.tableLayoutPanel1.Controls.Add(this._accession, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._mrn, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this._healthcard, 4, 0);
			this.tableLayoutPanel1.Controls.Add(this._familyName, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._givenName, 3, 1);
			this.tableLayoutPanel1.Controls.Add(this._orderingPractitioner, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._fromDate, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this._untilDate, 3, 3);
			this.tableLayoutPanel1.Controls.Add(this._procedureType, 3, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(426, 186);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _mrn
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._mrn, 2);
			this._mrn.LabelText = "MRN";
			this._mrn.Location = new System.Drawing.Point(142, 2);
			this._mrn.Margin = new System.Windows.Forms.Padding(2);
			this._mrn.Mask = "";
			this._mrn.Name = "_mrn";
			this._mrn.PasswordChar = '\0';
			this._mrn.Size = new System.Drawing.Size(136, 41);
			this._mrn.TabIndex = 1;
			this._mrn.ToolTip = null;
			this._mrn.Value = null;
			// 
			// _healthcard
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._healthcard, 2);
			this._healthcard.LabelText = "Healthcard #";
			this._healthcard.Location = new System.Drawing.Point(282, 2);
			this._healthcard.Margin = new System.Windows.Forms.Padding(2);
			this._healthcard.Mask = "";
			this._healthcard.Name = "_healthcard";
			this._healthcard.PasswordChar = '\0';
			this._healthcard.Size = new System.Drawing.Size(138, 41);
			this._healthcard.TabIndex = 2;
			this._healthcard.ToolTip = null;
			this._healthcard.Value = null;
			// 
			// _familyName
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._familyName, 3);
			this._familyName.LabelText = "Family Name";
			this._familyName.Location = new System.Drawing.Point(2, 48);
			this._familyName.Margin = new System.Windows.Forms.Padding(2);
			this._familyName.Mask = "";
			this._familyName.Name = "_familyName";
			this._familyName.PasswordChar = '\0';
			this._familyName.Size = new System.Drawing.Size(206, 41);
			this._familyName.TabIndex = 3;
			this._familyName.ToolTip = null;
			this._familyName.Value = null;
			// 
			// _givenName
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._givenName, 3);
			this._givenName.LabelText = "Given Name";
			this._givenName.Location = new System.Drawing.Point(212, 48);
			this._givenName.Margin = new System.Windows.Forms.Padding(2);
			this._givenName.Mask = "";
			this._givenName.Name = "_givenName";
			this._givenName.PasswordChar = '\0';
			this._givenName.Size = new System.Drawing.Size(209, 41);
			this._givenName.TabIndex = 4;
			this._givenName.ToolTip = null;
			this._givenName.Value = null;
			// 
			// _orderingPractitioner
			// 
			this._orderingPractitioner.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.SetColumnSpan(this._orderingPractitioner, 3);
			this._orderingPractitioner.LabelText = "Ordered By";
			this._orderingPractitioner.Location = new System.Drawing.Point(2, 94);
			this._orderingPractitioner.Margin = new System.Windows.Forms.Padding(2);
			this._orderingPractitioner.Name = "_orderingPractitioner";
			this._orderingPractitioner.Size = new System.Drawing.Size(206, 42);
			this._orderingPractitioner.TabIndex = 5;
			this._orderingPractitioner.Value = null;
			// 
			// _fromDate
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._fromDate, 3);
			this._fromDate.LabelText = "From Date";
			this._fromDate.Location = new System.Drawing.Point(2, 140);
			this._fromDate.Margin = new System.Windows.Forms.Padding(2);
			this._fromDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._fromDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._fromDate.Name = "_fromDate";
			this._fromDate.Nullable = true;
			this._fromDate.Size = new System.Drawing.Size(206, 41);
			this._fromDate.TabIndex = 7;
			this._fromDate.Value = new System.DateTime(2009, 4, 28, 16, 56, 44, 343);
			// 
			// _untilDate
			// 
			this.tableLayoutPanel1.SetColumnSpan(this._untilDate, 3);
			this._untilDate.LabelText = "Until Date";
			this._untilDate.Location = new System.Drawing.Point(212, 140);
			this._untilDate.Margin = new System.Windows.Forms.Padding(2);
			this._untilDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._untilDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._untilDate.Name = "_untilDate";
			this._untilDate.Nullable = true;
			this._untilDate.Size = new System.Drawing.Size(209, 44);
			this._untilDate.TabIndex = 8;
			this._untilDate.Value = new System.DateTime(2009, 4, 28, 16, 56, 47, 203);
			// 
			// _keepOpen
			// 
			this._keepOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._keepOpen.AutoSize = true;
			this._keepOpen.Location = new System.Drawing.Point(265, 221);
			this._keepOpen.Name = "_keepOpen";
			this._keepOpen.Size = new System.Drawing.Size(80, 17);
			this._keepOpen.TabIndex = 1;
			this._keepOpen.Text = "Keep Open";
			this._keepOpen.UseVisualStyleBackColor = true;
			// 
			// _searchButton
			// 
			this._searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._searchButton.Location = new System.Drawing.Point(350, 217);
			this._searchButton.Margin = new System.Windows.Forms.Padding(2);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(81, 24);
			this._searchButton.TabIndex = 2;
			this._searchButton.TabStop = false;
			this._searchButton.Text = "Search";
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// _procedureType
			// 
			this._procedureType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.SetColumnSpan(this._procedureType, 3);
			this._procedureType.LabelText = "ProcedureType";
			this._procedureType.Location = new System.Drawing.Point(212, 94);
			this._procedureType.Margin = new System.Windows.Forms.Padding(2);
			this._procedureType.Name = "_procedureType";
			this._procedureType.Size = new System.Drawing.Size(212, 42);
			this._procedureType.TabIndex = 6;
			this._procedureType.Value = null;
			// 
			// SearchComponentControl
			// 
			this.AcceptButton = this._searchButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._searchButton);
			this.Controls.Add(this._filterGroupBox);
			this.Controls.Add(this._keepOpen);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "SearchComponentControl";
			this.Size = new System.Drawing.Size(439, 246);
			this._filterGroupBox.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TextField _accession;
		private System.Windows.Forms.GroupBox _filterGroupBox;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private LookupField _orderingPractitioner;
		private ClearCanvas.Desktop.View.WinForms.TextField _familyName;
		private ClearCanvas.Desktop.View.WinForms.TextField _healthcard;
		private ClearCanvas.Desktop.View.WinForms.TextField _mrn;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _fromDate;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _untilDate;
		private System.Windows.Forms.CheckBox _keepOpen;
		private System.Windows.Forms.Button _searchButton;
		private ClearCanvas.Desktop.View.WinForms.TextField _givenName;
		private LookupField _procedureType;
    }
}
