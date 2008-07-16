using System;
using System.IO;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	public class StreamingFrame : Frame
	{
		private string _host;

		public StreamingFrame(StreamingImageSop parentImageSop, int frameNumber, string host)
			: base(parentImageSop, frameNumber)
		{
			_host = host;
		}

		public override byte[] GetNormalizedPixelData()
		{
			if (_pixelData == null)
			{
				lock (_syncLock)
				{
					if (_pixelData == null)
					{
						//TimeSpanStatistics timer = new TimeSpanStatistics();

						//timer.Start();
						HttpWebResponse response = GetWadoReponse();
						//timer.End();

						//string str = String.Format("GetWadoReponse: {0} ms", timer.Value.TotalMilliseconds);
						//Platform.Log(LogLevel.Info, str);

						if (response.StatusCode != HttpStatusCode.OK)
							throw new Exception("Unable to connect to WADO server");

						//timer.Reset();

						//timer.Start();
						_pixelData = ReadWadoReponse(response);
						//timer.End();

						//str = String.Format("ReadWadoReponse: {0} ms", timer.Value.TotalMilliseconds);
						//Platform.Log(LogLevel.Info, str);
					}
				}
			}

			return _pixelData;
		}

		private byte[] ReadWadoReponse(HttpWebResponse response)
		{
			Stream stream = response.GetResponseStream();
			byte[] buffer = new byte[response.ContentLength];
			int bytesRead = 0;
			int offset = 0;

			do
			{
				bytesRead = stream.Read(buffer, offset, buffer.Length - offset);
				offset += bytesRead;
			}
			while (bytesRead > 0);

			stream.Close();

			return buffer;
		}

		private HttpWebResponse GetWadoReponse()
		{
			string url =
				"http://{4}:{5}/wado?requesttype=WADO&contentType=application%2Fclearcanvas&studyUID={0}&seriesUID={1}&objectUID={2}&frameNumber={3}";

			url = String.Format(
				url,
				ParentImageSop.StudyInstanceUID,
				ParentImageSop.SeriesInstanceUID,
				ParentImageSop.SopInstanceUID,
				FrameNumber - 1,
				_host,
				2000);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Accept = "*/*";
			return (HttpWebResponse)request.GetResponse();
		}
	}
}
