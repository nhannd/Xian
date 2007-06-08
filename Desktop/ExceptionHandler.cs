using System;

namespace ClearCanvas.Desktop
{
    ///<summary>
    /// Provides an <see cref="IExceptionPolicy"/> with a callback to abort the Exception-causing operation.
    /// Each individual <see cref="IExceptionPolicy"/> will determine if this is appropriate to be called.
    ///</summary>
    public delegate void AbortOperationDelegate();

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
            Report(e, null, desktopWindow);
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
            Report(e, userMessage, desktopWindow, null);
        }

        ///<summary>
        ///</summary>
        ///<param name="e"></param>
        ///<param name="contextualMessage"></param>
        ///<param name="desktopWindow"></param>
        ///<param name="abortDelegate"></param>
        public static void Report(Exception e, string contextualMessage, IDesktopWindow desktopWindow, AbortOperationDelegate abortDelegate)
        {
            ExceptionPolicyFactory.GetPolicy(e.GetType()).
                Handle(e, new ExceptionHandlingContext(e, contextualMessage, desktopWindow, abortDelegate));
        }
    }
}
