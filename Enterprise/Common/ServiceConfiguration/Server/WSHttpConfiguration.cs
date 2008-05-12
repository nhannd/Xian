using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;

namespace ClearCanvas.Enterprise.Common.ServiceConfiguration.Server
{
	class WSHttpConfiguration : IServiceHostConfiguration
	{
		#region IServiceHostConfiguration Members

        public void ConfigureServiceHost(ServiceHost host, ServiceHostConfigurationArgs args)
		{
			WSHttpBinding binding = new WSHttpBinding();
			binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;
            binding.ReaderQuotas.MaxStringContentLength = args.MaxReceivedMessageSize;
            binding.ReaderQuotas.MaxArrayLength = args.MaxReceivedMessageSize;
			binding.Security.Mode = SecurityMode.Message;
			binding.Security.Message.ClientCredentialType = args.Authenticated ?
				MessageCredentialType.UserName : MessageCredentialType.None;

			// establish endpoint
			host.AddServiceEndpoint(args.ServiceContract, binding, "");

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
				StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, args.HostUri.Host);

            if (args.Authenticated)
			{
				// set up authentication model
				host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
				host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = args.UserNamePasswordValidator;


				// set up authorization
				List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
				policies.Add(args.AuthorizationPolicy);
				host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
				host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
			}
		}

		#endregion
	}
}
