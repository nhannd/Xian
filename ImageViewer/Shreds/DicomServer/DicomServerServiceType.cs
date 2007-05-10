using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Services;

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

		public void Send(AEInformation destinationAEInformation, IEnumerable<string> uids)
		{
			DicomServerManager.Instance.Send(destinationAEInformation, uids);
		}

		public void RetrieveStudies(AEInformation sourceAEInformation, IEnumerable<StudyInformation> studiesToRetrieve)
		{
			DicomServerManager.Instance.RetrieveStudies(sourceAEInformation, studiesToRetrieve);
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
