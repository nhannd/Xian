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

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	[ExtensionOf(typeof(StudyLoaderExtensionPoint))]
	public class StreamingStudyLoader : IStudyLoader
	{
		private IEnumerator<DicomFile> _dicomFiles;
		private string _host;
		private StreamingPrefetchingStrategy _prefetcher;

		#region IStudyLoader Members

		public string Name
		{
			get { return "CC_STREAMING"; }
		}

		public int Start(StudyLoaderArgs studyLoaderArgs)
		{
			XmlDocument doc = RetrieveHeaderXml(studyLoaderArgs);

			List<DicomFile> dicomFiles = BuildFileList(studyLoaderArgs.StudyInstanceUid, doc);

			_dicomFiles = dicomFiles.GetEnumerator();
			_dicomFiles.Reset();

			return dicomFiles.Count;
		}

		public ImageSop LoadNextImage()
		{
			if (!_dicomFiles.MoveNext())
				return null;
			else
				return new StreamingImageSop(_dicomFiles.Current, _host);
		}

		public void PrefetchPixelData(IImageViewer imageViewer)
		{
			if (_prefetcher == null)
				_prefetcher = new StreamingPrefetchingStrategy(imageViewer);
		}

		#endregion

		private XmlDocument RetrieveHeaderXml(StudyLoaderArgs studyLoaderArgs)
		{
			ApplicationEntity ae = studyLoaderArgs.Server as ApplicationEntity;
			_host = ae.Host;
			HeaderRetrievalParameters headerParams = new HeaderRetrievalParameters();
			headerParams.StudyInstanceUID = studyLoaderArgs.StudyInstanceUid;
			headerParams.ServerAETitle = ae.AETitle;
			headerParams.ReferenceID = Guid.NewGuid().ToString();

			//string uri = String.Format("http://{0}:50221/HeaderRetrieval/HeaderRetrieval", _host);
			string uri = String.Format("http://{0}:60221/HeaderRetrieval/HeaderRetrieval", _host);
			EndpointAddress endpoint = new EndpointAddress(uri);

			HeaderStreamingServiceClient client = 
				new HeaderStreamingServiceClient(
				"BasicHttpBinding_IHeaderRetrievalService",
				endpoint);

			Stream stream = client.GetStudyHeader("CC_NORMAN", headerParams);

			return DecompressHeaderStreamToXml(stream);
		}

		private XmlDocument DecompressHeaderStreamToXml(Stream stream)
		{
			GZipStream gzStream = new GZipStream(stream, CompressionMode.Decompress);

			XmlDocument doc = new XmlDocument();
			doc.Load(gzStream);
			return doc;
		}

		private List<DicomFile> BuildFileList(string studyInstanceUid, XmlDocument doc)
		{
			StudyXml studyXml = new StudyXml(studyInstanceUid);
			studyXml.SetMemento(doc);

			List<DicomFile> files = new List<DicomFile>();
			foreach (SeriesXml seriesXml in studyXml)
			{
				foreach (InstanceXml instanceXml in seriesXml)
				{
					string filename = instanceXml.SopInstanceUid + ".dcm";
					DicomFile file = new DicomFile(
						filename,
						new DicomAttributeCollection(),
						instanceXml.Collection);

					files.Add(file);
				}
			}
			return files;
		}
	}
}
