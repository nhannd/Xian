using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Ris.Server
{
	public interface IServiceConfiguration
	{
		void ConfigureServiceHost(ServiceHost host, Type serviceContract, Uri hostUri, bool authenticated);
	}
}
