#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ExtensionOf(typeof(UserUpgradeProgressDialogViewExtensionPoint))]
	internal class UserUpgradeProgressDialogView : WinFormsView, IUserUpgradeProgressDialogView
	{
		private UserUpgradeProgressForm _form;

		#region IUserUpgradeProgressDialogView Members

		public void RunModal(string title, string startupMessage)
		{
			_form = new UserUpgradeProgressForm(title) { Message = startupMessage };
			_form.ShowDialog();
		}

		public void SetMessage(string message)
		{
			_form.Message = message;
		}

		public void SetProgressPercent(int progressPercent)
		{
			_form.ProgressPercent = progressPercent;
		}

		public void Close(string failureMessage)
		{
			_form.Close();

			if (String.IsNullOrEmpty(failureMessage))
				return;

			new MessageBox().Show(failureMessage, MessageBoxActions.Ok);
		}

		#endregion

		public override object GuiElement
		{
			get { throw new NotImplementedException(); }
		}
	}
}
