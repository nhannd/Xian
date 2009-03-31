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
		private IEnumerator<InstanceXml> _instances;
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
			StudyXml studyXml = new StudyXml();
			studyXml.SetMemento(doc);

			_instances = GetInstances(studyXml).GetEnumerator();

			return studyXml.NumberOfStudyRelatedInstances;
		}

		private IEnumerable<InstanceXml> GetInstances(StudyXml studyXml)
		{
			foreach (SeriesXml seriesXml in studyXml)
			{
				foreach (InstanceXml instanceXml in seriesXml)
				{
					yield return instanceXml;
				}
			}
		}

		protected override SopDataSource LoadNextSopDataSource()
		{
			if (!_instances.MoveNext())
				return null;

			return new StreamingSopDataSource(_instances.Current, _ae.Host, _ae.AETitle, _ae.WadoServicePort);
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
	}
}
