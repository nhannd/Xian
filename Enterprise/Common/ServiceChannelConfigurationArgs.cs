using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
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

        public Type ChannelFactoryClass;
        public Uri ServiceUri;
        public bool AuthenticationRequired;
        public int MaxReceivedMessageSize;
    }
}
