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
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Creates and configures a WS-HTTP service channel for the server.
    /// </summary>
    public class ServerWsHttpConfiguration : IServiceHostConfiguration
    {
        #region IServiceHostConfiguration Members

        public void ConfigureServiceHost(ServiceHost host, ServiceHostConfigurationArgs args)
        {
            WSHttpBinding binding = new WSHttpBinding();
            binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;
            binding.ReaderQuotas.MaxStringContentLength = args.MaxReceivedMessageSize;
            binding.ReaderQuotas.MaxArrayLength = args.MaxReceivedMessageSize;
            binding.Security.Mode = WebServicesSettings.Default.SecurityMode;
            binding.Security.Message.ClientCredentialType = args.Authenticated
                                                                ? MessageCredentialType.UserName
                                                                : MessageCredentialType.None;
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

            // set up the certificate 
            if (WebServicesSettings.Default.SecurityMode == SecurityMode.Message 
                || WebServicesSettings.Default.SecurityMode==SecurityMode.TransportWithMessageCredential)
            {
                host.Credentials.ServiceCertificate.SetCertificate(
                    StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, args.HostUri.Host);
            }
        }


    

        #endregion
    }


    /// <summary>
    /// Creates and configures a WS-HTTP service channel for the client.
    /// </summary>
    public class ClientWsHttpConfiguration : IServiceChannelConfiguration
    {
        #region IServiceChannelConfiguration Members

        /// <summary>
        /// Configures and returns an instance of the specified service channel factory, according to the specified arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ChannelFactory ConfigureChannelFactory(ServiceChannelConfigurationArgs args)
        {
            WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Mode =  WebServicesSettings.Default.SecurityMode;
            binding.Security.Message.ClientCredentialType =
                args.AuthenticationRequired ? MessageCredentialType.UserName : MessageCredentialType.None;
            binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;

            // allow individual string content to be same size as entire message
            binding.ReaderQuotas.MaxStringContentLength = args.MaxReceivedMessageSize;
            binding.ReaderQuotas.MaxArrayLength = args.MaxReceivedMessageSize;

            //binding.ReceiveTimeout = new TimeSpan(0, 0 , 20);
            //binding.SendTimeout = new TimeSpan(0, 0, 10);

            ChannelFactory channelFactory = (ChannelFactory)Activator.CreateInstance(args.ChannelFactoryClass, binding,
                new EndpointAddress(args.ServiceUri));
            channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = args.CertificateValidationMode;
            channelFactory.Credentials.ServiceCertificate.Authentication.RevocationMode = args.RevocationMode;

            return channelFactory;
        }

        #endregion
    }
}
