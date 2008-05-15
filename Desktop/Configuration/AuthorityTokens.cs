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
		public static class Admin
		{
			public static class System
			{
				[AuthorityToken(Description = "Allow access to the Settings Management screen.")]
				public const string SettingsManagement = "Admin/System/Settings Management";
			}
		}
	}
}
