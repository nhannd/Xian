using System.ServiceModel;
using System;

namespace ClearCanvas.ImageViewer.Services
{
	[ServiceContract(ConfigurationName = "IDicomFilePublishingService")]
	public interface IDicomFilePublishingService
	{
		[OperationContract(IsOneWay = false)]
		Guid PublishFiles(PublishFilesRequest request);
	}
}
