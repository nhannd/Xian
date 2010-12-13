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
using ClearCanvas.Web.Services;

namespace ClearCanvas.ImageServer.Services.WebViewer
{
	[ExtensionOf(typeof(UserAuthenticationExtensionPoint))]
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