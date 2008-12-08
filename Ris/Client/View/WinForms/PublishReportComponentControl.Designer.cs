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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class PublishReportComponentControl
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
			this._recipientsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._consultantContactPoint = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._addConsultantButton = new System.Windows.Forms.Button();
			this._groupBoxRecipients = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._consultantLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._printButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._browserPanel = new System.Windows.Forms.Panel();
			this._publishButton = new System.Windows.Forms.Button();
			this._groupBoxRecipients.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _recipientsTableView
			// 
			this._recipientsTableView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this._recipientsTableView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._recipientsTableView.FilterTextBoxWidth = 132;
			this._recipientsTableView.Location = new System.Drawing.Point(7, 20);
			this._recipientsTableView.Margin = new System.Windows.Forms.Padding(4);
			this._recipientsTableView.MultiSelect = false;
			this._recipientsTableView.Name = "_recipientsTableView";
			this._recipientsTableView.ReadOnly = false;
			this._recipientsTableView.ShowToolbar = false;
			this._recipientsTableView.Size = new System.Drawing.Size(410, 136);
			this._recipientsTableView.TabIndex = 0;
			// 
			// _consultantContactPoint
			// 
			this._consultantContactPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._consultantContactPoint.DataSource = null;
			this._consultantContactPoint.DisplayMember = "";
			this._consultantContactPoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._consultantContactPoint.LabelText = "Practitioner Contact Point";
			this._consultantContactPoint.Location = new System.Drawing.Point(5, 63);
			this._consultantContactPoint.Margin = new System.Windows.Forms.Padding(2);
			this._consultantContactPoint.Name = "_consultantContactPoint";
			this._consultantContactPoint.Size = new System.Drawing.Size(258, 41);
			this._consultantContactPoint.TabIndex = 1;
			this._consultantContactPoint.Value = null;
			// 
			// _addConsultantButton
			// 
			this._addConsultantButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this._addConsultantButton.Location = new System.Drawing.Point(188, 108);
			this._addConsultantButton.Margin = new System.Windows.Forms.Padding(2);
			this._addConsultantButton.Name = "_addConsultantButton";
			this._addConsultantButton.Size = new System.Drawing.Size(75, 23);
			this._addConsultantButton.TabIndex = 2;
			this._addConsultantButton.Text = "Add";
			this._addConsultantButton.UseVisualStyleBackColor = true;
			this._addConsultantButton.Click += new System.EventHandler(this._addConsultantButton_Click);
			// 
			// _groupBoxRecipients
			// 
			this._groupBoxRecipients.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._groupBoxRecipients.Controls.Add(this.groupBox2);
			this._groupBoxRecipients.Controls.Add(this._recipientsTableView);
			this._groupBoxRecipients.Location = new System.Drawing.Point(0, 3);
			this._groupBoxRecipients.Name = "_groupBoxRecipients";
			this._groupBoxRecipients.Size = new System.Drawing.Size(698, 163);
			this._groupBoxRecipients.TabIndex = 0;
			this._groupBoxRecipients.TabStop = false;
			this._groupBoxRecipients.Text = "Select Recipients";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox2.Controls.Add(this._consultantLookup);
			this.groupBox2.Controls.Add(this._addConsultantButton);
			this.groupBox2.Controls.Add(this._consultantContactPoint);
			this.groupBox2.Location = new System.Drawing.Point(424, 14);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(268, 142);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Add New Recipient";
			// 
			// _consultantLookup
			// 
			this._consultantLookup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._consultantLookup.LabelText = "Find Practitioner";
			this._consultantLookup.Location = new System.Drawing.Point(5, 18);
			this._consultantLookup.Margin = new System.Windows.Forms.Padding(2);
			this._consultantLookup.Name = "_consultantLookup";
			this._consultantLookup.Size = new System.Drawing.Size(258, 41);
			this._consultantLookup.TabIndex = 0;
			this._consultantLookup.Value = null;
			// 
			// _printButton
			// 
			this._printButton.Location = new System.Drawing.Point(542, 684);
			this._printButton.Name = "_printButton";
			this._printButton.Size = new System.Drawing.Size(75, 23);
			this._printButton.TabIndex = 3;
			this._printButton.Text = "Print";
			this._printButton.UseVisualStyleBackColor = true;
			this._printButton.Click += new System.EventHandler(this._printButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(623, 684);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 4;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _browserPanel
			// 
			this._browserPanel.Location = new System.Drawing.Point(0, 172);
			this._browserPanel.Name = "_browserPanel";
			this._browserPanel.Size = new System.Drawing.Size(701, 506);
			this._browserPanel.TabIndex = 1;
			// 
			// _publishButton
			// 
			this._publishButton.Location = new System.Drawing.Point(461, 684);
			this._publishButton.Name = "_publishButton";
			this._publishButton.Size = new System.Drawing.Size(75, 23);
			this._publishButton.TabIndex = 2;
			this._publishButton.Text = "Fax Queue";
			this._publishButton.UseVisualStyleBackColor = true;
			this._publishButton.Click += new System.EventHandler(this._publishButton_Click);
			// 
			// PublishReportComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._publishButton);
			this.Controls.Add(this._browserPanel);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._printButton);
			this.Controls.Add(this._groupBoxRecipients);
			this.Name = "PublishReportComponentControl";
			this.Size = new System.Drawing.Size(701, 712);
			this._groupBoxRecipients.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _recipientsTableView;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _consultantContactPoint;
		private LookupField _consultantLookup;
		private System.Windows.Forms.Button _addConsultantButton;
		private System.Windows.Forms.GroupBox _groupBoxRecipients;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button _printButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Panel _browserPanel;
		private System.Windows.Forms.Button _publishButton;
    }
}
