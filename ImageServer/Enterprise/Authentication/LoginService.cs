using System;
using System.Security;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Enterprise.Authentication
{
    /// <summary>
    /// Wrapper for <see cref="IAuthenticationService"/> service.
    /// </summary>
    public sealed class LoginService : IDisposable
    {
        public SessionInfo Login(string userName, string password)
        {
            SessionInfo session = null;
            
            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService  service)
                    {
                        try
                        {
                            InitiateSessionRequest request = new InitiateSessionRequest(userName, password);
                            request.GetAuthorizations = true;
                            InitiateSessionResponse response = service.InitiateSession(request);

                            if (response != null)
                            {
                                LoginCredentials credentials = new LoginCredentials();
                                credentials.UserName = userName;
                                credentials.DisplayName = response.DisplayName;
                                credentials.SessionToken = response.SessionToken;
                                credentials.Authorities = response.AuthorityTokens;
                                CustomPrincipal user =
                                    new CustomPrincipal(new CustomIdentity(userName, response.DisplayName),
                                                        credentials);
                                session = new SessionInfo(user);
                                
                            }
                        }
                        catch (FaultException<PasswordExpiredException> ex)
                        {
                            throw ex.Detail;
                        }
                    }
                );

            return session;
        }

        public void Logout(SessionInfo session)
        {
            TerminateSessionRequest request =
                new TerminateSessionRequest(session.User.Identity.Name, session.Credentials.SessionToken);

            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                    {
                        service.TerminateSession(request);
                    });
        }

        public void Validate(SessionInfo session)
        {
            ValidateSessionRequest request = new ValidateSessionRequest(session.User.Identity.Name, session.Credentials.SessionToken);
            request.GetAuthorizations = true;

            try
            {
                Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                    {
                        ValidateSessionResponse response = service.ValidateSession(request);
                        // update session info
                        session.Credentials.Authorities = response.AuthorityTokens;
                        session.Credentials.SessionToken = response.SessionToken;
                    });
            }
            catch(Exception ex)
            {
                //TODO: for now we can't distinguish communicate errors and credential validation errors.
                // All exceptions are treated the same: we can't verify the login.
                SessionValidationException e = new SessionValidationException(ex);
                throw e;
            }
            

            
        }

        public void ChangePassword(string userName, string oldPassword, string newPassword)
        {

            ChangePasswordRequest request = new ChangePasswordRequest(userName, oldPassword, newPassword);
            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                    {
                        service.ChangePassword(request);
                           
                    });
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}