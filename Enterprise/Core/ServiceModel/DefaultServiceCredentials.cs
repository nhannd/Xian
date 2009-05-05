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
