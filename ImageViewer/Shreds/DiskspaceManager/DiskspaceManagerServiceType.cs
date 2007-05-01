using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.DiskspaceManager;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerCall)]
    public class DiskspaceManagerServiceType : IDiskspaceManagerService
    {
        public DiskspaceManagerServiceType()
        {
		}

		#region IDiskspaceManagerService Members

		public ServiceInformation GetServiceInformation()
		{
			return DiskspaceManagerProcessor.Instance.GetServiceInformation();
		}

		public void UpdateServiceConfiguration(ServiceConfiguration newConfiguration)
		{
			DiskspaceManagerProcessor.Instance.UpdateServiceConfiguration(newConfiguration);
		}

		#endregion
	}
}
