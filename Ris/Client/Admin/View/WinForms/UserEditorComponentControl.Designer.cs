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
    partial class UserEditorComponentControl
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
            ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._authorityGroups = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._userId = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this._password = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._confirmPassword = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._validFrom = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._validUntil = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._setStaffButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._clearStaffButton = new System.Windows.Forms.Button();
            this._staffName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this._authorityGroups, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._userId, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this._password, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._confirmPassword, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._validFrom, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this._validUntil, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(687, 505);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _authorityGroups
            // 
            this._authorityGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._authorityGroups, 3);
            this._authorityGroups.Location = new System.Drawing.Point(5, 167);
            this._authorityGroups.Margin = new System.Windows.Forms.Padding(5);
            this._authorityGroups.MenuModel = null;
            this._authorityGroups.Name = "_authorityGroups";
            this._authorityGroups.ReadOnly = false;
            this._authorityGroups.Selection = selection2;
            this._authorityGroups.Size = new System.Drawing.Size(677, 289);
            this._authorityGroups.TabIndex = 0;
            this._authorityGroups.Table = null;
            this._authorityGroups.ToolbarModel = null;
            this._authorityGroups.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._authorityGroups.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _userId
            // 
            this._userId.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._userId.LabelText = "User ID";
            this._userId.Location = new System.Drawing.Point(3, 2);
            this._userId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._userId.Mask = "";
            this._userId.Name = "_userId";
            this._userId.Size = new System.Drawing.Size(222, 50);
            this._userId.TabIndex = 1;
            this._userId.Value = null;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 3);
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._acceptButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 465);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(679, 36);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(575, 4);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(4);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(100, 28);
            this._cancelButton.TabIndex = 0;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(467, 4);
            this._acceptButton.Margin = new System.Windows.Forms.Padding(4);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(100, 28);
            this._acceptButton.TabIndex = 1;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // _password
            // 
            this._password.LabelText = "Password";
            this._password.Location = new System.Drawing.Point(3, 56);
            this._password.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._password.Mask = "";
            this._password.Name = "_password";
            this._password.Size = new System.Drawing.Size(200, 50);
            this._password.TabIndex = 10;
            this._password.Value = null;
            // 
            // _confirmPassword
            // 
            this._confirmPassword.LabelText = "Confirm Password";
            this._confirmPassword.Location = new System.Drawing.Point(3, 110);
            this._confirmPassword.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._confirmPassword.Mask = "";
            this._confirmPassword.Name = "_confirmPassword";
            this._confirmPassword.Size = new System.Drawing.Size(200, 50);
            this._confirmPassword.TabIndex = 11;
            this._confirmPassword.Value = null;
            // 
            // _validFrom
            // 
            this._validFrom.LabelText = "Valid From";
            this._validFrom.Location = new System.Drawing.Point(231, 110);
            this._validFrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._validFrom.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._validFrom.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._validFrom.Name = "_validFrom";
            this._validFrom.Nullable = true;
            this._validFrom.ShowTime = false;
            this._validFrom.Size = new System.Drawing.Size(200, 50);
            this._validFrom.TabIndex = 12;
            this._validFrom.Value = null;
            // 
            // _validUntil
            // 
            this._validUntil.LabelText = "Valid Until";
            this._validUntil.Location = new System.Drawing.Point(460, 110);
            this._validUntil.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._validUntil.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._validUntil.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._validUntil.Name = "_validUntil";
            this._validUntil.Nullable = true;
            this._validUntil.ShowTime = false;
            this._validUntil.Size = new System.Drawing.Size(200, 50);
            this._validUntil.TabIndex = 13;
            this._validUntil.Value = null;
            // 
            // _setStaffButton
            // 
            this._setStaffButton.Location = new System.Drawing.Point(31, 26);
            this._setStaffButton.Name = "_setStaffButton";
            this._setStaffButton.Size = new System.Drawing.Size(89, 23);
            this._setStaffButton.TabIndex = 16;
            this._setStaffButton.Text = "Set Staff";
            this._setStaffButton.UseVisualStyleBackColor = true;
            this._setStaffButton.Click += new System.EventHandler(this._staffButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this._staffName);
            this.groupBox1.Controls.Add(this._clearStaffButton);
            this.groupBox1.Controls.Add(this._setStaffButton);
            this.groupBox1.Location = new System.Drawing.Point(231, 3);
            this.groupBox1.Name = "groupBox1";
            this.tableLayoutPanel1.SetRowSpan(this.groupBox1, 2);
            this.groupBox1.Size = new System.Drawing.Size(453, 100);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Staff";
            // 
            // _clearStaffButton
            // 
            this._clearStaffButton.Location = new System.Drawing.Point(31, 55);
            this._clearStaffButton.Name = "_clearStaffButton";
            this._clearStaffButton.Size = new System.Drawing.Size(89, 23);
            this._clearStaffButton.TabIndex = 17;
            this._clearStaffButton.Text = "Clear Staff";
            this._clearStaffButton.UseVisualStyleBackColor = true;
            this._clearStaffButton.Click += new System.EventHandler(this._clearStaffButton_Click);
            // 
            // _staffName
            // 
            this._staffName.LabelText = "Staff Name";
            this._staffName.Location = new System.Drawing.Point(126, 26);
            this._staffName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._staffName.Mask = "";
            this._staffName.Name = "_staffName";
            this._staffName.ReadOnly = true;
            this._staffName.Size = new System.Drawing.Size(321, 50);
            this._staffName.TabIndex = 18;
            this._staffName.Value = null;
            // 
            // UserEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UserEditorComponentControl";
            this.Size = new System.Drawing.Size(695, 512);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.TableView _authorityGroups;
        private ClearCanvas.Desktop.View.WinForms.TextField _userId;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _password;
        private ClearCanvas.Desktop.View.WinForms.TextField _confirmPassword;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _validUntil;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _validFrom;
        private System.Windows.Forms.Button _setStaffButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button _clearStaffButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _staffName;

    }
}
