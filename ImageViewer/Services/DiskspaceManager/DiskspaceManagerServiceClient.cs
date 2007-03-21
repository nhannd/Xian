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

		public GetServerSettingResponse GetServerSetting()
		{
			return base.Channel.GetServerSetting();
		}

		public void UpdateServerSetting(UpdateServerSettingRequest request)
		{
			base.Channel.UpdateServerSetting(request);
		}
	}
}
