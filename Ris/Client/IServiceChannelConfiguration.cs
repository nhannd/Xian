using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.Ris.Client
{
    public interface IServiceChannelConfiguration
    {
        ChannelFactory ConfigureChannelFactory(ServiceChannelConfigurationArgs args);
    }
}
