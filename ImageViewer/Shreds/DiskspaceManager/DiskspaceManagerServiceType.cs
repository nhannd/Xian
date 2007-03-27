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
			//Platform.Log("[" + AppDomain.CurrentDomain.FriendlyName + "]: DiskspaceManagerServiceType Constructor");
		}

		#region IDiskspaceManagerService Members

		public GetServerSettingResponse GetServerSetting()
		{
			return DiskspaceManagerProcessor.Instance.GetServerSetting();
		}

		public void UpdateServerSetting(UpdateServerSettingRequest request)
		{
            DiskspaceManagerProcessor.Instance.UpdateServerSetting(request);
		}

		#endregion
	}
}
