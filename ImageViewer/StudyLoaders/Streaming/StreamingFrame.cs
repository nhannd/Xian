using System;
using System.IO;
using System.Net;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common;

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

		private StreamingImageSop StreamingParentImageSop
		{
			get { return this.ParentImageSop as StreamingImageSop; }			
		}

		public override byte[] GetNormalizedPixelData()
		{
			if (_pixelData == null)
			{
				lock (_syncLock)
				{
					if (_pixelData == null)
					{
						if (this.StreamingParentImageSop.DicomPixelData == null)
						{
							//TimeSpanStatistics timer = new TimeSpanStatistics();

							HttpWebResponse response = GetWadoReponse();

							if (response.StatusCode != HttpStatusCode.OK)
								throw new Exception("Unable to connect to WADO server");

							//timer.Start();
							MemoryStream stream = ReadWadoReponse(response);
							//timer.End();

							//string str = String.Format("ReadWadoReponse: {0} ms", timer.Value.TotalMilliseconds);
							//Platform.Log(LogLevel.Info, str);

							//timer.Start();
							this.StreamingParentImageSop.DicomPixelData = new DicomFile();
							this.StreamingParentImageSop.DicomPixelData.Load(stream);
							stream.Close();
							//timer.End();

							//str = String.Format("DicomFile.Load: {0} ms", timer.Value.TotalMilliseconds);
							//Platform.Log(LogLevel.Info, str);
						}

						_pixelData = GetNormalizedPixelData(this.StreamingParentImageSop.DicomPixelData);
					}
				}
			}

			return _pixelData;
		}

		private MemoryStream ReadWadoReponse(HttpWebResponse response)
		{
			Stream stream = response.GetResponseStream();
			int bufferSize = 1 * 1024 * 1024;// OptimizeBufferSize(response.ContentLength);
			byte[] buffer = new byte[bufferSize];
			MemoryStream memStream = new MemoryStream();
			int bytesRead = 0;

			do
			{
				bytesRead = stream.Read(buffer, 0, buffer.Length);
				memStream.Write(buffer, 0, bytesRead);
			} 
			while (bytesRead > 0);

			stream.Close();

			return memStream;
		}

		private HttpWebResponse GetWadoReponse()
		{
			string url = "http://{3}:{4}/wado?requesttype=WADO&studyUID={0}&seriesUID={1}&objectUID={2}&contentType=application%2Fdicom";
			url = String.Format(
				url,
				ParentImageSop.StudyInstanceUID,
				ParentImageSop.SeriesInstanceUID,
				ParentImageSop.SopInstanceUID,
				_host,
				1000);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Accept = "*/*";
			return (HttpWebResponse)request.GetResponse();
		}
	}
}
