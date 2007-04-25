using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Services.DicomServer
{
	[ServiceContract(ConfigurationName = "IDicomServerService")]
	public interface IDicomServerService
	{
		[OperationContract]
		void Send(DicomSendRequest request);

		[OperationContract]
		void Retrieve(DicomRetrieveRequest request);

		[OperationContract]
		DicomServerConfiguration GetServerConfiguration();

		[OperationContract]
		void UpdateServerConfiguration(DicomServerConfiguration newConfiguration);
	}
}
