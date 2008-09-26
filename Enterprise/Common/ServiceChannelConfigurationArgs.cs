using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Arguments for configuration of a service channel.
	/// </summary>
    public struct ServiceChannelConfigurationArgs
    {
        public ServiceChannelConfigurationArgs(
            Type channelFactoryClass,
            Uri serviceUri,
            bool authenticationRequired,
            int maxReceivedMessageSize,
            string certificateValidationMode,
            string revocationMode)
        {
            this.ChannelFactoryClass = channelFactoryClass;
            this.ServiceUri = serviceUri;
            this.AuthenticationRequired = authenticationRequired;
            this.MaxReceivedMessageSize = maxReceivedMessageSize;
            this.CertificateValidationMode = certificateValidationMode;
            this.RevocationMode = revocationMode;
        }

		/// <summary>
		/// The class of the channel factory to create.
		/// </summary>
        public Type ChannelFactoryClass;

		/// <summary>
		/// The URI on which the service is hosted.
		/// </summary>
        public Uri ServiceUri;

		/// <summary>
		/// A value indicating whether the service requires authentication.
		/// </summary>
        public bool AuthenticationRequired;

		/// <summary>
		/// The maximum size of received messages to allow, in bytes.
		/// </summary>
        public int MaxReceivedMessageSize;

        /// <summary>
        /// Specifies the mode used for X509 certificate validation.
        /// </summary>
	    public string CertificateValidationMode;

        /// <summary>
        /// Specifies the mode used to check for X509 certificate revocation.
        /// </summary>
        public string RevocationMode;
    }
}
