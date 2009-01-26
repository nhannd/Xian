using System;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal class StreamingSopDataSource : DicomMessageSopDataSource
	{
		private readonly string _host;
		private readonly string _aeTitle;
		private readonly int _wadoServicePort;

		public StreamingSopDataSource(DicomMessageBase message, string host, string aeTitle, int wadoServicePort)
			: base(message)
		{
			_host = host;
			_aeTitle = aeTitle;
			_wadoServicePort = wadoServicePort;
		}

		protected override byte[]  CreateFrameNormalizedPixelData(int frameNumber)
		{
			Uri uri = new Uri(String.Format(StreamingSettings.Default.FormatWadoUriPrefix, _host, _wadoServicePort));

			StreamingClient client = new StreamingClient(uri);
			byte[] pixelData = client.RetrievePixelData(_aeTitle, StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, frameNumber - 1);

			string photometricInterpretationCode = this[DicomTags.PhotometricInterpretation].ToString();
			PhotometricInterpretation pi = PhotometricInterpretation.FromCodeString(photometricInterpretationCode);

			if (pi.IsColor)
			{
				TransferSyntax ts = TransferSyntax.GetTransferSyntax(this.TransferSyntaxUid);

				if (ts == TransferSyntax.Jpeg2000ImageCompression ||
					ts == TransferSyntax.Jpeg2000ImageCompressionLosslessOnly ||
					ts == TransferSyntax.JpegExtendedProcess24 ||
					ts == TransferSyntax.JpegBaselineProcess1 ||
					ts == TransferSyntax.JpegLosslessNonHierarchicalFirstOrderPredictionProcess14SelectionValue1)
					pi = PhotometricInterpretation.Rgb;

				pixelData = ToArgb(SourceMessage, pixelData, pi);
			}

			return pixelData;
		}
	}
}
