using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.ServiceModel.Streaming;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Xml;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.ImageViewer.Configuration;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	[ExtensionOf(typeof(StudyLoaderExtensionPoint))]
	public class StreamingStudyLoader : IStudyLoader
	{
		private IEnumerator<DicomMessageBase> _dicomMessages;
		private ApplicationEntity _ae;
		private IPrefetchingStrategy _prefetchingStrategy;

		#region IStudyLoader Members

		public string Name
		{
			get { return "CC_STREAMING"; }
		}

		public IPrefetchingStrategy PrefetchingStrategy
		{
			get 
			{
				if (_prefetchingStrategy == null)
					_prefetchingStrategy = new VisibleDisplaySetPrefetchingStrategy();

				return _prefetchingStrategy;
			}
		}

		public int Start(StudyLoaderArgs studyLoaderArgs)
		{
			ApplicationEntity ae = studyLoaderArgs.Server as ApplicationEntity;
			_ae = ae;

			XmlDocument doc = RetrieveHeaderXml(studyLoaderArgs);

			List<DicomMessageBase> dicomMessages = BuildFileList(studyLoaderArgs.StudyInstanceUid, doc);

			_dicomMessages = dicomMessages.GetEnumerator();
			_dicomMessages.Reset();

			return dicomMessages.Count;
		}

		public ImageSop LoadNextImage()
		{
			if (!_dicomMessages.MoveNext())
				return null;
			else
				return new StreamingImageSop(_dicomMessages.Current, _ae.Host, _ae.AETitle, _ae.WadoServicePort);
		}

		#endregion

		private XmlDocument RetrieveHeaderXml(StudyLoaderArgs studyLoaderArgs)
		{
			HeaderRetrievalParameters headerParams = new HeaderRetrievalParameters();
			headerParams.StudyInstanceUID = studyLoaderArgs.StudyInstanceUid;
			headerParams.ServerAETitle = _ae.AETitle;
			headerParams.ReferenceID = Guid.NewGuid().ToString();

			string uri = String.Format("http://{0}:{1}/HeaderRetrieval/HeaderRetrieval", _ae.Host, _ae.HeaderServicePort);
			EndpointAddress endpoint = new EndpointAddress(uri);

			HeaderStreamingServiceClient client = 
				new HeaderStreamingServiceClient(
				"BasicHttpBinding_IHeaderRetrievalService",
				endpoint);

			Stream stream = client.GetStudyHeader(DicomServerConfigurationHelper.AETitle, headerParams);

			return DecompressHeaderStreamToXml(stream);
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
					string filename = instanceXml.SopInstanceUid + ".dcm";
					DicomFile file = new DicomFile(
						filename,
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
