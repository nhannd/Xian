using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.DicomServer
{
	public class DicomSendServiceClient : ClientBase<IDicomSendService>, IDicomSendService
	{
		public DicomSendServiceClient()
		{
		}

		#region IDicomSendService Members

		public SendOperationReference SendStudies(SendStudiesRequest request)
		{
			return base.Channel.SendStudies(request);
		}

		public SendOperationReference SendSeries(SendSeriesRequest request)
		{
			return base.Channel.SendSeries(request);
		}

		public SendOperationReference SendSopInstances(SendSopInstancesRequest request)
		{
			return base.Channel.SendSopInstances(request);
		}

		public SendOperationReference SendFiles(SendFilesRequest request)
		{
			return base.Channel.SendFiles(request);
		}

		#endregion
	}
}
