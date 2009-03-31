using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{

    /// <summary>
    /// Represents a web client that can be used to retrieve study images or pixel data from a streaming server using WADO protocol.
    /// </summary>
    public class StreamingClient
    {
        private readonly Uri _baseUri;

        /// <summary>
        /// Creates an instance of <see cref="StreamingClient"/> to connect to a streaming server.
        /// </summary>
        /// <param name="baseUri">Base Uri to the location where the streaming server is located (eg http://localhost:1000/wado)</param>
        public StreamingClient(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        #region Public Methods

        public byte[] RetrievePixelData(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid, int frame)
        {
            FrameStreamingResultMetaData result;
            return RetrievePixelData(serverAE, studyInstanceUID, seriesInstanceUID, sopInstanceUid, frame, out result);
        }

        public byte[] RetrievePixelData(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid, int frame, out FrameStreamingResultMetaData metaInfo)
        {
            CodeClock clock = new CodeClock();
			clock.Start();

			FrameStreamingResultMetaData result = new FrameStreamingResultMetaData();
            StringBuilder url = new StringBuilder();

            if (_baseUri.ToString().EndsWith("/"))
            {
                url.AppendFormat("{0}{1}", _baseUri, serverAE);                
            }
            else
            {
                url.AppendFormat("{0}/{1}", _baseUri, serverAE);
            }

            url.AppendFormat("?requesttype=WADO&studyUID={0}&seriesUID={1}&objectUID={2}", studyInstanceUID, seriesInstanceUID, sopInstanceUid);
            url.AppendFormat("&frameNumber={0}", frame);
            url.AppendFormat("&contentType={0}", HttpUtility.HtmlEncode("application/clearcanvas"));

            result.Speed.Start();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.ToString());
            request.Accept = "application/dicom,application/clearcanvas,image/jpeg";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(String.Format("Server responded with an error: {0}", HttpUtility.HtmlDecode(response.StatusDescription)));
            }

            byte[] buffer = new byte[response.ContentLength];
            Stream stream = response.GetResponseStream();
            int offset = 0;
            do
            {
                int readSize = stream.Read(buffer, offset, buffer.Length - offset);
                if (readSize <= 0)
                    break;
                offset += readSize;

            } while (true);
            stream.Close();

            result.Speed.SetData(buffer.Length);
            result.Speed.End();

            result.ResponseMimeType = response.ContentType;
            result.Status = response.StatusCode;
            result.StatusDescription = response.StatusDescription;
            result.Uri = response.ResponseUri;
            result.ContentLength = buffer.Length;
            result.IsLast = (response.Headers["IsLast"] != null && bool.Parse(response.Headers["IsLast"]));

			clock.Stop();
			PerformanceReportBroker.PublishReport("Streaming", "RetrievePixelData", clock.Seconds);

            if (response.Headers["Compressed"] != null && bool.Parse(response.Headers["Compressed"]))
            {
                buffer = DecompressPixelData(response, buffer);
            }

			metaInfo = result;
            return buffer;
        }

		public Stream RetrieveImageHeader(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid)
		{
			StreamingResultMetaData metaInfo;
			return RetrieveImageHeader(serverAE, studyInstanceUID, seriesInstanceUID, sopInstanceUid, DicomTags.PixelData, out metaInfo);
		}

		public Stream RetrieveImageHeader(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid, uint stopTag, out StreamingResultMetaData metaInfo)
		{
        	string imageUrl = BuildImageUrl(serverAE, studyInstanceUID, seriesInstanceUID, sopInstanceUid);
			imageUrl = imageUrl + String.Format("&stopTag={0:x8}", stopTag);
			imageUrl = imageUrl + String.Format("&contentType={0}", HttpUtility.HtmlEncode("application/clearcanvas-header"));
        	return RetrieveImageData(imageUrl, out metaInfo);
		}
		
		public Stream RetrieveImage(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid)
        {
            StreamingResultMetaData result;
            return RetrieveImage(serverAE, studyInstanceUID, seriesInstanceUID, sopInstanceUid, out result);
        }

        public Stream RetrieveImage(string serverAE, string studyInstanceUID, string seriesInstanceUID, string sopInstanceUid, out StreamingResultMetaData metaInfo)
        {
        	string imageUrl = BuildImageUrl(serverAE, studyInstanceUID, seriesInstanceUID, sopInstanceUid);
			imageUrl = imageUrl + String.Format("&contentType={0}", HttpUtility.HtmlEncode("application/dicom"));
        	return RetrieveImageData(imageUrl, out metaInfo);
        }

        #endregion Public Methods

        #region Private Static Methods

		private string BuildImageUrl(string serverAE, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid)
		{
			Platform.CheckForEmptyString(serverAE, "serverAE");
			Platform.CheckForEmptyString(studyInstanceUid, "studyInstanceUid");
			Platform.CheckForEmptyString(seriesInstanceUid, "seriesInstanceUid");
			Platform.CheckForEmptyString(sopInstanceUid, "sopInstanceUid");

			StringBuilder url = new StringBuilder();
			if (_baseUri.ToString().EndsWith("/"))
			{
				url.AppendFormat("{0}{1}", _baseUri, serverAE);
			}
			else
			{
				url.AppendFormat("{0}/{1}", _baseUri, serverAE);
			}

			url.AppendFormat("?requesttype=WADO&studyUID={0}&seriesUID={1}&objectUID={2}", studyInstanceUid, seriesInstanceUid, sopInstanceUid);
			return url.ToString();
		}

    	private static MemoryStream RetrieveImageData(string url, out StreamingResultMetaData result)
		{
			result = new StreamingResultMetaData();

			result.Speed.Start();

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.ToString());
			request.Accept = "application/dicom,application/clearcanvas,application/clearcanvas-header,image/jpeg";
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			if (response.StatusCode != HttpStatusCode.OK)
			{
				throw new Exception(String.Format("Server responded with an error: {0}", HttpUtility.HtmlDecode(response.StatusDescription)));
			}

			byte[] buffer = new byte[response.ContentLength];
			Stream stream = response.GetResponseStream();
			int offset = 0;
			do
			{
				int readSize = stream.Read(buffer, offset, buffer.Length - offset);
				if (readSize <= 0)
					break;
				offset += readSize;

			} while (true);
			stream.Close();

			result.Speed.SetData(buffer.Length);
			result.Speed.End();

			result.ResponseMimeType = response.ContentType;
			result.Status = response.StatusCode;
			result.StatusDescription = response.StatusDescription;
			result.Uri = response.ResponseUri;
			result.ContentLength = buffer.Length;

			return new MemoryStream(buffer);
		}

    	/// <summary>
        /// Decompressed an image buffer returned by wado http response.
        /// </summary>
        /// <param name="response">WADO http response</param>
        /// <param name="buffer">Compressed pixel data</param>
        /// <returns></returns>
        private static byte[] DecompressPixelData(HttpWebResponse response, byte[] buffer)
        {
            try
            {
				CodeClock clock = new CodeClock();
				clock.Start();

                string transferSyntaxUid = response.Headers["TransferSyntaxUid"];
                TransferSyntax transferSyntax = TransferSyntax.GetTransferSyntax(transferSyntaxUid);
                ushort bitesAllocated = ushort.Parse(response.Headers["BitsAllocated"]);
                ushort bitsStored = ushort.Parse(response.Headers["BitsStored"]);
                ushort height = ushort.Parse(response.Headers["ImageHeight"]);
                ushort width = ushort.Parse(response.Headers["ImageWidth"]);
                ushort samples = ushort.Parse(response.Headers["SamplesPerPixel"]);

                DicomAttributeCollection collection = new DicomAttributeCollection();
                collection[DicomTags.BitsAllocated].SetUInt16(0, bitesAllocated);
                collection[DicomTags.BitsStored].SetUInt16(0, bitsStored);
                collection[DicomTags.HighBit].SetUInt16(0, ushort.Parse(response.Headers["HighBit"]));
                collection[DicomTags.Rows].SetUInt16(0, height);
                collection[DicomTags.Columns].SetUInt16(0, width);
                collection[DicomTags.PhotometricInterpretation].SetStringValue(response.Headers["PhotometricInterpretation"]);
                collection[DicomTags.PixelRepresentation].SetUInt16(0, ushort.Parse(response.Headers["PixelRepresentation"]));
                collection[DicomTags.SamplesPerPixel].SetUInt16(0, samples);
                collection[DicomTags.DerivationDescription].SetStringValue(response.Headers["DerivationDescription"]);
                collection[DicomTags.LossyImageCompression].SetStringValue(response.Headers["LossyImageCompression"]);
                collection[DicomTags.LossyImageCompressionMethod].SetStringValue(response.Headers["LossyImageCompressionMethod"]);
                collection[DicomTags.LossyImageCompressionRatio].SetFloat32(0, float.Parse(response.Headers["LossyImageCompressionRatio"]));
                collection[DicomTags.PixelData] = new DicomFragmentSequence(DicomTags.PixelData);

                ushort planar;
                if (ushort.TryParse(response.Headers["PlanarConfiguration"], out planar))
                    collection[DicomTags.PlanarConfiguration].SetUInt16(0, planar);

                DicomCompressedPixelData cpd = new DicomCompressedPixelData(collection);
                cpd.TransferSyntax = transferSyntax;

                cpd.AddFrameFragment(buffer);
                buffer = cpd.GetFrame(0);

				clock.Stop();
				PerformanceReportBroker.PublishReport("Streaming", "DecompressPixelData", clock.Seconds);

                return buffer;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error occurred while decompressing the pixel data: {0}", ex.Message));
            }

        }
        #endregion

    }
}