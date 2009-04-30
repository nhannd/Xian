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
        /// Shows the specified message in a message box in the context's <see cref="IDesktopWindow"/>.
        ///</summary>
        ///<param name="message">The message to be displayed.</param>
        void ShowMessageBox(string message);

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