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
    ///<summary>
    /// Provides contextual information for an <see cref="IExceptionPolicy"/> to handle an <see cref="Exception"/>.
    ///</summary>
    public interface IExceptionHandlingContext
    {
        ///<summary>
        /// The <see cref="IDesktopWindow"/> of the component in which the exception has occurred.
        ///</summary>
        IDesktopWindow DesktopWindow { get; }

        ///<summary>
        /// A contextual user-friendly message provided by the component which should be common for all exceptions.
        ///</summary>
        string ContextualMessage { get; }

        ///<summary>
        /// Logs the specified exception.
        ///</summary>
        void Log(LogLevel level, Exception e);

        ///<summary>
        /// Aborts the exception-causing operation.
        ///</summary>
        void Abort();

		///<summary>
		/// Shows the specified detail message in a message box in the context's <see cref="IDesktopWindow"/>.
		///</summary>
		/// <remarks>
		/// Automatically prepends the contextual message supplied by the application component to the detail message.
		/// </remarks>
		///<param name="detailMessage">The message to be shown.</param>
		void ShowMessageBox(string detailMessage);

        ///<summary>
        /// Shows the specified detail message in a message box in the context's <see cref="IDesktopWindow"/>.
        ///</summary>
        /// <remarks>
		/// Optionally prepends the contextual message supplied by the application component to the detail message.
		/// </remarks>
        ///<param name="detailMessage">The message to be shown.</param>
		///<param name="prependContextualMessage">Indicates whether or not to prepend the <see cref="ContextualMessage"/>
		/// before <paramref name="detailMessage"/>.</param>
        void ShowMessageBox(string detailMessage, bool prependContextualMessage);
    }
}