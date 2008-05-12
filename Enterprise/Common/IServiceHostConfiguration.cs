using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common
{
	public interface IServiceHostConfiguration
	{
		void ConfigureServiceHost(ServiceHost host, ServiceHostConfigurationArgs args);
	}
}
