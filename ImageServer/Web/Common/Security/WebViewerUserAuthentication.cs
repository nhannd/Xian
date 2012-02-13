#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Web.Enterprise.Authentication;
using ClearCanvas.Web.Services;

namespace ClearCanvas.ImageServer.Web.Common.Security
{
    [ExtensionOf(typeof(UserAuthenticationExtensionPoint))]
    public class WebViewerUserAuthentication:IUserAuthentication
    {
        public UserSessionInfo QuerySession(string sessionId)
        {
            using(LoginService service = new LoginService())
            {
                var sessionInfo = service.Query(sessionId);
                if (sessionInfo == null)
                {
                    throw new SessionDoesNotExistException();
                }
                return new UserSessionInfo(sessionInfo.User, sessionInfo.Credentials.SessionToken);
            }
        }

        public UserSessionInfo RenewSession(UserSessionInfo session)
        {
            using (LoginService service = new LoginService())
            {
                var newToken = service.Renew(session.SessionToken.Id);
                return new UserSessionInfo(session.Principal, newToken);
            }
        }

        public void Logout(UserSessionInfo session)
        {
            using (LoginService service = new LoginService())
            {
                service.Logout(session.SessionToken.Id);
            }
        }
    }
}