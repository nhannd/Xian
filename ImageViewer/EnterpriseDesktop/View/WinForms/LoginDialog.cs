#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.EnterpriseDesktop.View.WinForms
{
	[ExtensionOf(typeof(LoginDialogExtensionPoint))]
	public class LoginDialog : ILoginDialog
	{
		private LoginForm _form;
		private LoginDialogMode _mode;

		public LoginDialog()
		{
			_form = new LoginForm();
		}

		#region ILoginDialog Members

		public bool Show()
		{
			System.Windows.Forms.Application.EnableVisualStyles();

			if (_form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public LoginDialogMode Mode
		{
			get { return _mode; }
			set
			{
				_mode = value;
				_form.SetMode(_mode);
			}
		}

		public string UserName
		{
			get { return _form.UserName; }
			set { _form.UserName = value; }
		}

		public string Password
		{
			get { return _form.Password; }
		}

		//public string Domain
		//{
		//    get { return null; }
		//    set { ; }
		//}

		//public string[] DomainChoices
		//{
		//    get { return new string[0]; }
		//    set { ; }
		//}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			// nothing to do
		}

		#endregion
	}
}