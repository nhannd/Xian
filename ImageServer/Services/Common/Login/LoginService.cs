using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
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

        public LoginResult SignOn(string userName, string password)
        {
            AuthenticationClient client = new AuthenticationClient();
            LoginResult result = new LoginResult();

            InitiateSessionRequest request = new InitiateSessionRequest(userName, password);
            request.GetAuthorizations = true;

            InitiateSessionResponse response = client.InitiateSession(request);
            if (response != null)
            {
                result.Successful = true;
                result.Groups = response.AuthorityTokens;
            }

            return result;
        }

        public void SignOff(string userName, SessionToken token)
        {
            AuthenticationClient client = new AuthenticationClient();
            TerminateSessionRequest request = new TerminateSessionRequest(userName, token);
            TerminateSessionResponse response = client.TerminateSession(request);
            
        }

        #endregion
    }

    class AuthenticationClient : ClientBase<IAuthenticationService>, IAuthenticationService
    {

        #region IAuthenticationService Members

        public InitiateSessionResponse InitiateSession(InitiateSessionRequest request)
        {
            return base.Channel.InitiateSession(request);
        }

        public ValidateSessionResponse ValidateSession(ValidateSessionRequest request)
        {
            return base.Channel.ValidateSession(request);
        }

        public TerminateSessionResponse TerminateSession(TerminateSessionRequest request)
        {
            return base.Channel.TerminateSession(request);
        }

        public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
        {
            return base.Channel.ChangePassword(request);
        }

        public GetAuthorizationsResponse GetAuthorizations(GetAuthorizationsRequest request)
        {
            return base.Channel.GetAuthorizations(request);
        }

        #endregion
    }
}
