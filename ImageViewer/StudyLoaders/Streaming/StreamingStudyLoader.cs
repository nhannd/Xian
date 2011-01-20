#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Xml;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
    public interface IStreamingStudyLoaderConfiguration
    {
        string GetClientAETitle();
    }

    /// <summary>
    /// This class is intended to be used for the workstation
    /// </summary>
    public class DefaultStreamingStudyLoaderConfigurationProvider : IStreamingStudyLoaderConfiguration
    {
        public string GetClientAETitle()
        {
            return new ServerTree().RootNode.LocalDataStoreNode.GetClientAETitle();
        }
    }

    public class StreamingStudyLoaderConfigurationExtensionPoint : ExtensionPoint<IStreamingStudyLoaderConfiguration>
    { }


    [ExtensionOf(typeof(StudyLoaderExtensionPoint))]
    public class StreamingStudyLoader : StudyLoader
    {
        private IEnumerator<InstanceXml> _instances;
        private ApplicationEntity _serverAe;

        public StreamingStudyLoader()
            : base("CC_STREAMING")
        {
            PrefetchingStrategy = new StreamingPrefetchingStrategy();
        }

        protected override int OnStart(StudyLoaderArgs studyLoaderArgs)
        {
            ApplicationEntity serverAe = studyLoaderArgs.Server as ApplicationEntity;
            _serverAe = serverAe;

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
                AuditHelper.LogOpenStudies(new string[] { _serverAe.AETitle }, loadedInstances, EventSource.CurrentUser, result);
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

            return new StreamingSopDataSource(_instances.Current, _serverAe.Host, _serverAe.AETitle, StreamingSettings.Default.FormatWadoUriPrefix, _serverAe.WadoServicePort);
        }

        private XmlDocument RetrieveHeaderXml(StudyLoaderArgs studyLoaderArgs)
        {
            HeaderStreamingParameters headerParams = new HeaderStreamingParameters();
            headerParams.StudyInstanceUID = studyLoaderArgs.StudyInstanceUid;
            headerParams.ServerAETitle = _serverAe.AETitle;
            headerParams.ReferenceID = Guid.NewGuid().ToString();

            HeaderStreamingServiceClient client = null;
            try
            {

                string uri = String.Format(StreamingSettings.Default.FormatHeaderServiceUri, _serverAe.Host, _serverAe.HeaderServicePort);
                EndpointAddress endpoint = new EndpointAddress(uri);

                client = new HeaderStreamingServiceClient();
                client.Endpoint.Address = endpoint;

                client.Open();
                XmlDocument headerXmlDocument;

                // TODO: REVIEW THIS
                // StreamingStudyLoader now users a plugin to determine the client ae title.
                // The default behaviour is to use the server tree.
                using (Stream stream = client.GetStudyHeader(GetClientAETitle(), headerParams))
                {
                    headerXmlDocument = DecompressHeaderStreamToXml(stream);
                    stream.Close();
                }
                client.Close();
                return headerXmlDocument;
            }
            catch (FaultException<StudyIsInUseFault> e)
            {
                throw new InUseLoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
            }
            catch (FaultException<StudyIsNearlineFault> e)
            {
                throw new NearlineLoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
            }
            catch (FaultException<StudyNotFoundFault> e)
            {
                throw new NotFoundLoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
            }
            catch (FaultException e)
            {
                //TODO: remove this hack.  Not sure why the ImageServer throws a generic fault when there's a more specialized one.
                string message = e.Message.ToLower();
                if (message.Contains("nearline"))
                    throw new NearlineLoadStudyException(studyLoaderArgs.StudyInstanceUid, e);

                throw new LoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
            }
            catch (Exception e)
            {
                if (client != null)
                    client.Abort();
                throw new LoadStudyException(studyLoaderArgs.StudyInstanceUid, e);
            }
        }

        private string GetClientAETitle()
        {
            IStreamingStudyLoaderConfiguration config = null;
            try
            {
                config = new StreamingStudyLoaderConfigurationExtensionPoint().CreateExtension() as IStreamingStudyLoaderConfiguration;
            }
            catch (Exception)
            {
                config = new DefaultStreamingStudyLoaderConfigurationProvider();
            }

            return config.GetClientAETitle();
        }

        private static XmlDocument DecompressHeaderStreamToXml(Stream stream)
        {
            XmlDocument doc;
            using (GZipStream gzStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                doc = new XmlDocument();
                doc.Load(gzStream);
                Platform.Log(LogLevel.Debug, "Unzipped gzip header stream");

                gzStream.Close();
            }

            return doc;
        }
    }
}
