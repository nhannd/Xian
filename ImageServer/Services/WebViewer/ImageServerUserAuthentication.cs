#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Principal;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.Web.Services;
using System.Web;
using System.Web.Caching;

namespace ClearCanvas.ImageServer.Services.WebViewer
{
	[ExtensionOf(typeof(UserAuthenticationExtensionPoint), Enabled = false)]
	public class EnterpriseUserAuthentication : IUserAuthentication
	{
		#region Enterprise session cache hack

		private static readonly Cache _cache = HttpRuntime.Cache;
		
		private static string GetLoggedOutSessionKey(string sessionId)
		{
			if (String.IsNullOrEmpty(sessionId))
				return null;

			return "LoggedOutSession: " + sessionId;
		}

		private static void AddLoggedOutSession(string sessionId)
		{
			if (String.IsNullOrEmpty(sessionId))
				return;

			string loggedOutSessionKey = GetLoggedOutSessionKey(sessionId);
			if (loggedOutSessionKey == null)
				return;

			//keep sessions we know are logged out around for a while
			_cache.Add(loggedOutSessionKey, sessionId, null, Platform.Time.AddMinutes(20),
				Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
		}

		private static void CheckSessionNotLoggedOut(string sessionId)
		{
			if (null != _cache.Get(GetLoggedOutSessionKey(sessionId)))
				throw new InvalidUserSessionException();
		}

		#endregion

		private static UserSessionInfo CreateUserSessionInfo(string username, SessionInfo sessionInfo)
		{
			IIdentity identity = new GenericIdentity(username);
			IPrincipal principal = DefaultPrincipal.CreatePrincipal(identity,
				sessionInfo.Credentials.SessionToken, sessionInfo.Credentials.Authorities ?? new string[0]);

			if (!principal.IsInRole(ImageViewer.AuthorityTokens.ViewerVisible))
				throw new UserAccessDeniedException();

			return new UserSessionInfo(principal, sessionInfo.Credentials.SessionToken);
		}

		public UserSessionInfo Login(string username, string password, string applicationName)
		{
			if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
				throw new ArgumentException("The supplied user credentials are invalid.");

            try
            {
                using (LoginService service = new LoginService())
                {
                    SessionInfo sessionInfo = service.Login(username, password, applicationName);
                    return CreateUserSessionInfo(username, sessionInfo);
                }
            }
            catch (SessionValidationException ex)
            {
                throw ex.InnerException;
            }
		}

		public UserSessionInfo ValidateSession(string username, string sessionId)
        {
			if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(sessionId))
				throw new ArgumentException("The supplied session information is invalid.");

			CheckSessionNotLoggedOut(sessionId);

			try
            {
                using (LoginService service = new LoginService())
                {
                    SessionInfo sessionInfo = new SessionInfo(username, username, new SessionToken(sessionId));
                    service.Validate(sessionInfo);
                    return CreateUserSessionInfo(username, sessionInfo);
                }
            }
            catch(SessionValidationException ex)
            {
                throw ex.InnerException;
            }
        }

		public SessionToken RenewSession(string username, SessionToken sessionToken)
		{
			if (String.IsNullOrEmpty(username) || sessionToken == null || String.IsNullOrEmpty(sessionToken.Id))
				throw new ArgumentException("The supplied session information is invalid.");

			CheckSessionNotLoggedOut(sessionToken.Id);

            try
            {
                using (LoginService service = new LoginService())
                {
                    SessionInfo sessionInfo = new SessionInfo(username, username, new SessionToken(sessionToken.Id));
                    service.Validate(sessionInfo);
                    return sessionInfo.Credentials.SessionToken;
                }
            }
            catch (SessionValidationException ex)
            {
                throw ex.InnerException;
            }
		}

		public void Logout(string username, SessionToken sessionToken)
		{
			if (String.IsNullOrEmpty(username) || sessionToken == null || String.IsNullOrEmpty(sessionToken.Id))
				throw new ArgumentException("The supplied session information is invalid.");

			AddLoggedOutSession(sessionToken.Id);

            try
            {
                using (LoginService service = new LoginService())
                {
                    service.Logout(new SessionInfo(username, username, sessionToken));
                }
            }
            catch (SessionValidationException ex)
            {
                throw ex.InnerException;
            }
		}
	}

	[ExtensionOf(typeof(UserAuthenticationExtensionPoint), Enabled = true)]
	public class ImageServerDefaultUserAuthentication : IUserAuthentication
    {
		private static UserSessionInfo CreateUserSessionInfo(string userName)
		{
			return String.IsNullOrEmpty(userName) ? null : new UserSessionInfo(new GenericPrincipal(new GenericIdentity(userName), new string[0]), null);
		}

		#region IUserAuthentication Members

		public UserSessionInfo Login(string username, string password, string applicationName)
		{
			return CreateUserSessionInfo(username);
		}

		public UserSessionInfo ValidateSession(string username, string sessionId)
		{
			return CreateUserSessionInfo(username);
		}

		public SessionToken RenewSession(string username, SessionToken sessionToken)
		{
			return new SessionToken(sessionToken.Id, Platform.Time + ServerPlatform.WebSessionTimeout);
		}

		public void Logout(string username, SessionToken sessionToken)
		{
		}

		#endregion
	}
}