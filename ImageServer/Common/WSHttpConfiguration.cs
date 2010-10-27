#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
