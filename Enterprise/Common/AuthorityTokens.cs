#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Enterprise.Common
{
    public class AuthorityTokens
    {
		/// <summary>
		/// Tokens that allow access to administrative functionality.
		/// </summary>
		public static class Admin
		{
			public static class System
			{
				[AuthorityToken(Description = "Allow modification of enterprise configuration store data.")]
				public const string Configuration = "Enterprise/Admin/System/Configuration";
			}

			public static class Security
			{
				[AuthorityToken(Description = "Allow administration of User Accounts.")]
                public const string User = "Enterprise/Admin/Security/User";

				[AuthorityToken(Description = "Allow administration of Authority Groups.")]
                public const string AuthorityGroup = "Enterprise/Admin/Security/Authority Group";
			}
		}
	}
}
