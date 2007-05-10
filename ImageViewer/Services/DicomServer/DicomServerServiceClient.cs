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

		public void Send(AEInformation destinationAEInformation, IEnumerable<string> uids)
		{
			base.Channel.Send(destinationAEInformation, uids);
		}

		public void RetrieveStudies(AEInformation sourceAEInformation, IEnumerable<StudyInformation> studiesToRetrieve)
		{
			base.Channel.RetrieveStudies(sourceAEInformation, studiesToRetrieve);
		}

		public DicomServerConfiguration GetServerConfiguration()
		{
			return base.Channel.GetServerConfiguration();
		}

		public void UpdateServerConfiguration(DicomServerConfiguration newConfiguration)
		{
			base.Channel.UpdateServerConfiguration(newConfiguration);
		}
	}
}
