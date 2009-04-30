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

namespace ClearCanvas.Desktop
{
    ///<summary>
    /// Provides an <see cref="IExceptionPolicy"/> with a callback to abort the Exception-causing operation.
    ///</summary>
	///<remarks>
	/// Each individual <see cref="IExceptionPolicy"/> will determine if this is appropriate to be called.
	/// </remarks>
	public delegate void AbortOperationDelegate();

    /// <summary>
    /// Contains static methods used to report exceptions to the user.
    /// </summary>
    public static class ExceptionHandler
    {
        /// <summary>
        /// Reports the specified exception to the user, using the <see cref="Exception.Message"/> property value as the
        /// message.
        /// </summary>
        /// <remarks>
		/// The exception is also automatically logged.
		/// </remarks>
        /// <param name="e">Exception to report.</param>
        /// <param name="desktopWindow">Desktop window that parents the exception dialog.</param>
        public static void Report(Exception e, IDesktopWindow desktopWindow)
        {
            Report(e, null, desktopWindow);
        }

        /// <summary>
        /// Reports the specified exception to the user, displaying the specified user message first.
        /// </summary>
		/// <remarks>
		/// The exception is also automatically logged.
		/// </remarks>
		/// <param name="e">Exception to report.</param>
        /// <param name="userMessage">User-friendly message to display, instead of the message contained in the exception.</param>
        /// <param name="desktopWindow">Desktop window that parents the exception dialog.</param>
        public static void Report(Exception e, string userMessage, IDesktopWindow desktopWindow)
        {
            Report(e, userMessage, desktopWindow, null);
        }

		/// <summary>
		/// Reports the specified exception to the user, displaying the specified user message first.
		/// </summary>
		/// <remarks>
		/// The exception is also automatically logged.
		/// </remarks>
		/// <param name="e">Exception to report.</param>
		/// <param name="contextualMessage">User-friendly (contextual) message to display, instead of the message contained in the exception.</param>
		/// <param name="desktopWindow">Desktop window that parents the exception dialog.</param>
		/// <param name="abortDelegate">A callback delegate for aborting the exception-causing operation.  Decision as to whether or
		/// not the callback is called is up to the individual <see cref="IExceptionPolicy"/>.</param>
		public static void Report(Exception e, string contextualMessage, IDesktopWindow desktopWindow, AbortOperationDelegate abortDelegate)
        {
            ExceptionPolicyFactory.GetPolicy(e.GetType()).
                Handle(e, new ExceptionHandlingContext(e, contextualMessage, desktopWindow, abortDelegate));
        }
    }
}
