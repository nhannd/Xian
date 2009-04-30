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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface to extensions of <see cref="SessionManagerExtensionPoint"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A session manager extension is optional.  If present, the application will load the session manager and
    /// call its <see cref="InitiateSession"/> and <see cref="TerminateSession"/> at the beginning and end
    /// of the applications execution, respectively.
    /// </para>
    /// <para>
    /// The purpose of the session manager is to provide a hook through which custom session management can occur.
    /// A typical session manager implemenation may show a login dialog at start-up in order to gather user credentials,
    /// and may perform other custom initialization.
    /// </para>
    /// </remarks>
    public interface ISessionManager
    {
        /// <summary>
        /// Called by the framework at start-up to initiate a session.
        /// </summary>
        /// <remarks>
        /// This method is called after the GUI system and application view have been initialized,
        /// so the implementation may interact with the user if necessary, and may
        /// make use of the <see cref="Application"/> object.  However, no desktop windows exist yet.
        /// Any exception thrown from this method will effectively prevent the establishment of a session, causing
        /// execution to terminate with an error.  A return value of false may be used
        /// to silently refuse initiation of a session.  In this case, no error is reported, but the application
        /// terminates immediately.
        /// </remarks>
        bool InitiateSession();

        /// <summary>
        /// Called by the framework at shutdown to terminate an existing session.
        /// </summary>
        /// <remarks>
        /// This method is called prior to terminating the GUI subsytem and application view, so the
        /// implementation may interact with the user if necessary.
        /// </remarks>
        void TerminateSession();
    }
}
