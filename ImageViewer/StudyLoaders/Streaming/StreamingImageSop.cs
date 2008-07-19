using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	public class StreamingImageSop : ImageSop
	{
		private readonly string _host;
		private readonly int _wadoServicePort;

		public StreamingImageSop(DicomMessageBase dicomMessage, string host, int wadoServicePort)
			: base(dicomMessage)
		{
			_host = host;
			_wadoServicePort = wadoServicePort;
		}

		protected override Frame CreateFrame(int index)
		{
			return new StreamingFrame(this, index, _host, _wadoServicePort);
		}
	}
}
