using System;
using System.IO;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.DicomServices.ServiceModel.Streaming;

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
					}
				}
			}

			return _pixelData;
		}

	}
}
