using System.Security.Principal;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageViewer.EnterpriseDesktop
{
	public class Session
	{
		private Session(string userName, string displayName, string[] authorityTokens, SessionToken sessionToken)
		{
			this.Principal = new GenericPrincipal(new GenericIdentity(userName), authorityTokens);
			this.DisplayName = displayName;
			this.Token = sessionToken;
		}

		public readonly GenericPrincipal Principal;
		public readonly string DisplayName;
		public SessionToken Token;

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
