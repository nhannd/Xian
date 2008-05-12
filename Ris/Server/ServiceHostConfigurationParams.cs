using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Server
{
    public struct ServiceHostConfigurationParams
    {
        public ServiceHostConfigurationParams(Type serviceContract, Uri hostUri, bool authenticated, int maxReceivedMessageSize)
        {
            ServiceContract = serviceContract;
            HostUri = hostUri;
            Authenticated = authenticated;
            MaxReceivedMessageSize = maxReceivedMessageSize;
        }

        public Type ServiceContract;
        public Uri HostUri;
        public bool Authenticated;
        public int MaxReceivedMessageSize;
    }
}
