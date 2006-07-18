using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Default enterprise session manager implementation.  For the moment, this implementation simply
    /// creates a new session.  Eventually it should do some authentication.
    /// </summary>
    [ExtensionOf(typeof(SessionManagerExtensionPoint))]
    public class SessionManager : ISessionManager
    {
        #region ISessionManager Members

        public void InitiateSession()
        {
            Session.Current = new Session();
        }

        public void TerminateSession()
        {
            Session.Current = null;
        }

        #endregion
    }
}
