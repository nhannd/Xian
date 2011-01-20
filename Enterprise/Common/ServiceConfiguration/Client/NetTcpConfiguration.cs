#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common.ServiceConfiguration.Client
{
	/// <summary>
	/// Creates and configures a TCP service channel.
	/// </summary>
    public class NetTcpConfiguration : IServiceChannelConfiguration
    {
        #region IServiceChannelConfiguration Members

		/// <summary>
		/// Configures and returns an instance of the specified service channel factory, according to the specified arguments.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public ChannelFactory ConfigureChannelFactory(ServiceChannelConfigurationArgs args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = args.AuthenticationRequired ? SecurityMode.TransportWithMessageCredential : SecurityMode.Transport;
            binding.Security.Message.ClientCredentialType =
                args.AuthenticationRequired ? MessageCredentialType.UserName : MessageCredentialType.None;

			// turn off transport security altogether
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;


            binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;

            // allow individual string content to be same size as entire message
            binding.ReaderQuotas.MaxStringContentLength = args.MaxReceivedMessageSize;
            binding.ReaderQuotas.MaxArrayLength = args.MaxReceivedMessageSize;

            ChannelFactory channelFactory = (ChannelFactory)Activator.CreateInstance(args.ChannelFactoryClass, binding,
                new EndpointAddress(args.ServiceUri));
            channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = args.CertificateValidationMode;
			channelFactory.Credentials.ServiceCertificate.Authentication.RevocationMode = args.RevocationMode;

            return channelFactory;
        }

        #endregion
    }
}
