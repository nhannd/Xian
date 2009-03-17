using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClearCanvas.ImageViewer.Common
{
	public static class PermissionsHelper
	{
		public static bool IsInRole(string authorityToken)
		{
			return IsInRoles(authorityToken);
		}

		public static bool IsInRoles(params string[] authorityTokens)
		{
			if (authorityTokens == null)
				return true;

			return IsInRoles((IEnumerable<string>) authorityTokens);
		}

		public static bool IsInRoles(IEnumerable<string> authorityTokens)
		{
			if (Thread.CurrentPrincipal == null || !Thread.CurrentPrincipal.Identity.IsAuthenticated)
				return true;

			foreach (string authorityToken in authorityTokens)
			{
				if (!Thread.CurrentPrincipal.IsInRole(authorityToken))
					return false;
			}

			return true;
		}
	}
}
