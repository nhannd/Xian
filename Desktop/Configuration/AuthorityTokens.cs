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

namespace ClearCanvas.Desktop.Configuration
{
	public class AuthorityTokens
	{
		/// <summary>
		/// Tokens that allow access to administrative functionality.
		/// </summary>
		public static class Desktop
		{
			[AuthorityToken(Description = "Allow access to the Settings Management screen.")]
			public const string SettingsManagement = "Desktop/Settings Management";

			[AuthorityToken(Description = "Allow users to customize the Date-Time display format.")]
			public const string CustomizeDateTimeFormat = "Desktop/Customize Date-Time Format";
		}
	}
}
