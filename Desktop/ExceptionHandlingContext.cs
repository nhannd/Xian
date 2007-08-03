using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    internal class ExceptionHandlingContext : IExceptionHandlingContext
    {
        private Exception _exception;
        private readonly IDesktopWindow _desktopWindow;
        private readonly string _contextualMessage;
        private readonly AbortOperationDelegate _abortDelegate;

        public ExceptionHandlingContext(Exception e, string contextualMessage, IDesktopWindow desktopWindow, AbortOperationDelegate abortOperationDelegate)
        {
            _exception = e;
            _contextualMessage = contextualMessage;
            _desktopWindow = desktopWindow;
            _abortDelegate = abortOperationDelegate;
        }

        public IDesktopWindow DesktopWindow
        {
            get { return _desktopWindow; }
        }

        public string ContextualMessage
        {
            get { return _contextualMessage; }
        }

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

        public void ShowMessageBox(string message)
        {
            bool showStackTraceInDialog = true;

            try
            {
                showStackTraceInDialog = ExceptionHandlerSettings.Default.ShowStackTraceInDialog;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                ShowExceptionDialog(e, SR.ExceptionCannotLoadExceptionDialogSetting);
            }

            if(showStackTraceInDialog)
            {
                ShowExceptionDialog(_exception, message);
            }
            else
            {
                _desktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);            
            }
        }

        public void ShowMessageBox(string detailMessage, bool prependContextualMessage)
        {
            string message =
                string.IsNullOrEmpty(_contextualMessage) || prependContextualMessage == false
                    ? detailMessage
                    : string.Format("{0}: {1}", _contextualMessage, detailMessage);

            ShowMessageBox(message);
        }

        private void ShowExceptionDialog(Exception e, string message)
        {
            ApplicationComponent.LaunchAsDialog(
                _desktopWindow,
                new ExceptionHandlerComponent(e, message),
                Application.Name);
        }
    }
}