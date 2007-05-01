using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.ImageViewer.Services.DiskspaceManager
{
	public partial class DiskspaceManagerServiceClient : ClientBase<IDiskspaceManagerService>, IDiskspaceManagerService
	{
		public DiskspaceManagerServiceClient()
		{
		}

		public ServiceInformation GetServiceInformation()
		{
			return base.Channel.GetServiceInformation();
		}

		public void UpdateServiceConfiguration(ServiceConfiguration newConfiguration)
		{
			base.Channel.UpdateServiceConfiguration(newConfiguration);
		}
	}
}
