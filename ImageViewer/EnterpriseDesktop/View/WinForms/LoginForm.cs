#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using System.Drawing;

namespace ClearCanvas.ImageViewer.EnterpriseDesktop.View.WinForms
{
	public partial class LoginForm : Form
	{
		//private string[] _facilityChoices;
		private Point _refPoint;

		public LoginForm()
		{
			// Need to explicitely dismiss the splash screen here, as the login dialog is shown before the desktop window, which is normally
			// responsible for dismissing it.
#if !MONO
			SplashScreenManager.DismissSplashScreen(this);
#endif

			InitializeComponent();
		}

		public void SetMode(LoginDialogMode mode)
		{
			_userName.Enabled = mode == LoginDialogMode.InitialLogin;
			//_domain.Enabled = mode == LoginDialogMode.InitialLogin;
		}

		//public string[] FacilityChoices
		//{
		//    get { return _facilityChoices; }
		//    set
		//    {
		//        _facilityChoices = value;
		//        _domain.Items.Clear();
		//        _domain.Items.AddRange(_facilityChoices);
		//    }
		//}

		//public string SelectedFacility
		//{
		//    get { return (string)_domain.SelectedItem; }
		//    set
		//    {
		//        _domain.SelectedItem = value;
		//    }
		//}

		public string UserName
		{
			get { return _userName.Text; }
			set { _userName.Text = value; }
		}

		public string Password
		{
			get { return _password.Text; }
		}



		private void _loginButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void LoginForm_Load(object sender, EventArgs e)
		{
			// depending on use-case, the username may already be filled in
			if (string.IsNullOrEmpty(_userName.Text))
				_userName.Select();
			else
				_password.Select();
		}

		private void _userName_TextChanged(object sender, EventArgs e)
		{
			UpdateButtonStates();
		}

		private void _password_TextChanged(object sender, EventArgs e)
		{
			UpdateButtonStates();
		}

		private void _facility_SelectedValueChanged(object sender, EventArgs e)
		{
			UpdateButtonStates();
		}

		private void UpdateButtonStates()
		{
			bool ok = !string.IsNullOrEmpty(_userName.Text) && !string.IsNullOrEmpty(_password.Text);
			_loginButton.Enabled = ok;
		}

		private void LoginForm_MouseDown(object sender, MouseEventArgs e)
		{
			_refPoint = new Point(e.X, e.Y);
		}

		private void LoginForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.Left += (e.X - _refPoint.X);
				this.Top += (e.Y - _refPoint.Y);
			}
		}
	}
}
