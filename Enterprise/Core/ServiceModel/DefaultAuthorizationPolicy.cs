#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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