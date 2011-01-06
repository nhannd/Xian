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
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security.Principal;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core.ServiceModel
{
    /// <summary>
    /// Implementation of <see cref="IAuthorizationPolicy"/> that establishes
    /// an instance of <see cref="IPrincipal"/> that uses the <see cref="IAuthenticationService"/>
    /// to determine authorization.
    /// </summary>
    public class DefaultAuthorizationPolicy : IAuthorizationPolicy
    {
    	private readonly string _id = Guid.NewGuid().ToString();
    	private readonly string _userName;
		private readonly SessionToken _sessionToken;

		public DefaultAuthorizationPolicy(string userName, SessionToken sessionToken)
		{
			_userName = userName;
			_sessionToken = sessionToken;
		}

		public string Id
		{
			get { return _id; }
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
			if (obj == null)
				return false;

			// find the matching identity
			IIdentity clientIdentity = CollectionUtils.SelectFirst(identities,
				delegate(IIdentity i) { return i.Name == _userName; });

			if (clientIdentity == null)
				return false;

			// set the principal
            context.Properties["Principal"] = DefaultPrincipal.CreatePrincipal(clientIdentity, _sessionToken);

			return true;
		}
	}
}