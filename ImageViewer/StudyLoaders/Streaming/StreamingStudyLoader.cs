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
		private IEnumerator<DicomFile> _dicomFiles;
		private ApplicationEntity _ae;
		private StreamingPrefetchingStrategy _prefetcher;

		#region IStudyLoader Members

		public string Name
		{
			get { return "CC_STREAMING"; }
		}

		public int Start(StudyLoaderArgs studyLoaderArgs)
		{
			ApplicationEntity ae = studyLoaderArgs.Server as ApplicationEntity;
			_ae = ae;

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
				return new StreamingImageSop(_dicomFiles.Current, _ae.Host, _ae.WadoServicePort);
		}

		public void StartPrefetching(IImageViewer imageViewer)
		{
			if (_prefetcher == null)
				_prefetcher = new StreamingPrefetchingStrategy(imageViewer);
		}

		public void StopPrefetching()
		{
			_prefetcher.Stop();
			_prefetcher = null;
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

					file.TransferSyntax = instanceXml.TransferSyntax;
					files.Add(file);
				}
			}
			return files;
		}
	}
}
