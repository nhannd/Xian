using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Ris.Client
{
	public static class AuthorityTokens
	{
		/// <summary>
		/// Tokens that allow access to development tools and functionality.
		/// </summary>
		public static class Development
		{
			[AuthorityToken(Description = "Allow the homepage to be closed and re-opened.")]
			public const string RestartHomepage = "Development/Restart Homepage";
		}

	}
}
