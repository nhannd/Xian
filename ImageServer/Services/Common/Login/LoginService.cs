using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Services.Login;

namespace ClearCanvas.ImageServer.Services.Common.Login
{
    /// <summary>
    /// Login service
    /// </summary>
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    public class LoginService : ICoreServiceLayer, ILoginService, IDisposable
    {
        private AuthenticationClient _client;

        #region ILoginService Members

        public LoginService()
        {
            try
            {
                _client = new AuthenticationClient();
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex); 
                throw;
            } 
        }

        public SessionInfo Login(string userName, string password)
        {
            
            try
            {
                
                InitiateSessionRequest request = new InitiateSessionRequest(userName, password);
                request.GetAuthorizations = true;

                InitiateSessionResponse response = _client.InitiateSession(request);

                if (response != null)
                {
                    LoginCredentials credentials = new LoginCredentials();
                    credentials.UserName = userName;
                    credentials.DisplayName = response.DisplayName;
                    credentials.SessionToken = response.SessionToken;
                    credentials.Authorities = response.AuthorityTokens;
                    CustomPrincipal user =
                    new CustomPrincipal(new CustomIdentity(userName, response.DisplayName), credentials);
                    SessionInfo session = new SessionInfo(user);

                    return session;
                }
                else
                {
                    throw new SecurityException();
                }
            }
                
            catch(FaultException<PasswordExpiredException> ex)
            {
                throw ex.Detail;
            }
        }

        public void Logout(SessionInfo session)
        {
            TerminateSessionRequest request = new TerminateSessionRequest(session.User.Identity.Name, session.Credentials.SessionToken);
            TerminateSessionResponse response = _client.TerminateSession(request);
        }

        #endregion

        #region ILoginService Members


        public void Validate(SessionInfo session)
        {
            ValidateSessionRequest request = new ValidateSessionRequest(session.User.Identity.Name, session.Credentials.SessionToken);
            request.GetAuthorizations = true;
            ValidateSessionResponse response = _client.ValidateSession(request);
        
            // update session info
            session.Credentials.Authorities = response.AuthorityTokens;
            session.Credentials.SessionToken = response.SessionToken;
        }

        #endregion


        #region ILoginService Members


        public void ChangePassword(string userName, string oldPassword, string newPassword)
        {
            
            ChangePasswordRequest request = new ChangePasswordRequest(userName, oldPassword, newPassword);
            ChangePasswordResponse response = _client.ChangePassword(request);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_client!=null)
            {
                switch(_client.State)
                {
                    case  CommunicationState.Opened:
                        _client.Close();
                        break;

                }
            }
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
