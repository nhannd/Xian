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
using System.Security.Principal;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Authentication;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Implemenation of <see cref="IPrincipal"/> that determines role information
	/// for the user via the <see cref="IAuthenticationService"/>.
	/// </summary>
	public class DefaultPrincipal : IPrincipal, IUserCredentialsProvider
	{
		/// <summary>
		/// Creates an object that implements <see cref="IPrincipal"/> based on the specified
		/// identity and session token.  The authorizations will be automatically obtained
        /// via the <see cref="IAuthenticationService"/>.
		/// </summary>
		/// <param name="identity"></param>
		/// <param name="sessionToken"></param>
		/// <returns></returns>
		public static IPrincipal CreatePrincipal(IIdentity identity, SessionToken sessionToken)
		{
			return new DefaultPrincipal(identity, sessionToken, null);
		}

        /// <summary>
        /// Creates an object that implements <see cref="IPrincipal"/> based on the specified
        /// identity, session token, and authorizations.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="sessionToken"></param>
        /// <returns></returns>
        public static IPrincipal CreatePrincipal(IIdentity identity, SessionToken sessionToken, string[] authorityTokens)
        {
            return new DefaultPrincipal(identity, sessionToken, authorityTokens);
        }


		private readonly IIdentity _identity;
		private readonly SessionToken _sessionToken;
		private string[] _authorityTokens;

		private DefaultPrincipal(IIdentity identity, SessionToken sessionToken, string[] authorityTokens)
		{
			_identity = identity;
			_sessionToken = sessionToken;
            _authorityTokens = authorityTokens;
		}

		public IIdentity Identity
		{
			get { return _identity; }
		}

        public SessionToken SessionToken
        {
            get { return _sessionToken; }
        }

		public bool IsInRole(string role)
		{
			// initialize auth tokens if not yet initialized
			if (_authorityTokens == null)
			{
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
