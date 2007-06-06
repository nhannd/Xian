using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    ///<summary>
    ///</summary>
    public enum ExceptionHandlingMessageBoxType
    {
        ///<summary>
        ///</summary>
        Simple,

        ///<summary>
        ///</summary>
        Detailed
    }

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
        /// A user message provided by the component which should be common for all exceptions
        ///</summary>
        string UserMessage { get; }

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
        /// Shows the specified text in a simple message box in the context's <see cref="IDesktopWindow"/>
        ///</summary>
        ///<param name="message"></param>
        void ShowMessageBox(string message);

        ///<summary>
        ///</summary>
        ///<param name="e"></param>
        ///<param name="type"></param>
        void ShowMessageBox(Exception e, ExceptionHandlingMessageBoxType type);
    }
}