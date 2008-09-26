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
            binding.MaxReceivedMessageSize = args.MaxReceivedMessageSize;

            // allow individual string content to be same size as entire message
            binding.ReaderQuotas.MaxStringContentLength = args.MaxReceivedMessageSize;
            binding.ReaderQuotas.MaxArrayLength = args.MaxReceivedMessageSize;

            ChannelFactory channelFactory = (ChannelFactory)Activator.CreateInstance(args.ChannelFactoryClass, binding,
                new EndpointAddress(args.ServiceUri));
            channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = (System.ServiceModel.Security.X509CertificateValidationMode)Enum.Parse(typeof(System.ServiceModel.Security.X509CertificateValidationMode), args.CertificateValidationMode);

            return channelFactory;
        }

        #endregion
    }
}
