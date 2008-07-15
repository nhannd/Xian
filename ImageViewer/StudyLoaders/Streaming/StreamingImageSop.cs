using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	public class StreamingImageSop : ImageSop
	{
		private readonly string _host;
		private DicomFile _dicomPixelData;

		public StreamingImageSop(DicomFile dicomFile, string host)
		{
			_dicomFile = dicomFile;
			_host = host;
		}

		internal DicomFile DicomPixelData
		{
			get { return _dicomPixelData; }
			set { _dicomPixelData = value; }
		}

		protected override Frame CreateFrame(int index)
		{
			return new StreamingFrame(this, index, _host);
		}
	}
}
