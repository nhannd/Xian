using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;

namespace ClearCanvas.Enterprise.Core.ServiceModel
{
	/// <summary>
	/// Implemenation of <see cref="IPrincipal"/> that determines role information
	/// for the user via the <see cref="IAuthenticationService"/>.
	/// </summary>
	public class DefaultPrincipal : IPrincipal, IUserCredentialsProvider
	{
		/// <summary>
		/// Creates an object that implements <see cref="IPrincipal"/> based on the specified
		/// identity and session token.
		/// </summary>
		/// <param name="identity"></param>
		/// <param name="sessionToken"></param>
		/// <returns></returns>
		public static IPrincipal CreatePrincipal(IIdentity identity, SessionToken sessionToken)
		{
			return new DefaultPrincipal(identity, sessionToken);
		}


		private readonly IIdentity _identity;
		private readonly SessionToken _sessionToken;
		private string[] _authorityTokens;

		private DefaultPrincipal(IIdentity identity, SessionToken sessionToken)
		{
			_identity = identity;
			_sessionToken = sessionToken;
		}

		public IIdentity Identity
		{
			get { return _identity; }
		}

		public bool IsInRole(string role)
		{
			// initialize auth tokens if this is the first call
			if (_authorityTokens == null)
			{
                //AuthenticationClient authClient = new AuthenticationClient();
                //_authorityTokens = authClient.GetAuthorizations(new GetAuthorizationsRequest(_identity.Name, _sessionToken)).AuthorityTokens;

                Platform.GetService<IAuthenticationService>(
                    delegate(IAuthenticationService service)
                    {
                        _authorityTokens = service.GetAuthorizations(new GetAuthorizationsRequest(_identity.Name, _sessionToken)).AuthorityTokens;
                    });
			}

			// check that the user was granted this token
			return CollectionUtils.Contains(_authorityTokens, delegate(string token) { return token == role; });
		}

		#region IUserCredentialsProvider Members

		string IUserCredentialsProvider.UserName
		{
			get { return _identity.Name; }
		}

		string IUserCredentialsProvider.SessionTokenId
		{
			get { return _sessionToken.Id; }
		}

		#endregion
	}
}
