using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public interface ISessionManager
    {
        /// <summary>
        /// Called by the framework at start-up to initiate a session.
        /// </summary>
        /// <remarks>
        /// Any exception thrown from this method will effectively prevent the establishment of a session, causing
        /// execution to terminate with a message box.  A return value of false may be used
        /// to silently refuse initiation of a session.  In this case, no message box is shown, so
        /// the reason for failure must be clear to the user by other means.
        /// </remarks>
        bool InitiateSession();

        /// <summary>
        /// Called by the framework at shutdown to terminate an existing session.
        /// </summary>
        void TerminateSession();
    }
}
