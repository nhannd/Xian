#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    internal class ExceptionHandlingContext : IExceptionHandlingContext
    {
        private readonly Exception _exception;
    	private readonly AbortOperationDelegate _abortDelegate;

        public ExceptionHandlingContext(Exception e, string contextualMessage, IDesktopWindow desktopWindow, AbortOperationDelegate abortOperationDelegate)
        {
            _exception = e;
            ContextualMessage = contextualMessage;
            DesktopWindow = desktopWindow;
            _abortDelegate = abortOperationDelegate;
        }

    	public IDesktopWindow DesktopWindow { get; private set; }
    	public string ContextualMessage { get; private set; }

    	public void Log(LogLevel level, Exception e)
        {
            Platform.Log(level, e);
        }

        public void Abort()
        {
            if (_abortDelegate != null)
            {
                _abortDelegate();
            }
        }

		public void ShowMessageBox(string detailMessage)
        {
			ShowMessageBox(detailMessage, true);
        }

        public void ShowMessageBox(string detailMessage, bool prependContextualMessage)
        {
        	string message = GetMessage(detailMessage, prependContextualMessage);
			if (ExceptionHandler.ShowStackTraceInDialog)
			{
				ShowExceptionDialog(message);
			}
			else
			{
				DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
			}
        }

		private string GetMessage(string detailMessage, bool prependContextualMessage)
		{
			return string.IsNullOrEmpty(ContextualMessage) || prependContextualMessage == false
				? detailMessage
				: string.Format("{0}\r\n - {1}", ContextualMessage, detailMessage);
		}
		
		private void ShowExceptionDialog(string message)
        {
			try
			{
				ExceptionDialog.Show(message, _exception, ExceptionDialogActions.Ok);
			}
			catch (Exception dialogException)
			{
				Platform.Log(LogLevel.Debug, dialogException);

				// fallback to displaying the message in a message box
				DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
			}
		}
    }
}