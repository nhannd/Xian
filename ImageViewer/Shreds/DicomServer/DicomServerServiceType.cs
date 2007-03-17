using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerCall)]
    public class DicomServerServiceType : IDicomServerService
    {
        public DicomServerServiceType()
        {
			Platform.Log("[" + AppDomain.CurrentDomain.FriendlyName + "]: DicomServerServiceType Constructor");
		}

		#region IDicomServerService Members

		public void Send(DicomSendRequest request)
		{
			DicomServerManager.Instance.Send(request);
		}

		public void Retrieve(DicomRetrieveRequest request)
		{
			DicomServerManager.Instance.Retrieve(request);

		}

		public GetServerSettingResponse GetServerSetting()
		{
			return DicomServerManager.Instance.GetServerSetting();
		}

		public void UpdateServerSetting(UpdateServerSettingRequest request)
		{
			DicomServerManager.Instance.UpdateServerSetting(request);
		}

		#endregion
	}
}
