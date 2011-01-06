#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using System.Security;
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
                SessionToken token = new SessionToken(tokenId.ToString(), Platform.Time + ServerPlatform.WebSessionTimeout);
                string[] authority = Roles.GetRolesForUser(request.UserName);
                string displayName = request.UserName;
                InitiateSessionResponse rsp = new InitiateSessionResponse(token, authority, displayName);

                SessionTokenManager.Instance.AddSession(token);

                return rsp;
            }
            else
            {
                throw new FaultException<UserAccessDeniedException>(new UserAccessDeniedException());
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
                throw new FaultException<UserAccessDeniedException>(new UserAccessDeniedException());
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
                throw new FaultException<UserAccessDeniedException>(new UserAccessDeniedException());
        }

        public GetAuthorizationsResponse GetAuthorizations(GetAuthorizationsRequest request)
        {
            string[] authorities = Roles.GetRolesForUser(request.UserName);
            return new GetAuthorizationsResponse(authorities);
        }

        #endregion
    }
}