#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.Desktop.ExtensionBrowser
{
	public class AuthorityTokens
	{
		/// <summary>
		/// Tokens that allow access to administrative functionality.
		/// </summary>
		public static class Desktop
		{
			[AuthorityToken(Description = "Allow access to the desktop Extension Browser.")]
			public const string ExtensionBrowser = "Desktop/Extension Browser";
		}
	}
}
