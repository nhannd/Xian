using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.ImageViewer.Services.DicomServer
{
	public partial class DicomServerServiceClient : ClientBase<IDicomServerService>, IDicomServerService
	{
		public DicomServerServiceClient()
		{
		}

		public void Send(DicomSendRequest request)
		{
			base.Channel.Send(request);
		}

		public void Retrieve(DicomRetrieveRequest request)
		{
			base.Channel.Retrieve(request);
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
