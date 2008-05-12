using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Defines an interface to an object that is responsible for configuring a service host.
	/// </summary>
	public interface IServiceHostConfiguration
	{
		/// <summary>
		/// Configures the specified service host, according to the specified arguments.
		/// </summary>
		/// <param name="host"></param>
		/// <param name="args"></param>
		void ConfigureServiceHost(ServiceHost host, ServiceHostConfigurationArgs args);
	}
}
