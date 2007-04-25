using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.DicomServer;

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

		public DicomServerConfiguration GetServerConfiguration()
		{
			return DicomServerManager.Instance.GetServerConfiguration();
		}

		public void UpdateServerConfiguration(DicomServerConfiguration newConfiguration)
		{
			DicomServerManager.Instance.UpdateServerConfiguration(newConfiguration);
		}

		#endregion
	}
}
