using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Services.DiskspaceManager
{
	[ServiceContract(ConfigurationName = "IDiskspaceManagerService")]
	public interface IDiskspaceManagerService
	{
		[OperationContract]
		ServiceInformation GetServiceInformation();

		[OperationContract]
		void UpdateServiceConfiguration(ServiceConfiguration newConfiguration);
	}
}
