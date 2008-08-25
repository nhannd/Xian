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
		public static class Admin
		{
			public static class System
			{
				[AuthorityToken(Description = "Allow access to the desktop Extension Browser.")]
				public const string ExtensionBrowser = "Admin/System/Extension Browser";
			}
		}
	}
}
