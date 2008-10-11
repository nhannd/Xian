using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Xml;
using ClearCanvas.ImageViewer.Services.ServerTree;

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
			HeaderStreamingParameters headerParams = new HeaderStreamingParameters();
			headerParams.StudyInstanceUID = studyLoaderArgs.StudyInstanceUid;
			headerParams.ServerAETitle = _ae.AETitle;
			headerParams.ReferenceID = Guid.NewGuid().ToString();

			//TODO: make it a setting
			string uri = String.Format("http://{0}:{1}/HeaderStreaming/HeaderStreaming", _ae.Host, _ae.HeaderServicePort);
			EndpointAddress endpoint = new EndpointAddress(uri);

			//TODO: load strictly from configuration
			HeaderStreamingServiceClient client = 
				new HeaderStreamingServiceClient(
				"BasicHttpBinding_IHeaderStreamingService",
				endpoint);

			try
			{
				client.Open();
				Stream stream = client.GetStudyHeader(ServerTree.GetClientAETitle(), headerParams);
				client.Close();
				return DecompressHeaderStreamToXml(stream);
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
