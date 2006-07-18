using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    public interface ISessionManager
    {
        /// <summary>
        /// Called by the framework at start-up to initiate a session.  Any exception thrown 
        /// from this method will effectively prevent the establishment of a session, causing
        /// execution to terminate.
        /// </summary>
        void InitiateSession();

        /// <summary>
        /// Called by the framework at shutdown to terminate an existing session.
        /// </summary>
        void TerminateSession();
    }
}
