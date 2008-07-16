using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	public class StreamingImageSop : ImageSop
	{
		private readonly string _host;

		public StreamingImageSop(DicomFile dicomFile, string host)
		{
			_dicomFile = dicomFile;
			_host = host;
		}

		protected override Frame CreateFrame(int index)
		{
			return new StreamingFrame(this, index, _host);
		}
	}
}
