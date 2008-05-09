using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;

namespace ClearCanvas.Ris.Server
{
	class WSHttpConfiguration : IServiceConfiguration
	{
		private const int OneMegaByte = 1048576;

		#region IServiceConfiguration Members

		public void ConfigureServiceHost(ServiceHost host, Type serviceContract, Uri hostUri, bool authenticated)
		{
			WSHttpBinding binding = new WSHttpBinding();
			binding.MaxReceivedMessageSize = OneMegaByte;
			binding.ReaderQuotas.MaxStringContentLength = OneMegaByte;
			binding.ReaderQuotas.MaxArrayLength = OneMegaByte;
			binding.Security.Mode = SecurityMode.Message;
			binding.Security.Message.ClientCredentialType = authenticated ?
				MessageCredentialType.UserName : MessageCredentialType.None;

			// establish endpoint
			host.AddServiceEndpoint(serviceContract, binding, "");

			// expose meta-data via HTTP GET
			ServiceMetadataBehavior metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
			if (metadataBehavior == null)
			{
				metadataBehavior = new ServiceMetadataBehavior();
				metadataBehavior.HttpGetEnabled = true;
				host.Description.Behaviors.Add(metadataBehavior);
			}

			// set up the certificate - required for WSHttpBinding
			host.Credentials.ServiceCertificate.SetCertificate(
				StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, hostUri.Host);

			if (authenticated)
			{
				// set up authentication model
				host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
				host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new CustomUserValidator();


				// set up authorization
				List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
				policies.Add(new CustomAuthorizationPolicy());
				host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
				host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
			}
		}

		#endregion
	}
}
