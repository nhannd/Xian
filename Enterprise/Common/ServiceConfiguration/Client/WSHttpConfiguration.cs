using System;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common.ServiceConfiguration.Client
{
	/// <summary>
	/// Creates and configures a WS-HTTP service channel.
	/// </summary>
    public class WSHttpConfiguration : IServiceChannelConfiguration
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
            binding.Security.Mode = SecurityMode.Message;
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
            channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = (System.ServiceModel.Security.X509CertificateValidationMode)Enum.Parse(typeof(System.ServiceModel.Security.X509CertificateValidationMode), args.CertificateValidationMode);
            channelFactory.Credentials.ServiceCertificate.Authentication.RevocationMode = (System.Security.Cryptography.X509Certificates.X509RevocationMode)Enum.Parse(typeof(System.Security.Cryptography.X509Certificates.X509RevocationMode), args.RevocationMode);

            return channelFactory;
        }

        #endregion
    }
}
