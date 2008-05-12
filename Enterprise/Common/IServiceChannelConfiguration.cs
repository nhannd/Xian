using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Defines an interface to an object that is responsible for configuring a service channel.
	/// </summary>
    public interface IServiceChannelConfiguration
    {
		/// <summary>
		/// Configures and returns an instance of the specified service channel factory, according to the specified arguments.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
        ChannelFactory ConfigureChannelFactory(ServiceChannelConfigurationArgs args);
    }
}
