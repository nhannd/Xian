using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.DicomServer
{
	[ServiceContract(ConfigurationName = "IDicomSendService")]
	public interface IDicomSendService
	{
		[OperationContract]
		SendOperationReference SendStudies(SendStudiesRequest request);
		
		[OperationContract]
		SendOperationReference SendSeries(SendSeriesRequest request);
		
		[OperationContract]
		SendOperationReference SendSopInstances(SendSopInstancesRequest request);

		[OperationContract]
		SendOperationReference SendFiles(SendFilesRequest request);
	}
}
