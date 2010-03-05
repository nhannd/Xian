#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails.View.WinForms
{
    partial class SeriesDetailsComponentControl
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._accessionNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._dob = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._studyDescription = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._studyDate = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._patientId = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._patientsName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._seriesTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._close = new System.Windows.Forms.Button();
			this._refresh = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._accessionNumber);
			this.splitContainer1.Panel1.Controls.Add(this._dob);
			this.splitContainer1.Panel1.Controls.Add(this._studyDescription);
			this.splitContainer1.Panel1.Controls.Add(this._studyDate);
			this.splitContainer1.Panel1.Controls.Add(this._patientId);
			this.splitContainer1.Panel1.Controls.Add(this._patientsName);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._seriesTable);
			this.splitContainer1.Size = new System.Drawing.Size(457, 374);
			this.splitContainer1.SplitterDistance = 108;
			this.splitContainer1.TabIndex = 3;
			// 
			// _accessionNumber
			// 
			this._accessionNumber.LabelText = "Accesion Number";
			this._accessionNumber.Location = new System.Drawing.Point(12, 58);
			this._accessionNumber.Margin = new System.Windows.Forms.Padding(2);
			this._accessionNumber.Mask = "";
			this._accessionNumber.Name = "_accessionNumber";
			this._accessionNumber.PasswordChar = '\0';
			this._accessionNumber.ReadOnly = true;
			this._accessionNumber.Size = new System.Drawing.Size(115, 41);
			this._accessionNumber.TabIndex = 3;
			this._accessionNumber.ToolTip = null;
			this._accessionNumber.Value = null;
			// 
			// _dob
			// 
			this._dob.LabelText = "DOB";
			this._dob.Location = new System.Drawing.Point(331, 13);
			this._dob.Margin = new System.Windows.Forms.Padding(2);
			this._dob.Mask = "";
			this._dob.Name = "_dob";
			this._dob.PasswordChar = '\0';
			this._dob.ReadOnly = true;
			this._dob.Size = new System.Drawing.Size(115, 41);
			this._dob.TabIndex = 2;
			this._dob.ToolTip = null;
			this._dob.Value = null;
			// 
			// _studyDescription
			// 
			this._studyDescription.LabelText = "Study Description";
			this._studyDescription.Location = new System.Drawing.Point(131, 58);
			this._studyDescription.Margin = new System.Windows.Forms.Padding(2);
			this._studyDescription.Mask = "";
			this._studyDescription.Name = "_studyDescription";
			this._studyDescription.PasswordChar = '\0';
			this._studyDescription.ReadOnly = true;
			this._studyDescription.Size = new System.Drawing.Size(196, 41);
			this._studyDescription.TabIndex = 4;
			this._studyDescription.ToolTip = null;
			this._studyDescription.Value = null;
			// 
			// _studyDate
			// 
			this._studyDate.LabelText = "Study Date";
			this._studyDate.Location = new System.Drawing.Point(331, 58);
			this._studyDate.Margin = new System.Windows.Forms.Padding(2);
			this._studyDate.Mask = "";
			this._studyDate.Name = "_studyDate";
			this._studyDate.PasswordChar = '\0';
			this._studyDate.ReadOnly = true;
			this._studyDate.Size = new System.Drawing.Size(115, 41);
			this._studyDate.TabIndex = 5;
			this._studyDate.ToolTip = null;
			this._studyDate.Value = null;
			// 
			// _patientId
			// 
			this._patientId.LabelText = "Patient ID";
			this._patientId.Location = new System.Drawing.Point(12, 13);
			this._patientId.Margin = new System.Windows.Forms.Padding(2);
			this._patientId.Mask = "";
			this._patientId.Name = "_patientId";
			this._patientId.PasswordChar = '\0';
			this._patientId.ReadOnly = true;
			this._patientId.Size = new System.Drawing.Size(115, 41);
			this._patientId.TabIndex = 0;
			this._patientId.ToolTip = null;
			this._patientId.Value = null;
			// 
			// _patientsName
			// 
			this._patientsName.LabelText = "Name";
			this._patientsName.Location = new System.Drawing.Point(131, 13);
			this._patientsName.Margin = new System.Windows.Forms.Padding(2);
			this._patientsName.Mask = "";
			this._patientsName.Name = "_patientsName";
			this._patientsName.PasswordChar = '\0';
			this._patientsName.ReadOnly = true;
			this._patientsName.Size = new System.Drawing.Size(196, 41);
			this._patientsName.TabIndex = 1;
			this._patientsName.ToolTip = null;
			this._patientsName.Value = null;
			// 
			// _seriesTable
			// 
			this._seriesTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._seriesTable.Location = new System.Drawing.Point(0, 0);
			this._seriesTable.Name = "_seriesTable";
			this._seriesTable.ReadOnly = false;
			this._seriesTable.ShowToolbar = false;
			this._seriesTable.Size = new System.Drawing.Size(457, 262);
			this._seriesTable.TabIndex = 7;
			// 
			// _close
			// 
			this._close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._close.Location = new System.Drawing.Point(371, 380);
			this._close.Name = "_close";
			this._close.Size = new System.Drawing.Size(75, 23);
			this._close.TabIndex = 4;
			this._close.Text = "Close";
			this._close.UseVisualStyleBackColor = true;
			this._close.Click += new System.EventHandler(this._close_Click);
			// 
			// _refresh
			// 
			this._refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._refresh.Location = new System.Drawing.Point(290, 380);
			this._refresh.Name = "_refresh";
			this._refresh.Size = new System.Drawing.Size(75, 23);
			this._refresh.TabIndex = 5;
			this._refresh.Text = "Refresh";
			this._refresh.UseVisualStyleBackColor = true;
			this._refresh.Click += new System.EventHandler(this._refresh_Click);
			// 
			// SeriesDetailsComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._refresh);
			this.Controls.Add(this._close);
			this.Controls.Add(this.splitContainer1);
			this.Name = "SeriesDetailsComponentControl";
			this.Size = new System.Drawing.Size(457, 413);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private ClearCanvas.Desktop.View.WinForms.TextField _studyDescription;
		private ClearCanvas.Desktop.View.WinForms.TextField _studyDate;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientId;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientsName;
		private ClearCanvas.Desktop.View.WinForms.TableView _seriesTable;
		private ClearCanvas.Desktop.View.WinForms.TextField _dob;
		private ClearCanvas.Desktop.View.WinForms.TextField _accessionNumber;
		private System.Windows.Forms.Button _close;
		private System.Windows.Forms.Button _refresh;


	}
}
