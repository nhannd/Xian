using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	public class StreamingImageSop : ImageSop
	{
		private readonly string _host;
		private readonly string _aeTitle;
		private readonly int _wadoServicePort;

		public StreamingImageSop(
			DicomMessageBase dicomMessage, 
			string host, 
			string aeTitle, 
			int wadoServicePort)
			: base(dicomMessage)
		{
			_host = host;
			_aeTitle = aeTitle;
			_wadoServicePort = wadoServicePort;
		}

		protected override Frame CreateFrame(int index)
		{
			return new StreamingFrame(this, index, _host, _aeTitle, _wadoServicePort);
		}
	}
}
