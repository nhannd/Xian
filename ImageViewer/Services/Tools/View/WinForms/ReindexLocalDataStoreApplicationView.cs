#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Common.LocalDataStore;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;
using System.Windows.Forms;
using ClearCanvas.Common;
using Application=ClearCanvas.Desktop.Application;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
	[ExtensionOf(typeof(ReindexLocalDataStoreApplicationViewExtensionPoint))]
	public class ReindexLocalDataStoreApplicationView : ApplicationView, IReindexLocalDataStoreApplicationView
	{
		private ReindexStartupDialogForm _startupDialog;
		private ReindexLocalDataStoreDialogForm _reindexDialog;
		private NotifyDelegate _notifyReindexClosed;

		private readonly Stack<Form> _messageBoxes = new Stack<Form>();

		#region IReindexLocalDataStoreApplicationView Members

		public void ShowStartupDialog(string message)
		{
			if (_reindexDialog != null)
				throw new InvalidOperationException();

			if (_startupDialog != null)
				return;

			_startupDialog = new ReindexStartupDialogForm {Text = Application.Name };
			if (!String.IsNullOrEmpty(message))
				_startupDialog.Message = message;

			SplashScreenManager.DismissSplashScreen(_startupDialog);
			_startupDialog.Show();
			_startupDialog.Activate();
		}

		public void DismissStartupDialog()
		{
			if (_startupDialog == null)
				return;
				
			_startupDialog.Close();
			_startupDialog.Dispose();
			_startupDialog = null;
		}

		public void ShowReindexDialog(ILocalDataStoreReindexer reindexer, NotifyDelegate notifyUserClosed)
		{
			if (_reindexDialog != null)
				return;

			_notifyReindexClosed = notifyUserClosed;
			DismissStartupDialog();
			_reindexDialog = new ReindexLocalDataStoreDialogForm(reindexer) {Text = Application.Name };
			_reindexDialog.FormClosed += OnReindexDialogClosed;
			SplashScreenManager.DismissSplashScreen(_reindexDialog);
			//Post it so the splash screen can go away.
			_reindexDialog.Show();
			_reindexDialog.Activate();
		}

		void OnReindexDialogClosed(object sender, FormClosedEventArgs e)
		{
			if (_notifyReindexClosed != null)
				_notifyReindexClosed();
		}

		public void DismissReindexDialog()
		{
			if (_reindexDialog == null)
				return;

			_reindexDialog.DialogResult = DialogResult.OK;
			_reindexDialog.Dispose();
			_reindexDialog = null;
		}

		public void ShowMessageBox(string message)
		{
			var parentForm = _startupDialog as Form ?? _reindexDialog;

			var messageBox = new DismissableMessageBox
			                 	{
									Text = Application.Name,
			                 		Message = message,
			                 		ShowInTaskbar = parentForm == null,
									StartPosition = parentForm == null ? FormStartPosition.CenterScreen : FormStartPosition.CenterParent
			                 	};
			
			SplashScreenManager.DismissSplashScreen(messageBox);
			_messageBoxes.Push(messageBox);
			messageBox.ShowDialog(parentForm);
			messageBox.Dispose();
		}

		public void DismissMessageBoxes()
		{
			while (_messageBoxes.Count > 0)
			{
				var messageBox = _messageBoxes.Pop();
				messageBox.DialogResult = DialogResult.OK;
				messageBox.Close();
			}
		}

		#endregion

		public override IDesktopWindowView CreateDesktopWindowView(DesktopWindow window)
		{
			throw new InvalidOperationException();
		}

		public override DialogBoxAction ShowMessageBox(string message, MessageBoxActions actions)
		{
			throw new InvalidOperationException();
		}

		#region IDisposable Members

		public void Dispose()
		{
			DismissMessageBoxes();
			DismissStartupDialog();
			DismissReindexDialog();
		}

		#endregion
	}
}
