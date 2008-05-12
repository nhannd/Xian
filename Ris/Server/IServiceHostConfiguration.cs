using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Ris.Server
{
	public interface IServiceHostConfiguration
	{
		void ConfigureServiceHost(ServiceHost host, ServiceHostConfigurationParams args);
	}
}
