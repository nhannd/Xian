using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class DicomMessageSopDataSource : StandardSopDataSource, IDicomMessageSopDataSource
	{
		private readonly object _syncLock = new object();
		private readonly DicomMessageBase _sourceMessage;
		private bool _loaded = false;
		private bool _loading = false;

		protected DicomMessageSopDataSource(DicomMessageBase sourceMessage)
			: base()
		{
			_sourceMessage = sourceMessage;
		}

		// TODO: not actually thread-safe because client code can use indexers on "SourceMessage",
		// triggering empty attributes to be inserted.
		public DicomMessageBase SourceMessage
		{
			get
			{
				Load();
				return _sourceMessage;
			}
		}

		protected virtual void EnsureLoaded()
		{
		}

		private void Load()
		{
			lock(_syncLock)
			{
				if (_loaded || _loading)
					return;

				try
				{
					_loading = true;
					EnsureLoaded();
					_loaded = true;
				}
				finally
				{
					_loading = false;
				}
			}
		}

		public override DicomAttribute GetDicomAttribute(uint tag)
		{
			lock (_syncLock)
			{
				Load();

				bool isInMetaInfoRange = (tag >= _sourceMessage.MetaInfo.StartTagValue && tag <= _sourceMessage.MetaInfo.EndTagValue);
				bool isInDataSetRange = (tag >= _sourceMessage.DataSet.StartTagValue && tag <= _sourceMessage.DataSet.EndTagValue);

				if (isInDataSetRange)
				{
					DicomAttribute attribute = _sourceMessage.DataSet[tag];
					if (!attribute.IsEmpty || !isInMetaInfoRange)
						return attribute;
				}

				if (isInMetaInfoRange)
					return _sourceMessage.MetaInfo[tag];

				throw new ArgumentOutOfRangeException("tag", "The tag is not within the valid range for either the meta info or dataset.");
			}
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			lock (_syncLock)
			{
				Load();

				if (_sourceMessage.DataSet.TryGetAttribute(tag, out attribute))
					return true;

				return _sourceMessage.MetaInfo.TryGetAttribute(tag, out attribute);
			}
		}

		protected override byte[] CreateFrameNormalizedPixelData(int frameNumber)
		{
			//already locked by base calling method, but it doesn't hurt.
			lock (_syncLock)
			{
				Load();
				return CreateFrameNormalizedPixelData(SourceMessage, frameNumber);
			}
		}

		public static byte[] CreateFrameNormalizedPixelData(DicomMessageBase message, int frameNumber)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			PhotometricInterpretation photometricInterpretation;
			byte[] rawPixelData;

			if (!message.TransferSyntax.Encapsulated)
			{
				DicomUncompressedPixelData pixelData = new DicomUncompressedPixelData(message);
				// DICOM library uses zero-based frame numbers
				rawPixelData = pixelData.GetFrame(frameNumber - 1);
				photometricInterpretation = PhotometricInterpretation.FromCodeString(message.DataSet[DicomTags.PhotometricInterpretation]);
			}
			else if (DicomCodecRegistry.GetCodec(message.TransferSyntax) != null)
			{
				DicomCompressedPixelData pixelData = new DicomCompressedPixelData(message);
				string pi;
				rawPixelData = pixelData.GetFrame(frameNumber - 1, out pi);
				photometricInterpretation = PhotometricInterpretation.FromCodeString(pi);
			}
			else
				throw new DicomCodecException("Unsupported transfer syntax");

			if (photometricInterpretation.IsColor)
				rawPixelData = ToArgb(message, rawPixelData, photometricInterpretation);

			clock.Stop();
			PerformanceReportBroker.PublishReport("DicomMessageSopDataSource", "CreateFrameNormalizedPixelData", clock.Seconds);

			return rawPixelData;
		}

		/// <summary>
		/// Converts colour pixel data to ARGB.
		/// </summary>
		protected static byte[] ToArgb(DicomMessageBase message, byte[] pixelData, PhotometricInterpretation photometricInterpretation)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			int rows = message.DataSet[DicomTags.Rows].GetInt32(0, 0);
			int columns = message.DataSet[DicomTags.Columns].GetInt32(0, 0);
			int sizeInBytes = rows * columns * 4;
			byte[] argbPixelData = new byte[sizeInBytes];

			// Convert palette colour images to ARGB so we don't get interpolation artifacts
			// when rendering.
			if (photometricInterpretation == PhotometricInterpretation.PaletteColor)
			{
				int bitsAllocated = message.DataSet[DicomTags.BitsAllocated].GetInt32(0, 0);
				int pixelRepresentation = message.DataSet[DicomTags.PixelRepresentation].GetInt32(0, 0);

				ColorSpaceConverter.ToArgb(
					bitsAllocated,
					pixelRepresentation != 0 ? true : false,
					pixelData,
					argbPixelData,
					PaletteColorMap.Create(message.DataSet));
			}
			// Convert RGB and YBR variants to ARGB
			else
			{
				int planarConfiguration = message.DataSet[DicomTags.PlanarConfiguration].GetInt32(0, 0);

				ColorSpaceConverter.ToArgb(
					photometricInterpretation,
					planarConfiguration,
					pixelData,
					argbPixelData);
			}

			clock.Stop();
			PerformanceReportBroker.PublishReport("DicomMessageSopDataSource", "ToArgb", clock.Seconds);

			return argbPixelData;
		}
	}
}
