#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Text.RegularExpressions;
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
		private AuthenticationSettings _settings;


		#region IAuthenticationService Members

		[UpdateOperation(ChangeSetAuditable = false)]
		public InitiateSessionResponse InitiateSession(InitiateSessionRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.Application, "Application");
			Platform.CheckMemberIsSet(request.HostName, "HostName");
			Platform.CheckMemberIsSet(request.Password, "Password");

			// check host name against white-list
			if (!CheckWhiteList(this.Settings.HostNameWhiteList, request.HostName))
				throw new UserAccessDeniedException();

			// check application name against white-list
			if (!CheckWhiteList(this.Settings.ApplicationWhiteList, request.Application))
				throw new UserAccessDeniedException();


			// find user
			var user = GetUser(request.UserName);
			if (user == null)
				throw new UserAccessDeniedException();

			// clean-up any expired sessions
			CleanExpiredSessions(user);

			// initiate new session
			var session = user.InitiateSession(request.Application, request.HostName, request.Password, GetSessionTimeout());

			// get authority tokens if requested
			var authorizations = request.GetAuthorizations ?
				PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindTokensByUserName(request.UserName) : new string[0];

			return new InitiateSessionResponse(session.GetToken(), authorizations, user.DisplayName);
		}

		[UpdateOperation(ChangeSetAuditable = false)]
		public ChangePasswordResponse ChangePassword(ChangePasswordRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.CurrentPassword, "CurrentPassword");
			Platform.CheckMemberIsSet(request.NewPassword, "NewPassword");

			var now = Platform.Time;
			var user = GetUser(request.UserName);

			// ensure user found, account is active and the current password is correct
			if (user == null || !user.IsActive(now) || !user.Password.Verify(request.CurrentPassword))
			{
				// no such user, account not active, or invalid password
				// the error message is deliberately vague
				throw new UserAccessDeniedException();
			}

			// check new password meets policy
			if (!Regex.Match(request.NewPassword, this.Settings.ValidPasswordRegex).Success)
				throw new RequestValidationException(this.Settings.ValidPasswordMessage);

			var expiryTime = Platform.Time.AddDays(this.Settings.PasswordExpiryDays);

			// change the password
			user.ChangePassword(request.NewPassword, expiryTime);

			return new ChangePasswordResponse();
		}

		[UpdateOperation(ChangeSetAuditable = false)]
		[ResponseCaching("GetSessionTokenCacheDirective")]
		public ValidateSessionResponse ValidateSession(ValidateSessionRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.SessionToken, "SessionToken");
			Platform.CheckMemberIsSet(request.SessionToken.Id, "SessionToken.Id");

			// get the session
			var session = GetSession(request.SessionToken);
			if (session == null)
				throw new InvalidUserSessionException();

			// determine if still valid
			session.Validate(request.UserName, this.Settings.UserSessionTimeoutEnabled);

			// renew
			session.Renew(GetSessionTimeout());

			// get authority tokens if requested
			var authorizations = request.GetAuthorizations ?
				PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindTokensByUserName(request.UserName) : new string[0];

			return new ValidateSessionResponse(session.GetToken(), authorizations);
		}

		[UpdateOperation(ChangeSetAuditable = false)]
		public TerminateSessionResponse TerminateSession(TerminateSessionRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");
			Platform.CheckMemberIsSet(request.SessionToken, "SessionToken");
			Platform.CheckMemberIsSet(request.SessionToken.Id, "SessionToken.Id");

			// get the session and user
			var session = GetSession(request.SessionToken);
			if (session == null)
				throw new InvalidUserSessionException();

			var user = session.User;

			// validate the session, ignoring the expiry time
			session.Validate(request.UserName, false);

			// terminate it
			session.Terminate();

			// delete the session object
			var broker = PersistenceContext.GetBroker<IUserSessionBroker>();
			broker.Delete(session);

			// while we're at it, clean-up any other expired sessions for that user
			CleanExpiredSessions(user);

			return new TerminateSessionResponse();
		}


		[ReadOperation]
		[ResponseCaching("GetAuthorityTokenCacheDirective")]
		public GetAuthorizationsResponse GetAuthorizations(GetAuthorizationsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.UserName, "UserName");

			//TODO: ideally we should validate the username and session token and check session expiry
			//this would ensure that only a user with a valid session could obtain his authorizations,
			//however, there is an issue in the RIS right now that prevents the session token from being passed
			// in the request... this is a WCF architecture question that needs to be resolved

			var tokens = PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindTokensByUserName(request.UserName);

			return new GetAuthorizationsResponse(tokens);
		}

		#endregion

		/// <summary>
		/// Gets the user specified by the user name, or null if no such user exists.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		private User GetUser(string userName)
		{
			var criteria = new UserSearchCriteria();
			criteria.UserName.EqualTo(userName);

			// use query caching here to make this fast (assuming the user table is not often updated)
			var users = PersistenceContext.GetBroker<IUserBroker>().Find(
				criteria, new SearchResultPage(0, 1), new EntityFindOptions { Cache = true });

			// bug #3701: to ensure the username match is case-sensitive, we need to compare the stored name to the supplied name
			// returns null if no match
			return CollectionUtils.SelectFirst(users, u => u.UserName == userName);
		}

		/// <summary>
		/// Gets the session identified by the specified session token, or null if no session exists.
		/// </summary>
		/// <param name="sessionToken"></param>
		/// <returns></returns>
		private UserSession GetSession(SessionToken sessionToken)
		{
			var where = new UserSessionSearchCriteria();
			where.SessionId.EqualTo(sessionToken.Id);

			// use query caching here to hopefully speed this up a bit
			var sessions = PersistenceContext.GetBroker<IUserSessionBroker>().Find(
				where, new SearchResultPage(0, 1), new EntityFindOptions { Cache = true });

			// ensure case-sensitive match, returns null if no match
			return CollectionUtils.SelectFirst(sessions, s => s.SessionId == sessionToken.Id);
		}

		/// <summary>
		/// Perform clean-up of any expired sessions that may be left over for the specified user.
		/// </summary>
		/// <param name="user"></param>
		private void CleanExpiredSessions(User user)
		{
			var expiredSessions = user.TerminateExpiredSessions();

			// delete the session objects
			var broker = PersistenceContext.GetBroker<IUserSessionBroker>();
			foreach (var session in expiredSessions)
			{
				broker.Delete(session);
			}
		}

		/// <summary>
		/// Asserts that the specified value is contained in the specified list.
		/// </summary>
		/// <param name="commaDelimitedList"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool CheckWhiteList(string commaDelimitedList, string value)
		{
			if (commaDelimitedList == null)
				return true;

			value = StringUtilities.EmptyIfNull(value).Trim();

			var items = CollectionUtils.Map(
				commaDelimitedList.Trim().Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries),
				(string s) => s.Trim());

			return items.Count == 0 || CollectionUtils.Contains(items,
				s => s.Equals(value, StringComparison.InvariantCultureIgnoreCase));
		}

		/// <summary>
		/// Gets the user session timeout from settings.
		/// </summary>
		/// <returns></returns>
		private TimeSpan GetSessionTimeout()
		{
			return TimeSpan.FromMinutes(this.Settings.UserSessionTimeoutMinutes);
		}

		/// <summary>
		/// Gets the session token response caching directive.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private ResponseCachingDirective GetSessionTokenCacheDirective(object request)
		{
			return new ResponseCachingDirective(
				this.Settings.SessionTokenCachingEnabled,
				TimeSpan.FromSeconds(this.Settings.SessionTokenCachingTimeToLiveSeconds),
				ResponseCachingSite.Client);
		}

		/// <summary>
		/// Gets the authority token response caching directive.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private ResponseCachingDirective GetAuthorityTokenCacheDirective(object request)
		{
			return new ResponseCachingDirective(
				this.Settings.AuthorityTokenCachingEnabled,
				TimeSpan.FromSeconds(this.Settings.AuthorityTokenCachingTimeToLiveSeconds),
				ResponseCachingSite.Client);
		}

		/// <summary>
		/// Gets an instance of the settings.  The instance is loaded on demand
		/// once per instance of this service class.
		/// </summary>
		private AuthenticationSettings Settings
		{
			get
			{
				if (_settings == null)
				{
					_settings = new AuthenticationSettings();
				}
				return _settings;
			}
		}


	}
}
