#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
