using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Desktop.Help
{
	public class AuthorityTokens
	{
		/// <summary>
		/// Tokens that allow access to administrative functionality.
		/// </summary>
		public static class Desktop
		{
			[AuthorityToken(Description = "Allow access to the 'Show Logs' functionality.")]
			public const string ShowLogs = "Desktop/Show Logs";
		}
	}
}