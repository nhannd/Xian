using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    ///<summary>
    /// Provides contextual information for an <see cref="IExceptionPolicy"/> to handle an <see cref="Exception"/>
    ///</summary>
    public interface IExceptionHandlingContext
    {
        ///<summary>
        /// The <see cref="IDesktopWindow"/> of the component in which the exception has occurred
        ///</summary>
        IDesktopWindow DesktopWindow { get; }

        ///<summary>
        /// A contextual user-friendly message provided by the component which should be common for all exceptions
        ///</summary>
        string ContextualMessage { get; }

        ///<summary>
        /// Logs the specified exception as an error
        ///</summary>
        ///<param name="e"></param>
        void Log(Exception e);

        ///<summary>
        ///</summary>
        ///<param name="e"></param>
        ///<param name="level"></param>
        void Log(Exception e, LogLevel level);

        ///<summary>
        ///</summary>
        void Abort();

        ///<summary>
        /// Shows the specified message in a message box in the context's <see cref="IDesktopWindow"/>.
        ///</summary>
        ///<param name="detailMessage"></param>
        void ShowMessageBox(string message);

        ///<summary>
        /// Shows the specified detail message in a message box in the context's <see cref="IDesktopWindow"/>.
        /// Optionally prepends the contextual message supplied by the application component to the detail message.
        ///</summary>
        ///<param name="detailMessage"></param>
        void ShowMessageBox(string detailMessage, bool prependContextualMessage);
    }
}