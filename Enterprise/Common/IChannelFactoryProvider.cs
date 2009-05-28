using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common
{
    public interface IChannelFactoryProvider
    {
        ChannelFactory GetPrimary(Type serviceContract);
        ChannelFactory GetFailover(Type serviceContract, EndpointAddress failedEndpoint);
    }
}
