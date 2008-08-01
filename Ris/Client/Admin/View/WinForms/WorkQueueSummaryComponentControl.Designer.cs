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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class WorkQueueSummaryComponentControl
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
			this.components = new System.ComponentModel.Container();
			this._searchButton = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._diagnosticServiceTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.panel1 = new System.Windows.Forms.Panel();
			this._status = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._type = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._user = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._endTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._startTime = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._clearButton = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _searchButton
			// 
			this._searchButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._searchButton.BackColor = System.Drawing.Color.Transparent;
			this._searchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._searchButton.FlatAppearance.BorderSize = 0;
			this._searchButton.Image = global::ClearCanvas.Ris.Client.Admin.View.WinForms.SR.SearchToolSmall;
			this._searchButton.Location = new System.Drawing.Point(486, 73);
			this._searchButton.Margin = new System.Windows.Forms.Padding(0);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(30, 30);
			this._searchButton.TabIndex = 4;
			this.toolTip1.SetToolTip(this._searchButton, "Search");
			this._searchButton.UseVisualStyleBackColor = false;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._okButton);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 610);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(614, 30);
			this.flowLayoutPanel1.TabIndex = 2;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(537, 2);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(458, 2);
			this._okButton.Margin = new System.Windows.Forms.Padding(2);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 0;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._diagnosticServiceTableView, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(618, 642);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// _diagnosticServiceTableView
			// 
			this._diagnosticServiceTableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._diagnosticServiceTableView.Location = new System.Drawing.Point(4, 129);
			this._diagnosticServiceTableView.Margin = new System.Windows.Forms.Padding(4);
			this._diagnosticServiceTableView.Name = "_diagnosticServiceTableView";
			this._diagnosticServiceTableView.ReadOnly = false;
			this._diagnosticServiceTableView.Size = new System.Drawing.Size(610, 475);
			this._diagnosticServiceTableView.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._status);
			this.panel1.Controls.Add(this._type);
			this.panel1.Controls.Add(this._user);
			this.panel1.Controls.Add(this._endTime);
			this.panel1.Controls.Add(this._startTime);
			this.panel1.Controls.Add(this._clearButton);
			this.panel1.Controls.Add(this._searchButton);
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(612, 119);
			this.panel1.TabIndex = 0;
			// 
			// _status
			// 
			this._status.DataSource = null;
			this._status.DisplayMember = "";
			this._status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._status.LabelText = "Status";
			this._status.Location = new System.Drawing.Point(323, 62);
			this._status.Margin = new System.Windows.Forms.Padding(2);
			this._status.Name = "_status";
			this._status.Size = new System.Drawing.Size(150, 41);
			this._status.TabIndex = 10;
			this._status.Value = null;
			// 
			// _type
			// 
			this._type.DataSource = null;
			this._type.DisplayMember = "";
			this._type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._type.LabelText = "Type";
			this._type.Location = new System.Drawing.Point(169, 62);
			this._type.Margin = new System.Windows.Forms.Padding(2);
			this._type.Name = "_type";
			this._type.Size = new System.Drawing.Size(150, 41);
			this._type.TabIndex = 9;
			this._type.Value = null;
			// 
			// _user
			// 
			this._user.LabelText = "User";
			this._user.Location = new System.Drawing.Point(15, 62);
			this._user.Margin = new System.Windows.Forms.Padding(2);
			this._user.Mask = "";
			this._user.Name = "_user";
			this._user.PasswordChar = '\0';
			this._user.Size = new System.Drawing.Size(150, 41);
			this._user.TabIndex = 8;
			this._user.ToolTip = null;
			this._user.Value = null;
			// 
			// _endTime
			// 
			this._endTime.LabelText = "End";
			this._endTime.Location = new System.Drawing.Point(169, 17);
			this._endTime.Margin = new System.Windows.Forms.Padding(2);
			this._endTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._endTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._endTime.Name = "_endTime";
			this._endTime.Nullable = true;
			this._endTime.Size = new System.Drawing.Size(150, 41);
			this._endTime.TabIndex = 7;
			this._endTime.Value = new System.DateTime(2008, 7, 31, 16, 3, 53, 875);
			// 
			// _startTime
			// 
			this._startTime.LabelText = "Start";
			this._startTime.Location = new System.Drawing.Point(15, 17);
			this._startTime.Margin = new System.Windows.Forms.Padding(2);
			this._startTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._startTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._startTime.Name = "_startTime";
			this._startTime.Nullable = true;
			this._startTime.Size = new System.Drawing.Size(150, 41);
			this._startTime.TabIndex = 6;
			this._startTime.Value = new System.DateTime(2008, 7, 31, 16, 3, 48, 687);
			// 
			// _clearButton
			// 
			this._clearButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._clearButton.BackColor = System.Drawing.Color.Transparent;
			this._clearButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._clearButton.FlatAppearance.BorderSize = 0;
			this._clearButton.Image = global::ClearCanvas.Ris.Client.Admin.View.WinForms.SR.ClearFilterSmall;
			this._clearButton.Location = new System.Drawing.Point(516, 73);
			this._clearButton.Margin = new System.Windows.Forms.Padding(0);
			this._clearButton.Name = "_clearButton";
			this._clearButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._clearButton.Size = new System.Drawing.Size(30, 30);
			this._clearButton.TabIndex = 5;
			this._clearButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.toolTip1.SetToolTip(this._clearButton, "Clear search query");
			this._clearButton.UseVisualStyleBackColor = false;
			this._clearButton.Click += new System.EventHandler(this._clearButton_Click);
			// 
			// WorkQueueSummaryComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "WorkQueueSummaryComponentControl";
			this.Size = new System.Drawing.Size(618, 642);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button _searchButton;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private ClearCanvas.Desktop.View.WinForms.TableView _diagnosticServiceTableView;
		private System.Windows.Forms.Panel panel1;
		private ClearCanvas.Desktop.View.WinForms.TextField _user;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _endTime;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _startTime;
		private System.Windows.Forms.Button _clearButton;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _status;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _type;
    }
}
