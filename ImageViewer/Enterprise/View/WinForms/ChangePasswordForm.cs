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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Enterprise.View.WinForms
{
	public partial class ChangePasswordForm : Form
	{
		public ChangePasswordForm()
		{
			InitializeComponent();
		}

		public string UserName
		{
			get { return _userName.Text; }
			set { _userName.Text = value; }
		}

		public string Password
		{
			get { return _password.Text; }
			set { _password.Text = value; }
		}

		public string NewPassword
		{
			get { return _newPassword.Text; }
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void _password_TextChanged(object sender, EventArgs e)
		{
			UpdateButtonStates();
		}

		private void _newPassword_TextChanged(object sender, EventArgs e)
		{
			UpdateButtonStates();
		}

		private void _newPasswordConfirm_TextChanged(object sender, EventArgs e)
		{
			UpdateButtonStates();
		}

		private void UpdateButtonStates()
		{
			_errorProvider.SetError(_newPassword,
			                        _newPassword.Text == _password.Text ?
			                                                            	"New password must be different from previous" : null);

			_errorProvider.SetError(_newPasswordConfirm,
			                        _newPassword.Text != _newPasswordConfirm.Text ?
			                                                                      	"New passwords do not match" : null);

			bool ok = !string.IsNullOrEmpty(_userName.Text) && !string.IsNullOrEmpty(_password.Text) &&
			          !string.IsNullOrEmpty(_newPassword.Text) && !string.IsNullOrEmpty(_newPasswordConfirm.Text) &&
			          _newPassword.Text.Equals(_newPasswordConfirm.Text);

			_okButton.Enabled = ok;
		}

		private void ChangePasswordForm_Load(object sender, EventArgs e)
		{
			// depending on use-case, the old password may already be filled in
			if (string.IsNullOrEmpty(_password.Text))
				_password.Select();
			else
				_newPassword.Select();
		}
	}
}