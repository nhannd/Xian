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
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security;
using System.Text;
using System.ServiceModel.Description;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core.ServiceModel
{
	class DefaultServiceCredentials : ServiceCredentials
	{

		#region Helper classes

		class CustomSecurityTokenManager : ServiceCredentialsSecurityTokenManager
		{
			public CustomSecurityTokenManager(ServiceCredentials parent)
				: base(parent)
			{
			}

			public override SecurityTokenAuthenticator CreateSecurityTokenAuthenticator(System.IdentityModel.Selectors.SecurityTokenRequirement tokenRequirement, out System.IdentityModel.Selectors.SecurityTokenResolver outOfBandTokenResolver)
			{
				if (tokenRequirement.TokenType == SecurityTokenTypes.UserName)
				{
					outOfBandTokenResolver = null;

					// Get the current validator
					UserNamePasswordValidator validator = ServiceCredentials.UserNameAuthentication.CustomUserNamePasswordValidator;
					return new CustomSecurityTokenAuthenticator(validator);

				}

				return base.CreateSecurityTokenAuthenticator(tokenRequirement, out outOfBandTokenResolver);

			}

			public override System.IdentityModel.Selectors.SecurityTokenProvider CreateSecurityTokenProvider(System.IdentityModel.Selectors.SecurityTokenRequirement requirement)
			{
				return base.CreateSecurityTokenProvider(requirement);
			}

			public override System.IdentityModel.Selectors.SecurityTokenSerializer CreateSecurityTokenSerializer(System.IdentityModel.Selectors.SecurityTokenVersion version)
			{
				return base.CreateSecurityTokenSerializer(version);
			}
		}

		class CustomSecurityTokenAuthenticator : CustomUserNameSecurityTokenAuthenticator
		{
			public CustomSecurityTokenAuthenticator(UserNamePasswordValidator validator)
				:base(validator)
			{
			}

			protected override ReadOnlyCollection<IAuthorizationPolicy> ValidateUserNamePasswordCore(String userName, String password)
			{
				ReadOnlyCollection<IAuthorizationPolicy> currentPolicies = base.ValidateUserNamePasswordCore(userName, password);

				List<IAuthorizationPolicy> newPolicies = new List<IAuthorizationPolicy>(currentPolicies);

				// the "password" is actually the session token ID
				newPolicies.Add(new DefaultAuthorizationPolicy(userName, new SessionToken(password)));
				return newPolicies.AsReadOnly();
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public DefaultServiceCredentials()
		{
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="source"></param>
		private DefaultServiceCredentials(DefaultServiceCredentials source)
			:base(source)
		{
		}

		#region Overrides

		public override SecurityTokenManager CreateSecurityTokenManager()
		{
			// Check if the current validation mode is for custom username password validation
			if (UserNameAuthentication.UserNamePasswordValidationMode == UserNamePasswordValidationMode.Custom)
			{
				return new CustomSecurityTokenManager(this);
			}

			return base.CreateSecurityTokenManager();
		}

		protected override ServiceCredentials CloneCore()
		{
			return new DefaultServiceCredentials(this);
		}

		#endregion
	}
}
