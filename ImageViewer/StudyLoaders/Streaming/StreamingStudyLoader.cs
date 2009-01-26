using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Xml;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	[ExtensionOf(typeof(StudyLoaderExtensionPoint))]
	public class StreamingStudyLoader : StudyLoader
	{
		private IEnumerator<DicomMessageBase> _dicomMessages;
		private ApplicationEntity _ae;

		public StreamingStudyLoader() : base("CC_STREAMING")
		{
			PrefetchingStrategy = new VisibleDisplaySetPrefetchingStrategy(StreamingSettings.Default.PrefetchingConcurrency);
		}

		public override int OnStart(StudyLoaderArgs studyLoaderArgs)
		{
			ApplicationEntity ae = studyLoaderArgs.Server as ApplicationEntity;
			_ae = ae;

			XmlDocument doc = RetrieveHeaderXml(studyLoaderArgs);

			List<DicomMessageBase> dicomMessages = BuildFileList(studyLoaderArgs.StudyInstanceUid, doc);

			_dicomMessages = dicomMessages.GetEnumerator();
			_dicomMessages.Reset();

			return dicomMessages.Count;
		}

		protected override SopDataSource  LoadNextSopDataSource()
		{
			if (!_dicomMessages.MoveNext())
				return null;

			return new StreamingSopDataSource(_dicomMessages.Current, _ae.Host, _ae.AETitle, _ae.WadoServicePort);
		}

		private XmlDocument RetrieveHeaderXml(StudyLoaderArgs studyLoaderArgs)
		{
			HeaderStreamingParameters headerParams = new HeaderStreamingParameters();
			headerParams.StudyInstanceUID = studyLoaderArgs.StudyInstanceUid;
			headerParams.ServerAETitle = _ae.AETitle;
			headerParams.ReferenceID = Guid.NewGuid().ToString();

			string uri = String.Format(StreamingSettings.Default.FormatHeaderServiceUri, _ae.Host, _ae.HeaderServicePort);
			EndpointAddress endpoint = new EndpointAddress(uri);

			HeaderStreamingServiceClient client = new HeaderStreamingServiceClient();
			client.Endpoint.Address = endpoint;

			try
			{
				client.Open();
				XmlDocument headerXmlDocument;
				using (Stream stream = client.GetStudyHeader(ServerTree.GetClientAETitle(), headerParams))
				{
					headerXmlDocument = DecompressHeaderStreamToXml(stream);
				}
				client.Close();
				return headerXmlDocument;
			}
			catch(Exception)
			{
				client.Abort();
				throw;
			}
		}

		private static XmlDocument DecompressHeaderStreamToXml(Stream stream)
		{
			GZipStream gzStream = new GZipStream(stream, CompressionMode.Decompress);

			XmlDocument doc = new XmlDocument();
			doc.Load(gzStream);
			return doc;
		}

		private static List<DicomMessageBase> BuildFileList(string studyInstanceUid, XmlDocument doc)
		{
			StudyXml studyXml = new StudyXml(studyInstanceUid);
			studyXml.SetMemento(doc);

			List<DicomMessageBase> dicomMessages = new List<DicomMessageBase>();
			foreach (SeriesXml seriesXml in studyXml)
			{
				foreach (InstanceXml instanceXml in seriesXml)
				{
					DicomFile file = new DicomFile(
						string.Empty,
						new DicomAttributeCollection(),
						instanceXml.Collection);

					file.TransferSyntax = instanceXml.TransferSyntax;
					dicomMessages.Add(file);
				}
			}
			return dicomMessages;
		}
	}
}
