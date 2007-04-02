using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.Login;
using System.Threading;
using System.Security.Principal;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Holds information related to the current login session.  This information may be sensitive, therefore
    /// all members of this class are either private or internal.
    /// </summary>
    internal sealed class LoginSession
    {
        private static LoginSession _current;

        internal static LoginSession Current
        {
            get { return _current; }
        }

        internal static void Create(string userName, string password)
        {
            // set the current session before attempting to access login service
            // the service will use the credentials to attempt to connect to the server
            _current = new LoginSession(userName, password);

            Platform.GetService<ILoginService>(
                delegate(ILoginService service)
                {
                    LoginResponse response = service.Login(new LoginRequest());

                    // if the call succeeded, construct a generic principal object on this thread, containing
                    // the set of authority tokens for this user
                    Thread.CurrentPrincipal = new GenericPrincipal(
                        new GenericIdentity(userName), response.UserAuthorityTokens);
                });
        }

        private string _userName;
        private string _password;

        private LoginSession(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

        internal string UserName
        {
            get { return _userName; }
        }

        internal string Password
        {
            get { return _password; }
        }
    }
}
