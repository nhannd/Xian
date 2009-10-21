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
            // by default we choose the more secure option and don't show the stack trace
            bool showStackTraceInDialog = false;

            try
            {
                showStackTraceInDialog = ExceptionHandlerSettings.Default.ShowStackTraceInDialog;
            }
            catch (Exception e)
            {
                // if we can't retrieve the setting for whatever reason, just log it
                // and move on
                Platform.Log(LogLevel.Error, e);
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
                    : string.Format("{0}: \n{1}", _contextualMessage, detailMessage);

            ShowMessageBox(message);
        }

        private void ShowExceptionDialog(Exception e, string message)
        {
            try
            {
                ApplicationComponent.LaunchAsDialog(
                    _desktopWindow,
                    new ExceptionHandlerComponent(e, message),
                    Application.Name);

            }
            catch (Exception dialogException)
            {
                // failed to launch ExceptionHandlerComponent - just log this
                Platform.Log(LogLevel.Error, dialogException);

                // fallback to displaying the message in a message box
                _desktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
            }
        }
    }
}