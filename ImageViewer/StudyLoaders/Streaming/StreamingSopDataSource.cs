using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.ImageViewer.StudyManagement;
using System.IO;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal class StreamingSopDataSource : DicomMessageSopDataSource
	{
		private readonly InstanceXml _instanceXml;
		private readonly string _host;
		private readonly string _aeTitle;
		private readonly int _wadoServicePort;
		private volatile bool _fullHeaderRetrieved = false;

		public StreamingSopDataSource(InstanceXml instanceXml, string host, string aeTitle, int wadoServicePort)
			: base(new DicomFile("", new DicomAttributeCollection(), instanceXml.Collection))
		{
			_instanceXml = instanceXml;
			_host = host;
			_aeTitle = aeTitle;
			_wadoServicePort = wadoServicePort;
		}

		private InstanceXmlDicomAttributeCollection AttributeCollection
		{
			get { return (InstanceXmlDicomAttributeCollection)_instanceXml.Collection; }	
		}

		public override DicomAttribute GetDicomAttribute(uint tag)
		{
			lock (SyncLock)
			{
				if (NeedFullHeader(tag))
					GetFullHeader();

				return base.GetDicomAttribute(tag);
			}
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			lock (SyncLock)
			{
				if (NeedFullHeader(tag))
					GetFullHeader();

				return base.TryGetAttribute(tag, out attribute);
			}
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

				pixelData = ToArgb(this, pixelData, pi);
			}

			return pixelData;
		}

		private bool NeedFullHeader(uint tag)
		{
			if (CollectionUtils.Contains(AttributeCollection.ExcludedTags,
				delegate(DicomTag dicomTag) { return dicomTag.TagValue == tag; }))
			{
				return true;
			}

			DicomAttribute attribute = base.GetDicomAttribute(tag);
			if (attribute is DicomAttributeSQ)
			{
				DicomSequenceItem[] items = attribute.Values as DicomSequenceItem[];
				if (items != null)
				{
					foreach (DicomSequenceItem item in items)
					{
						if (item is InstanceXmlDicomSequenceItem)
						{
							if (((InstanceXmlDicomSequenceItem) item).HasExcludedTags(true))
								return true;
						}
					}
				}
			}

			return false;
		}

		private void GetFullHeader()
		{
			if (!_fullHeaderRetrieved)
			{
				Uri uri = new Uri(String.Format(StreamingSettings.Default.FormatWadoUriPrefix, _host, _wadoServicePort));
				StreamingClient client = new StreamingClient(uri);

				DicomFile imageHeader = new DicomFile();
				using (Stream imageHeaderStream = client.RetrieveImageHeader(_aeTitle, StudyInstanceUid, SeriesInstanceUid, SopInstanceUid))
				{
					imageHeader.Load(imageHeaderStream);
					base.SourceMessage = imageHeader;
					_fullHeaderRetrieved = true;
				}
			}
		}
	}
}
