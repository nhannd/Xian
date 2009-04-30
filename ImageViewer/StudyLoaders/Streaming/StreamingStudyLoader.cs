#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Services.Auditing;
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
			PrefetchingStrategy = new StreamingPrefetchingStrategy();
		}

		public override int OnStart(StudyLoaderArgs studyLoaderArgs)
		{
			ApplicationEntity ae = studyLoaderArgs.Server as ApplicationEntity;
			_ae = ae;

			EventResult result = EventResult.Success;
			AuditedInstances loadedInstances = new AuditedInstances();
			try
			{

				XmlDocument doc = RetrieveHeaderXml(studyLoaderArgs);
				StudyXml studyXml = new StudyXml();
				studyXml.SetMemento(doc);

				_instances = GetInstances(studyXml).GetEnumerator();

				loadedInstances.AddInstance(studyXml.PatientId, studyXml.PatientsName, studyXml.StudyInstanceUid);

				return studyXml.NumberOfStudyRelatedInstances;

			} 
			finally 
			{
				AuditHelper.LogOpenStudies("Load Studies", new string[] { ae.AETitle }, loadedInstances, EventSource.CurrentUser, result);
			}
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

			return new StreamingSopDataSource(_instances.Current, _ae.Host, _ae.AETitle, StreamingSettings.Default.FormatWadoUriPrefix, _ae.WadoServicePort);
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
