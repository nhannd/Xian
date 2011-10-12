#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using System.Web.Security;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Services.Common.Authentication
{
    [ServiceImplementsContract(typeof(IAuthenticationService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class DefaultAuthenticationService : IApplicationServiceLayer, IAuthenticationService
    {
        #region IAuthenticationService Members

        public InitiateSessionResponse InitiateSession(InitiateSessionRequest request)
        {
            bool ok = Membership.ValidateUser(request.UserName, request.Password);
            if (ok)
            {
                Guid tokenId = Guid.NewGuid();
                var token = new SessionToken(tokenId.ToString(), Platform.Time + ServerPlatform.WebSessionTimeout);
                string[] authority = Roles.GetRolesForUser(request.UserName);
                string displayName = request.UserName;

                var rsp = new InitiateSessionResponse(token, authority, new Guid[0], displayName,string.Empty);

                SessionTokenManager.Instance.AddSession(token);

                return rsp;
            }
            throw new FaultException<UserAccessDeniedException>(new UserAccessDeniedException());
        }

        public ValidateSessionResponse ValidateSession(ValidateSessionRequest request)
        {
            SessionToken session = SessionTokenManager.Instance.FindSession(request.SessionToken.Id);
            if (session!=null)
            {
                if (session.ExpiryTime < Platform.Time)
                {
                    Platform.Log(LogLevel.Error, "Session ID {0} already expired", session.Id);
                    throw new FaultException<UserAccessDeniedException>(new UserAccessDeniedException());
                }

                session = SessionTokenManager.Instance.UpdateSession(session);

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, "Session ID {0} is updated. Valid until {1}", session.Id, session.ExpiryTime);
                return new ValidateSessionResponse(session, Roles.GetRolesForUser(session.Id));
            }
            Platform.Log(LogLevel.Error, "Session ID {0} does not exist", request.SessionToken.Id);
            throw new FaultException<UserAccessDeniedException>(new UserAccessDeniedException());
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
            throw new FaultException<UserAccessDeniedException>(new UserAccessDeniedException());
        }

        public GetAuthorizationsResponse GetAuthorizations(GetAuthorizationsRequest request)
        {
            string[] authorities = Roles.GetRolesForUser(request.UserName);
            return new GetAuthorizationsResponse(authorities);
        }

        public ResetPasswordResponse ResetPassword(ResetPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}