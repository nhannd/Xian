#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Threading;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Dicom.Core
{
    /// <summary>
    /// Class and utilities for finding the directory where a study is stored.
    /// </summary>
    public class StudyLocation
    {
        #region Constructors

        public StudyLocation(string studyInstanceUid)
        {
            Study = new StudyIdentifier
            {
                StudyInstanceUid = studyInstanceUid
            };

            StudyFolder = Path.Combine(GetFileStoreDirectory(), studyInstanceUid);
        }

        public StudyLocation(DicomMessageBase message)
        {
            Study = new StudyIdentifier(message.DataSet);

            StudyFolder = Path.Combine(GetFileStoreDirectory(), Study.StudyInstanceUid);
        }

        #endregion

        #region Public Properties

        public string StudyFolder { get; private set; }

        public StudyIdentifier Study { get; set; }

        #endregion

        #region Public Methods

        public string GetSopInstancePath(string seriesInstanceUid, string sopInstanceUid)
        {
            return Path.Combine(StudyFolder, 
                string.Format("{0}.{1}", sopInstanceUid, "dcm"));
        }

        public string GetStudyXmlPath()
        {
            return Path.Combine(StudyFolder, string.Format("{0}.xml", Study.StudyInstanceUid));
        }

        /// <summary>
        /// Load a <see cref="StudyXml"/> file for the <see cref="StudyLocation"/>
        /// </summary>
        /// <returns>The <see cref="StudyXml"/> instance</returns>
        public StudyXml LoadStudyXml()
        {
            var theXml = new StudyXml();

            string streamFile = GetStudyXmlPath();
            if (File.Exists(streamFile))
            {
                using (Stream fileStream = FileStreamOpener.OpenForRead(streamFile, FileMode.Open))
                {
                    var theDoc = new XmlDocument();

                    StudyXmlIo.Read(theDoc, fileStream);

                    theXml.SetMemento(theDoc);

                    fileStream.Close();
                }
            }
            return theXml;
        }

        public void SaveStudyXml(StudyXml theStream)
        {
            var _settings = new StudyXmlOutputSettings
            {
                IncludePrivateValues = StudyXmlTagInclusion.IgnoreTag,
                IncludeUnknownTags = StudyXmlTagInclusion.IgnoreTag,
                IncludeLargeTags = StudyXmlTagInclusion.IncludeTagExclusion,
                MaxTagLength = 2048,
                IncludeSourceFileName = true
            };
            var doc = theStream.GetMemento(_settings);
            string streamFile = Path.Combine(StudyFolder, string.Format("{0}.xml", Study.StudyInstanceUid));
            string gzStreamFile = Path.Combine(StudyFolder, string.Format("{0}.xml.gz", Study.StudyInstanceUid));

            // allocate the random number generator here, in case we need it below
            var rand = new Random();
            string tmpStreamFile = streamFile + "_tmp";
            string tmpGzStreamFile = gzStreamFile + "_tmp";
            for (int i = 0; ; i++)
                try
                {
                    if (File.Exists(tmpStreamFile))
                        FileUtils.Delete(tmpStreamFile);
                    if (File.Exists(tmpGzStreamFile))
                        FileUtils.Delete(tmpGzStreamFile);

                    using (FileStream xmlStream = FileStreamOpener.OpenForSoleUpdate(tmpStreamFile, FileMode.CreateNew),
                                      gzipStream = FileStreamOpener.OpenForSoleUpdate(tmpGzStreamFile, FileMode.CreateNew))
                    {
                        StudyXmlIo.WriteXmlAndGzip(doc, xmlStream, gzipStream);
                        xmlStream.Close();
                        gzipStream.Close();
                    }

                    File.Copy(tmpStreamFile, streamFile, true);
                    FileUtils.Delete(tmpStreamFile);

                    FileUtils.Copy(tmpGzStreamFile, gzStreamFile, true);
                    FileUtils.Delete(tmpGzStreamFile);
                    return;
                }
                catch (IOException)
                {
                    if (i < 5)
                    {
                        Thread.Sleep(rand.Next(5, 50)); // Sleep 5-50 milliseconds
                        continue;
                    }

                    throw;
                }
        }
        #endregion

        private static string GetFileStoreDirectory()
        {
            string directory = null;
            Platform.GetService<IDicomServerConfiguration>(
                s => directory = s.GetConfiguration(new GetDicomServerConfigurationRequest()).Configuration.FileStoreDirectory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }
    }
}
