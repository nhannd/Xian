using System.IO;
using System.ServiceModel;
using ClearCanvas.DicomServices.ServiceModel.Streaming;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	class HeaderStreamingServiceClient : ClientBase<IHeaderStreamingService>, IHeaderStreamingService
	{
		public HeaderStreamingServiceClient()
		{

		}

		public HeaderStreamingServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
			: base(endpointConfigurationName, remoteAddress)
		{
			
		}

		public Stream GetStudyHeader(string callingAETitle, HeaderStreamingParameters parameters)
		{
			return base.Channel.GetStudyHeader(callingAETitle, parameters);
		}
	}
}
