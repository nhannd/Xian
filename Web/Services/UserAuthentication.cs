#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using System.Security.Principal;

namespace ClearCanvas.Web.Services
{
	public class UserSessionInfo
	{
		public UserSessionInfo(IPrincipal principal, SessionToken sessionToken)
		{
			Principal = principal;
			SessionToken = sessionToken;
		}

		public IPrincipal Principal { get; private set; }
		public SessionToken SessionToken { get; private set; }
	}

	public interface IUserAuthentication
	{
		UserSessionInfo Login(string username, string password, string applicationName);
		UserSessionInfo ValidateSession(string username, string sessionId);
		SessionToken RenewSession(string username, SessionToken sessionToken);
		void Logout(string username, SessionToken sessionToken);
	}

	public class UserAuthenticationExtensionPoint : ExtensionPoint<IUserAuthentication>
	{
	}
	
	static class UserAuthentication
    {
		private class Default : IUserAuthentication
		{
			#region IUserAuthentication Members

			public UserSessionInfo Login(string username, string password, string applicationName)
			{
				return null;
			}

			public UserSessionInfo ValidateSession(string username, string sessionId)
			{
				return null;
			}

			public SessionToken RenewSession(string username, SessionToken sessionToken)
			{
				return null;
			}

			public void Logout(string username, SessionToken sessionToken)
			{
			}

			#endregion
		}

		private static readonly IUserAuthentication _instance;
		private static readonly bool _logStuff;

		static UserAuthentication()
		{
			try
			{
				_instance = (IUserAuthentication)new UserAuthenticationExtensionPoint().CreateExtension();
				_logStuff = true;
				return;
			}
			catch (NotSupportedException)
			{
			}

			_instance = new Default();
		}

		//TODO (CR May 2010): remove the console calls, use the formatting in Platform.Log


		static public UserSessionInfo Login(string username, string password, string applicationName)
		{
			if (_logStuff)
			{
				string message = String.Format("Attempting to log in (username={0}, appname={1}).", username, applicationName);
				Console.WriteLine(message);
				Platform.Log(LogLevel.Info, message);
			}

			return _instance.Login(username, password, applicationName);
		}

		static public UserSessionInfo ValidateSession(string username, string sessionId)
		{
			if (_logStuff)
			{
				string message = String.Format("Validating user session (username={0}, session id={1}).", username, sessionId);
				//Console.WriteLine(message);
				Platform.Log(LogLevel.Debug, message);
			}

			return _instance.ValidateSession(username, sessionId);
		}

		static public SessionToken RenewSession(string username, SessionToken sessionToken)
		{
			if (_logStuff)
			{
				string message = String.Format("Renewing user session (username={0}, session={1}).", username, sessionToken.Id);
				//Console.WriteLine(message);
				Platform.Log(LogLevel.Debug, message);
			}

			return _instance.RenewSession(username, sessionToken);
		}

		static public void Logout(string username, SessionToken sessionToken)
		{
			if (_logStuff)
			{
				string message = String.Format("Attempting to log out (username={0}, session={1}).", username, sessionToken.Id);
				Console.WriteLine(message);
			}

			try
			{
				_instance.Logout(username, sessionToken);
				if (_logStuff)
					Platform.Log(LogLevel.Info, "Successfully logged out (username={0}, session={1}).", username, sessionToken.Id);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Failed to log out (user={0}, session={1}).", username, sessionToken.Id);
			}
		}
	}
}