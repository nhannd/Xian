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

		public DicomServerServiceClient(string endpointConfigurationName)
			:
				base(endpointConfigurationName)
		{
		}

		public DicomServerServiceClient(string endpointConfigurationName, string remoteAddress)
			:
				base(endpointConfigurationName, remoteAddress)
		{
		}

		public DicomServerServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
			:
				base(endpointConfigurationName, remoteAddress)
		{
		}

		public DicomServerServiceClient(Binding binding, EndpointAddress remoteAddress)
			:
				base(binding, remoteAddress)
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
