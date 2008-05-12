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
            int maxReceivedMessageSize)
        {
            this.ChannelFactoryClass = channelFactoryClass;
            this.ServiceUri = serviceUri;
            this.AuthenticationRequired = authenticationRequired;
            this.MaxReceivedMessageSize = maxReceivedMessageSize;
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
    }
}
