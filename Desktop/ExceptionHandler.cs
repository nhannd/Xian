using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Contains static methods used to report exceptions to the user
    /// </summary>
    public static class ExceptionHandler
    {
        /// <summary>
        /// Reports the specified exception to the user, using the <see cref="Exception.Message"/> property value as the
        /// message.  Also logs the exception.
        /// </summary>
        /// <param name="e">Exception to report</param>
        /// <param name="desktopWindow">Desktop window that parents the exception dialog</param>
        public static void Report(Exception e, IDesktopWindow desktopWindow)
        {
            ExceptionHandler.Report(e, null, desktopWindow);
        }

        /// <summary>
        /// Reports the specified exception to the user, displaying the specified user message first.
        /// Also logs the exception.
        /// </summary>
        /// <param name="e">Exception to report</param>
        /// <param name="userMessage">User-friendly message to display, instead of the message contained in the exception</param>
        /// <param name="desktopWindow">Desktop window that parents the exception dialog</param>
        public static void Report(Exception e, string userMessage, IDesktopWindow desktopWindow)
        {
            Platform.Log(e, LogLevel.Error);
            
            IExceptionPolicy policy = ExceptionPolicyFactory.GetPolicy(e.GetType());
            ExceptionReport report = policy.Handle(e, userMessage);

            if (report.Action == ExceptionReportAction.ReportInDialog)
            {
                ApplicationComponent.LaunchAsDialog(
                    desktopWindow,
                    new ExceptionHandlerComponent(e, report.Message),
                    Application.ApplicationName);
            }
        }
    }
}
