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
        public SessionToken InitiateUserSession(string userName, string password)
        {
            Platform.CheckForNullReference(userName, "userName");
            Platform.CheckForNullReference(password, "password");

            DateTime currentTime = Platform.Time;

            // check user account is valid and password is correct
            User user = GetVerifiedUser(userName, password);

            // check if password expired
            if(user.Password.IsExpired)
                throw new PasswordExpiredException();

            // update last login time
            user.LastLoginTime = currentTime;

            UserSession session;
            bool newSession = false;
            try
            {
                // try to find an existing session object for this user
                UserSessionSearchCriteria criteria = new UserSessionSearchCriteria();
                criteria.UserName.EqualTo(userName);
                session = PersistenceContext.GetBroker<IUserSessionBroker>().FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                // no existing session - create a new one
                session = new UserSession(userName);
                newSession = true;
            }

            // generate a new session id
            session.SessionId = Guid.NewGuid().ToString("N");

            // set the expiration time
            GlobalSettings settings = new GlobalSettings();
            session.ExpiryTime = currentTime.AddMinutes(settings.UserSessionTimeoutMinutes);

            if(newSession)
                PersistenceContext.Lock(session, DirtyState.New);

            return session.GetToken();
        }

        [UpdateOperation(Auditable = false)]
        public void ChangePassword(string userName, string currentPassword, string newPassword)
        {
            Platform.CheckForNullReference(userName, "userName");
            Platform.CheckForNullReference(currentPassword, "currentPassword");
            Platform.CheckForNullReference(newPassword, "newPassword");

            // this will fail if the currentPassword is not valid, the account is not active or whatever
            User user = GetVerifiedUser(userName, currentPassword);

            // change the password
            user.ChangePassword(newPassword);
        }

        [UpdateOperation(Auditable = false)]
        public SessionToken ValidateUserSession(string userName, SessionToken sessionToken)
        {
            //TODO: this method could be implemented using a direct SQL UPDATE...WHERE... statement for improved performance

            DateTime currentTime = Platform.Time;

            UserSession session;
            try
            {
                // find a session object for this user and session token that has not yet expired
                UserSessionSearchCriteria criteria = new UserSessionSearchCriteria();
                criteria.UserName.EqualTo(userName);
                criteria.SessionId.EqualTo(sessionToken.Id);
                criteria.ExpiryTime.MoreThan(currentTime);
                session = PersistenceContext.GetBroker<IUserSessionBroker>().FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                // no session, or session has expired
                throw new SecurityTokenValidationException(SR.ExceptionInvalidSession);
            }

            // renew the expiration time
            GlobalSettings settings = new GlobalSettings();
            session.ExpiryTime = currentTime.AddMinutes(settings.UserSessionTimeoutMinutes);

            return session.GetToken();
        }

        [UpdateOperation(Auditable = false)]
        public void TerminateUserSession(string userName, SessionToken sessionToken)
        {
            try
            {
                // find a session object for this user and session token
                UserSessionSearchCriteria criteria = new UserSessionSearchCriteria();
                criteria.UserName.EqualTo(userName);
                criteria.SessionId.EqualTo(sessionToken.Id);
                IUserSessionBroker broker = PersistenceContext.GetBroker<IUserSessionBroker>();
                UserSession session = broker.FindOne(criteria);

                // delete the session record
                broker.Delete(session);
            }
            catch (EntityNotFoundException)
            {
                // no such session
                throw new SecurityTokenValidationException(SR.ExceptionInvalidSession);
            }
        }


        [ReadOperation]
        public string[] ListAuthorityTokensForUser(string userName)
        {
            return PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindTokensByUserName(userName);
        }

        [ReadOperation]
        public bool AssertTokenForUser(string userName, string token)
        {
            return PersistenceContext.GetBroker<IAuthorityTokenBroker>().AssertUserHasToken(userName, token);
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
            User user;
            try
            {
                UserSearchCriteria criteria = new UserSearchCriteria();
                criteria.UserName.EqualTo(userName);
                user = PersistenceContext.GetBroker<IUserBroker>().FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                // non-existant username
                // the error message is deliberately vague
                throw new SecurityTokenValidationException(SR.ExceptionInvalidUserAccount);
            }

            if (!user.IsActive || !user.Password.Verify(password))
            {
                // account not active, or invalid password
                // the error message is deliberately vague
                throw new SecurityTokenValidationException(SR.ExceptionInvalidUserAccount);
            }
            return user;
        }

    }
}
