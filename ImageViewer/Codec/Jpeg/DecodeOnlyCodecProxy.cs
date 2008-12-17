using System;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;

namespace ClearCanvas.ImageViewer.Codec.Jpeg
{
	internal class DecodeOnlyCodecProxy : IDicomCodec
	{
		private readonly IDicomCodec _codec;

		public DecodeOnlyCodecProxy(IDicomCodec codec)
		{
			_codec = codec;
		}

		#region IDicomCodec Members

		public string Name
		{
			get { return _codec.Name; }
		}

		public TransferSyntax CodecTransferSyntax
		{
			get { return _codec.CodecTransferSyntax; }
		}

		public void Encode(DicomUncompressedPixelData oldPixelData, DicomCompressedPixelData newPixelData, DicomCodecParameters parameters)
		{
			string message = String.Format("Encoding with the '{0}' codec is not supported at this time.", _codec.Name);
			throw new NotSupportedException(message);
		}

		public void Decode(DicomCompressedPixelData oldPixelData, DicomUncompressedPixelData newPixelData, DicomCodecParameters parameters)
		{
			_codec.Decode(oldPixelData, newPixelData, parameters);
		}

		public void DecodeFrame(int frame, DicomCompressedPixelData oldPixelData, DicomUncompressedPixelData newPixelData, DicomCodecParameters parameters)
		{
			_codec.DecodeFrame(frame, oldPixelData, newPixelData, parameters);
		}

		#endregion
	}
}
