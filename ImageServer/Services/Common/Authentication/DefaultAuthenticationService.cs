#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Configuration;
using System.Security;
using System.ServiceModel;
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
                SessionToken token = new SessionToken(request.UserName, Platform.Time.AddMinutes(Int32.Parse(ConfigurationManager.AppSettings["SessionTimeout"])));
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