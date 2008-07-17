using System;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.ServiceModel.Streaming;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	public class StreamingFrame : Frame
	{
		private readonly string _host;
		private readonly int _wadoServicePort;

		public StreamingFrame(StreamingImageSop parentImageSop, int frameNumber, string host, int wadoServicePort)
			: base(parentImageSop, frameNumber)
		{
			_host = host;
			_wadoServicePort = wadoServicePort;
		}

		public override byte[] GetNormalizedPixelData()
		{
			if (_pixelData == null)
			{
				lock (_syncLock)
				{
					if (_pixelData == null)
					{
						string uri = String.Format("http://{0}:{1}/WADO", _host, _wadoServicePort);

						StreamingClient client = new StreamingClient();
						_pixelData = client.RetrievePixelData(
							uri,
							ParentImageSop.StudyInstanceUID,
							ParentImageSop.SeriesInstanceUID,
							ParentImageSop.SopInstanceUID,
							FrameNumber - 1);

						//_pixelData = GetNormalizedPixelData(_pixelData);

						if (this.IsColor)
							_pixelData = ToArgb(_pixelData, PhotometricInterpretation.Rgb);
					}
				}
			}

			return _pixelData;
		}

	}
}
