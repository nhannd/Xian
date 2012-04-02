#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core
{
    /// <summary>
    /// Class for performing a Reindex of the database.
    /// </summary>
    public class ReindexProcessor
    {
        #region Public Properties

        public int StudyFoldersToScan { get; private set; }

        public int DatabaseStudiesToScan { get; private set; }

        public string FilestoreDirectory { get; private set; }

        public List<string> DirectoryList { get;private set; }

        public List<long> StudyOidList { get; private set; }

        #endregion

        #region Constructors

        public ReindexProcessor()
        {
            FilestoreDirectory = GetFileStoreDirectory();
            DirectoryList = new List<string>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize the Reindex.  Determine the number of studies in the database and the number of folders on disk to be used
        /// for progress.
        /// </summary>
        public void Initialize()
        {
            // Before scanning the study folders, cleanup any empty directories.
            CleanupFilestoreDirectory();

            try
            {
                FileProcessor.Process(FilestoreDirectory, "*.*", delegate(string file, out bool cancel)
                                                                     {
                                                                         if (Directory.Exists(file))
                                                                             DirectoryList.Add(file);

                                                                         cancel = false;

                                                                     }, false);
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x);
                throw;
            }

            StudyFoldersToScan = DirectoryList.Count;

            using (var context = new DataAccessContext())
            {
                var broker = context.GetStudyBroker();

                StudyOidList = broker.GetStudyOids();
            }

            DatabaseStudiesToScan = StudyOidList.Count;           
        }

        /// <summary>
        /// Process the Reindex.
        /// </summary>
        public void Process()
        {            
            ProcessStudiesInDatabase();

            ProcessFilesystem();            
        }

        #endregion

        #region Private Methods

        private void CleanupFilestoreDirectory()
        {
            try
            {
                DirectoryUtility.DeleteEmptySubDirectories(FilestoreDirectory, true);
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Warn, x, "Unexpected exception cleaning up empty subdirectories in filestore: {0}",
                             FilestoreDirectory);
            }
        }
        private void ProcessStudiesInDatabase()
        {
            foreach (long oid in StudyOidList)
            {
                try
                {
                    using (var context = new DataAccessContext())
                    {
                        var broker = context.GetStudyBroker();
                        var study = broker.GetStudy(oid);

                        // TODO (Marmot) structure of how studies are stored is abstracted away in 
                        // StudyLocation class, should we abstract here, also?
                        string studyFolder = Path.Combine(FilestoreDirectory, study.StudyInstanceUid);
                        
                        if (!Directory.Exists(studyFolder))
                        {
                            broker.Delete(study);
                            context.Commit();
                        }                        
                    }
                }
                catch (Exception x)
                {
                    Platform.Log(LogLevel.Warn, "Unexpected exception attempting to reindex StudyOid: {0}", oid);
                }
            }
        }

        private void ProcessFilesystem()
        {
            foreach (string studyFolder in DirectoryList)
            {
                ProcessStudy(studyFolder);
            }
        }

        private void ProcessStudy(string folder)
        {
            try
            {
                string studyInstanceUid = Path.GetDirectoryName(folder);
                StudyLocation location = new StudyLocation(studyInstanceUid);

                var studyXml = location.LoadStudyXml();

                FileProcessor.Process(FilestoreDirectory, "*.dcm", delegate(string file, out bool cancel)
                                                                       {
                                                                           try
                                                                           {
                                                                               var p = new SopInstanceProcessor(location);

                                                                               var theFile = new DicomFile(file);
                                                                               theFile.Load(DicomReadOptions.Default |
                                                                                            DicomReadOptions.StorePixelDataReferences);

                                                                               string sopInstanceUid =
                                                                                   Path.GetFileNameWithoutExtension(file);

                                                                               if (
                                                                                   !theFile.MediaStorageSopInstanceUid.
                                                                                        Equals(sopInstanceUid))
                                                                               {
                                                                                   File.Delete(file);
                                                                               }
                                                                               else
                                                                               {
                                                                                   p.ProcessFile(theFile, studyXml, null);
                                                                               }
                                                                           }
                                                                           catch (Exception x)
                                                                           {
                                                                               Platform.Log(LogLevel.Error, x);
                                                                           }


                                                                           cancel = false;

                                                                       }, false);
                bool fileCreated;
                location.SaveStudyXml(studyXml, out fileCreated);

            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x);
                throw;
            }
        }

        private static string GetFileStoreDirectory()
        {
            string directory = null;
            Platform.GetService<IDicomServerConfiguration>(
                s => directory = s.GetConfiguration(new GetDicomServerConfigurationRequest()).Configuration.FileStoreDirectory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        #endregion
    }
}
