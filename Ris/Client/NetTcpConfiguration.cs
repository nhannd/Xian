using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Ris.Client
{
    public class NetTcpConfiguration : IServiceChannelConfiguration
    {
        #region IServiceChannelConfiguration Members

        public ChannelFactory ConfigureChannelFactory(ServiceChannelConfigurationArgs args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = args.AuthenticationRequired ? SecurityMode.TransportWithMessageCredential : SecurityMode.Transport;
            binding.Security.Message.ClientCredentialType =
                args.AuthenticationRequired ? MessageCredentialType.UserName : MessageCredentialType.None;
            binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;

            // allow individual string content to be same size as entire message
            binding.ReaderQuotas.MaxStringContentLength = args.MaxReceivedMessageSize;
            binding.ReaderQuotas.MaxArrayLength = args.MaxReceivedMessageSize;

            ChannelFactory channelFactory = (ChannelFactory)Activator.CreateInstance(args.ChannelFactoryClass, binding,
                new EndpointAddress(args.ServiceUri));
            channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.PeerOrChainTrust;

            return channelFactory;
        }

        #endregion
    }
}
