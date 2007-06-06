using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    internal class ExceptionHandlingContext : IExceptionHandlingContext
    {
        private readonly IDesktopWindow _desktopWindow;
        private readonly string _userMessage;
        private readonly AbortOperationDelegate _abortDelegate;

        public ExceptionHandlingContext(string userMessage, IDesktopWindow desktopWindow, AbortOperationDelegate abortOperationDelegate)
        {
            _userMessage = userMessage;
            _desktopWindow = desktopWindow;
            _abortDelegate = abortOperationDelegate;
        }

        public IDesktopWindow DesktopWindow
        {
            get { return _desktopWindow; }
        }

        public string UserMessage
        {
            get { return _userMessage; }
        }

        public void Log(Exception e)
        {
            Platform.Log(e);
        }

        public void Log(Exception e, LogLevel level)
        {
            Platform.Log(e, level);
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
            ShowSimpleMessageBox(message);
        }

        public void ShowMessageBox(Exception e, ExceptionHandlingMessageBoxType type)
        {
            switch (type)
            {
                case ExceptionHandlingMessageBoxType.Simple:
                    ShowSimpleMessageBox(e.Message);
                    break;
                case ExceptionHandlingMessageBoxType.Detailed:
                    ShowDetailedMessageBox(e);
                    break;
            }
        }

        private void ShowDetailedMessageBox(Exception e)
        {
            ApplicationComponent.LaunchAsDialog(
                _desktopWindow,
                new ExceptionHandlerComponent(e, _userMessage),
                Application.ApplicationName);
        }

        private void ShowSimpleMessageBox(string message)
        {
            _desktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);            
        }
    }
}