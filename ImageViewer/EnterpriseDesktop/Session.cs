#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Security.Principal;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageViewer.EnterpriseDesktop
{
	public class Session
	{
		private Session(string userName, string displayName, string[] authorityTokens, SessionToken sessionToken)
		{
			IIdentity identity = new GenericIdentity(userName);
			this.Principal = DefaultPrincipal.CreatePrincipal(identity, sessionToken, authorityTokens);
			this.DisplayName = displayName;
			this.Token = sessionToken;
		}

		public readonly IPrincipal Principal;
		public readonly string DisplayName;
		public readonly SessionToken Token;

		private static Session _current;

		internal static void Create(string userName, string displayName, string[] authorityTokens, SessionToken sessionToken)
		{
			_current = new Session(userName, displayName, authorityTokens, sessionToken);
		}

		public static Session Current
		{
			get { return _current; }
		}
	}
}
