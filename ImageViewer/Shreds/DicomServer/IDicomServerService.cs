using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[ServiceContract]
	public interface IDicomServerService
	{
		[OperationContract]
		void Send(DicomSendRequest request);

		[OperationContract]
		void Retrieve(DicomRetrieveRequest request);

		[OperationContract]
		GetServerSettingResponse GetServerSetting();

		[OperationContract]
		void UpdateServerSetting(UpdateServerSettingRequest request);
	}
}
