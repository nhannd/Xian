using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Services.Login;

namespace ClearCanvas.ImageServer.Services.Common.Login
{
    /// <summary>
    /// Login service
    /// </summary>
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    public class LoginService : ICoreServiceLayer, ILoginService
    {

        #region ILoginService Members

        public LoginResult SignIn(string userName, string password)
        {
            AuthenticationClient client = new AuthenticationClient();
            LoginResult result = new LoginResult();
            result.Successful = false;

            SessionToken token = client.SignOn(userName, password);
            if (token!=null)
            {
                result.Successful = true;
                result.Token = token;
                result.Groups = client.ListAuthorityTokensForUser(userName);
            }

            return result;
        }

        public void SignOff(string userName, SessionToken token)
        {
            AuthenticationClient client = new AuthenticationClient();
            client.SignOff(userName, token);
        }

        #endregion
    }

    class AuthenticationClient : ClientBase<ClearCanvas.ImageServer.Common.IAuthenticationService>,
        ClearCanvas.ImageServer.Common.IAuthenticationService
    {

        #region IAuthenticationService Members

        public SessionToken SignOn(string userName, string password)
        {
            return base.Channel.SignOn(userName, password);
        }

        public SessionToken ValidateUserSession(string userName, SessionToken sessionToken)
        {
            return base.Channel.ValidateUserSession(userName, sessionToken);
        }

        public void SignOff(string userName, SessionToken sessionToken)
        {
            base.Channel.SignOff(userName, sessionToken);
        }

        public void ChangePassword(string userName, string currentPassword, string newPassword)
        {
            base.Channel.ChangePassword(userName, currentPassword, newPassword);
        }

        public string[] ListAuthorityTokensForUser(string userName)
        {
            return base.Channel.ListAuthorityTokensForUser(userName);
        }

        public bool AssertTokenForUser(string userName, string token)
        {
            return base.Channel.AssertTokenForUser(userName, token);
        }

        #endregion
    }
}
