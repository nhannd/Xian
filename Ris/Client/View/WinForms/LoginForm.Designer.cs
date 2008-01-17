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
    partial class LoginForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this._cancelButton = new System.Windows.Forms.Button();
            this._loginButton = new System.Windows.Forms.Button();
            this._userName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._facility = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._password = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.SuspendLayout();
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(173, 174);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 4;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _loginButton
            // 
            this._loginButton.Enabled = false;
            this._loginButton.Location = new System.Drawing.Point(94, 174);
            this._loginButton.Margin = new System.Windows.Forms.Padding(2);
            this._loginButton.Name = "_loginButton";
            this._loginButton.Size = new System.Drawing.Size(75, 23);
            this._loginButton.TabIndex = 3;
            this._loginButton.Text = "Login";
            this._loginButton.UseVisualStyleBackColor = true;
            this._loginButton.Click += new System.EventHandler(this._loginButton_Click);
            // 
            // _userName
            // 
            this._userName.LabelText = "User Name";
            this._userName.Location = new System.Drawing.Point(11, 11);
            this._userName.Margin = new System.Windows.Forms.Padding(2);
            this._userName.Mask = "";
            this._userName.Name = "_userName";
            this._userName.PasswordChar = '\0';
            this._userName.Size = new System.Drawing.Size(237, 41);
            this._userName.TabIndex = 0;
            this._userName.ToolTip = null;
            this._userName.Value = null;
            this._userName.ValueChanged += new System.EventHandler(this._userName_ValueChanged);
            // 
            // _facility
            // 
            this._facility.DataSource = null;
            this._facility.DisplayMember = "";
            this._facility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._facility.LabelText = "Facility";
            this._facility.Location = new System.Drawing.Point(11, 110);
            this._facility.Margin = new System.Windows.Forms.Padding(2);
            this._facility.Name = "_facility";
            this._facility.Size = new System.Drawing.Size(237, 41);
            this._facility.TabIndex = 2;
            this._facility.Value = null;
            // 
            // _password
            // 
            this._password.LabelText = "Password";
            this._password.Location = new System.Drawing.Point(11, 58);
            this._password.Margin = new System.Windows.Forms.Padding(2);
            this._password.Mask = "";
            this._password.Name = "_password";
            this._password.PasswordChar = '*';
            this._password.Size = new System.Drawing.Size(237, 41);
            this._password.TabIndex = 1;
            this._password.ToolTip = null;
            this._password.Value = null;
            // 
            // LoginForm
            // 
            this.AcceptButton = this._cancelButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 208);
            this.Controls.Add(this._password);
            this.Controls.Add(this._facility);
            this.Controls.Add(this._userName);
            this.Controls.Add(this._loginButton);
            this.Controls.Add(this._cancelButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ClearCanvas Login";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _loginButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _userName;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _facility;
        private ClearCanvas.Desktop.View.WinForms.TextField _password;
    }
}