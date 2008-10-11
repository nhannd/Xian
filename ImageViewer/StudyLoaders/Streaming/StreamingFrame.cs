using System;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	public class StreamingFrame : Frame
	{
		private readonly string _host;
		private readonly string _aeTitle;
		private readonly int _wadoServicePort;

		public StreamingFrame(
			StreamingImageSop parentImageSop, 
			int frameNumber, 
			string host, 
			string aeTitle,
			int wadoServicePort)
			: base(parentImageSop, frameNumber)
		{
			_host = host;
			_aeTitle = aeTitle;
			_wadoServicePort = wadoServicePort;
		}

		protected override byte[] CreateNormalizedPixelData()
		{
			//TODO: Put it in a setting.
			Uri uri = new Uri(String.Format("http://{0}:{1}/WADO", _host, _wadoServicePort));

			StreamingClient client = new StreamingClient(uri);
			byte[] pixelData = client.RetrievePixelData(
				_aeTitle,
				ParentImageSop.StudyInstanceUID,
				ParentImageSop.SeriesInstanceUID,
				ParentImageSop.SopInstanceUID,
				FrameNumber - 1);

			if (this.IsColor)
			{
				PhotometricInterpretation pi;

				TransferSyntax ts = TransferSyntax.GetTransferSyntax(ParentImageSop.TransferSyntaxUID);

				if ( ts == TransferSyntax.Jpeg2000ImageCompression ||
					ts == TransferSyntax.Jpeg2000ImageCompressionLosslessOnly ||
					ts == TransferSyntax.JpegExtendedProcess24 ||
					ts == TransferSyntax.JpegBaselineProcess1 ||
					ts == TransferSyntax.JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1)
					pi = PhotometricInterpretation.Rgb;
				else
					pi = this.PhotometricInterpretation;

				pixelData = ToArgb(pixelData, pi);
			}

			return pixelData;
		}
	}
}
