using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security.Principal;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.ServiceModel
{
    /// <summary>
    /// Implementation of <see cref="IAuthorizationPolicy"/> that establishes
    /// an instance of <see cref="IPrincipal"/> that uses the <see cref="IAuthenticationService"/>
    /// to determine authorization.
    /// </summary>
    class DefaultAuthorizationPolicy : IAuthorizationPolicy
    {

        #region IPrincipal implementation

        /// <summary>
        /// Implemenation of <see cref="IPrincipal"/> that determines role information
        /// for the user via the <see cref="IAuthenticationService"/>.
        /// </summary>
        class DefaultPrincipal : IPrincipal
        {
            private readonly IIdentity _identity;
            private string[] _authorityTokens;

            public DefaultPrincipal(IIdentity identity)
            {
                _identity = identity;
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
                    Platform.GetService<IAuthenticationService>(
                        delegate(IAuthenticationService service)
                        {
                            // TODO: we are supposed to pass session token here but we don't have access to it
                            _authorityTokens = service.GetAuthorizations(new GetAuthorizationsRequest(_identity.Name, null)).AuthorityTokens;
                        });
                }

                // check that the user was granted this token
                return CollectionUtils.Contains(_authorityTokens, delegate(string token) { return token == role; });
            }
        }

        #endregion

        string id = Guid.NewGuid().ToString();

		public string Id
		{
			get { return this.id; }
		}

		public ClaimSet Issuer
		{
			get { return ClaimSet.System; }
		}

		public bool Evaluate(EvaluationContext context, ref object state)
		{
			object obj;
			if (!context.Properties.TryGetValue("Identities", out obj))
				return false;

			IList<IIdentity> identities = obj as IList<IIdentity>;
			if (obj == null || identities.Count <= 0)
				return false;

            IIdentity clientIdentity = identities[0];
            context.Properties["Principal"] = new DefaultPrincipal(clientIdentity);

			return true;
		}
	}
}