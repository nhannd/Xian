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
using System.IdentityModel.Policy;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;

namespace ClearCanvas.Enterprise.Common.ServiceConfiguration.Server
{
	/// <summary>
	/// Configures a TCP service host.
	/// </summary>
	public class NetTcpConfiguration : IServiceHostConfiguration
	{
		#region IServiceHostConfiguration Members

		/// <summary>
		/// Configures the specified service host, according to the specified arguments.
		/// </summary>
		/// <param name="host"></param>
		/// <param name="args"></param>
		public void ConfigureServiceHost(ServiceHost host, ServiceHostConfigurationArgs args)
		{
            NetTcpBinding binding = new NetTcpBinding();
			binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;
            binding.ReaderQuotas.MaxStringContentLength = args.MaxReceivedMessageSize;
            binding.ReaderQuotas.MaxArrayLength = args.MaxReceivedMessageSize;
			binding.Security.Mode = args.Authenticated ? SecurityMode.TransportWithMessageCredential : SecurityMode.Transport;
			binding.Security.Message.ClientCredentialType = args.Authenticated ?
				MessageCredentialType.UserName : MessageCredentialType.None;

			// turn off transport security altogether
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

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
