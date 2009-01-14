#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Text;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Authentication
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IAuthenticationService))]
    public class AuthenticationService : CoreServiceLayer, IAuthenticationService
    {
        #region IAuthenticationService Members

    	[UpdateOperation(Auditable = false)]
		public InitiateSessionResponse InitiateSession(InitiateSessionRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.Password, "Password");

            DateTime currentTime = Platform.Time;

            // check user account is valid and password is correct
            User user = GetVerifiedUser(request.UserName, request.Password);

            // check if password expired
            if(user.Password.IsExpired)
                throw new PasswordExpiredException();

            // update last login time
            user.LastLoginTime = currentTime;

            UserSession session = user.CurrentSession;
            if (session == null)
            {
                session = new UserSession();
                user.CurrentSession = session;
            }

            // generate a new session id
            session.SessionId = Guid.NewGuid().ToString("N");

            // set the expiration time
            AuthenticationSettings settings = new AuthenticationSettings();
            session.ExpiryTime = currentTime.AddMinutes(settings.UserSessionTimeoutMinutes);

            return new InitiateSessionResponse(session.GetToken());
        }

        [UpdateOperation(Auditable = false)]
		public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.CurrentPassword, "CurrentPassword");
			Platform.CheckMemberIsSet(request.NewPassword, "NewPassword");

            // this will fail if the currentPassword is not valid, the account is not active or whatever
            User user = GetVerifiedUser(request.UserName, request.CurrentPassword);

            // change the password
            user.ChangePassword(request.NewPassword);

			return new ChangePasswordResponse();
        }

        [UpdateOperation(Auditable = false)]
		public ValidateSessionResponse ValidateSession(ValidateSessionRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.SessionToken, "SessionToken");
			Platform.CheckMemberIsSet(request.SessionToken.Id, "SessionToken.Id");

            AuthenticationSettings settings = new AuthenticationSettings();
            DateTime currentTime = Platform.Time;

            User user = GetVerifiedUser(request.UserName, request.SessionToken);
            UserSession session = user.CurrentSession;

            // if session timeouts are enabled, check expiry time
            if (settings.UserSessionTimeoutEnabled && session.ExpiryTime < currentTime)
            {
                // session has expired
                // the error message is deliberately vague
                throw new SecurityTokenValidationException(SR.ExceptionInvalidSession);
            }

            // renew the expiration time
            session.ExpiryTime = currentTime.AddMinutes(settings.UserSessionTimeoutMinutes);

            return new ValidateSessionResponse(session.GetToken());
        }

        [UpdateOperation(Auditable = false)]
		public TerminateSessionResponse TerminateSession(TerminateSessionRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.SessionToken, "SessionToken");
			Platform.CheckMemberIsSet(request.SessionToken.Id, "SessionToken.Id");

			User user = GetVerifiedUser(request.UserName, request.SessionToken);

            // set the current session to null
            UserSession session = user.CurrentSession;
            user.CurrentSession = null;

            // delete the session object
            IUserSessionBroker broker = PersistenceContext.GetBroker<IUserSessionBroker>();
            broker.Delete(session);

			return new TerminateSessionResponse();
        }


        [ReadOperation]
		public GetAuthorizationsResponse GetAuthorizations(GetAuthorizationsRequest request)
        {
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");

			string[] tokens = PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindTokensByUserName(request.UserName);

			return new GetAuthorizationsResponse(tokens);
        }

        #endregion

        /// <summary>
        /// Obtains user object for specified username and password.  Verifies that the user account
        /// is active and that the password is correct.  Does NOT check if the password has expired.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private User GetVerifiedUser(string userName, string password)
        {
            User user = GetUserByUserName(userName);

            if (!user.IsActive || !user.Password.Verify(password))
            {
                // account not active, or invalid password
                // the error message is deliberately vague
                throw new SecurityTokenValidationException(SR.ExceptionInvalidUserAccount);
            }
            return user;
        }

        /// <summary>
        /// Gets the user specified by the user name, verifying that the supplied session token
        /// matches the user.  If the session token does not match the user, and exception is thrown.
        /// </summary>
        /// <remarks>
        /// This method does not check if the session has expired.
        /// </remarks>
        /// <param name="userName"></param>
        /// <param name="sessionToken"></param>
        /// <returns></returns>
        private User GetVerifiedUser(string userName, SessionToken sessionToken)
        {
            User user = GetUserByUserName(userName);

            if (!user.IsActive || user.CurrentSession == null || user.CurrentSession.SessionId != sessionToken.Id)
            {
                // account not active, or invalid session token
                // the error message is deliberately vague
                throw new SecurityTokenValidationException(SR.ExceptionInvalidUserAccount);
            }
            return user;
        }

        /// <summary>
        /// Gets the user specified by the user name, or throws an exception if no such user exists.
        /// </summary>
        /// <remarks>
        /// This method does not check the validity of the user account.
        /// </remarks>
        /// <param name="userName"></param>
        /// <returns></returns>
        private User GetUserByUserName(string userName)
        {
            UserSearchCriteria criteria = new UserSearchCriteria();
            criteria.UserName.EqualTo(userName);

            // use query caching here to make this fast (assuming the user table is not often updated)
            IList<User> users = PersistenceContext.GetBroker<IUserBroker>().Find(
                new UserSearchCriteria[]{criteria}, new SearchResultPage(0, 1), true);

            User user = CollectionUtils.FirstElement(users);
            if(user == null)
            {
                // non-existant username
                // the error message is deliberately vague
                throw new SecurityTokenValidationException(SR.ExceptionInvalidUserAccount);
            }
            return user;
        }

    }
}
