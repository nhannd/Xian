using System.Security;
using System.Web.Security;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Services.Common.Authentication
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class DefaultAuthenticationService : IApplicationServiceLayer, IAuthenticationService
    {
        #region IAuthenticationService Members

        public InitiateSessionResponse InitiateSession(InitiateSessionRequest request)
        {
            bool ok = Membership.ValidateUser(request.UserName, request.Password);
            if (ok)
            {
                SessionToken token = new SessionToken(request.UserName, Platform.Time.AddMinutes(1));
                string[] authority = Roles.GetRolesForUser(request.UserName);
                string displayName = request.UserName;
                InitiateSessionResponse rsp = new InitiateSessionResponse(token, authority, displayName);

                SessionTokenManager.Instance.AddSession(token);

                return rsp;
            }
            else
            {
                throw new SecurityException("Invalid username or password");
            }
            
        }

        public ValidateSessionResponse ValidateSession(ValidateSessionRequest request)
        {
            SessionToken session = SessionTokenManager.Instance.FindSession(request.SessionToken.Id);
            if (session!=null)
            {
                string[] authorities = Roles.GetRolesForUser(request.SessionToken.Id);
                return new ValidateSessionResponse(session, authorities);
            }
            else
            {
                throw new SecurityException("Invalid Session");
            }
        }

        public TerminateSessionResponse TerminateSession(TerminateSessionRequest request)
        {
            SessionTokenManager.Instance.RemoveSession(request.SessionToken);
            return new TerminateSessionResponse();
        }

        public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
        {
            if (Membership.Provider.ChangePassword(request.UserName, request.CurrentPassword, request.NewPassword))
                return new ChangePasswordResponse();
            else
                throw new SecurityException("Unable to change password");
        }

        public GetAuthorizationsResponse GetAuthorizations(GetAuthorizationsRequest request)
        {
            string[] authorities = Roles.GetRolesForUser(request.UserName);
            return new GetAuthorizationsResponse(authorities);
        }

        #endregion
    }
}