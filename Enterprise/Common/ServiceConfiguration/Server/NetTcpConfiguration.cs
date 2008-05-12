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
	class NetTcpConfiguration : IServiceHostConfiguration
	{
		#region IServiceHostConfiguration Members

        public void ConfigureServiceHost(ServiceHost host, ServiceHostConfigurationArgs args)
		{
            NetTcpBinding binding = new NetTcpBinding();
			binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;
            binding.ReaderQuotas.MaxStringContentLength = args.MaxReceivedMessageSize;
            binding.ReaderQuotas.MaxArrayLength = args.MaxReceivedMessageSize;
			binding.Security.Mode = args.Authenticated ? SecurityMode.TransportWithMessageCredential : SecurityMode.Transport;
			binding.Security.Message.ClientCredentialType = args.Authenticated ?
				MessageCredentialType.UserName : MessageCredentialType.None;

			// establish endpoint
			host.AddServiceEndpoint(args.ServiceContract, binding, "");

			// set up the certificate - required for transmitting custom credentials
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
