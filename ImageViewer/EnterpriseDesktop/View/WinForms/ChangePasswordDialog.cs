#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.EnterpriseDesktop.View.WinForms
{
	[ExtensionOf(typeof(ChangePasswordDialogExtensionPoint))]
	public class ChangePasswordDialog : IChangePasswordDialog
	{
		private ChangePasswordForm _form;

		public ChangePasswordDialog()
		{
			_form = new ChangePasswordForm();
		}

		#region IChangePasswordDialog Members

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

		public string UserName
		{
			get { return _form.UserName; }
			set { _form.UserName = value; }
		}

		public string Password
		{
			get { return _form.Password; }
			set { _form.Password = value; }
		}

		public string NewPassword
		{
			get { return _form.NewPassword; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			// nothing to do
		}

		#endregion
	}
}
