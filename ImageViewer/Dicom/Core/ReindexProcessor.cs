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
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core
{
    /// <summary>
    /// Class for performing a Reindex of the database.
    /// </summary>
    public class ReindexProcessor
    {
        #region Private members

        private event EventHandler<StudyEventArgs> _studyDeletedEvent;
        private event EventHandler<StudyEventArgs> _studyFolderProcessedEvent;
        private event EventHandler<StudyEventArgs> _studyProcessedEvent;
        private readonly object _syncLock = new object();

        #endregion

        #region Public Events

        public class StudyEventArgs : EventArgs
        {
            public string StudyInstanceUid;
        }    
    

        public event EventHandler<StudyEventArgs> StudyDeletedEvent
        {
            add
            {
                lock (_syncLock)
                {
                    _studyDeletedEvent += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _studyDeletedEvent -= value;
                }
            }
        }

        public event EventHandler<StudyEventArgs> StudyFolderProcessedEvent
        {
            add
            {
                lock (_syncLock)
                {
                    _studyFolderProcessedEvent += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _studyFolderProcessedEvent -= value;
                }
            }
        }

        public event EventHandler<StudyEventArgs> StudyProcessedEvent
        {
            add
            {
                lock (_syncLock)
                {
                    _studyProcessedEvent += value;
                }
            }
            remove
            {
                lock (_syncLock)
                {
                    _studyProcessedEvent -= value;
                }
            }
        }

        #endregion

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
                DirectoryList = new List<string>(Directory.GetDirectories(FilestoreDirectory));
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x);
                throw;
            }

            StudyFoldersToScan = DirectoryList.Count;

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetStudyBroker();

                StudyOidList = new List<long>(); 
                
                var studyList = broker.GetStudies();
                foreach (var study in studyList)
                {
                    study.Deleted = true;
                    StudyOidList.Add(study.Oid);
                }
                context.Commit();
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

            // Before scanning the study folders, cleanup any empty directories.
            CleanupFilestoreDirectory();
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
                    using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
                    {
                        var broker = context.GetStudyBroker();
                        var workItemBroker = context.GetWorkItemBroker();
                        var workItemUidBroker = context.GetWorkItemUidBroker();

                        var study = broker.GetStudy(oid);

                        var location = new StudyLocation(study.StudyInstanceUid);
                        if (!Directory.Exists(location.StudyFolder))
                        {
                            broker.Delete(study);
                            var workItemList = workItemBroker.GetWorkItems(null, null, study.StudyInstanceUid);
                            foreach (var item in workItemList)
                            {
                                Platform.Log(LogLevel.Info, "Deleting WorkItem for Study {0}, OID: {1}",
                                             study.StudyInstanceUid, item.Oid);

                                foreach (var uid in item.WorkItemUids)
                                {
                                    workItemUidBroker.Delete(uid);
                                }

                                workItemBroker.Delete(item);
                            }

                            context.Commit();
                            EventsHelper.Fire(_studyDeletedEvent, this, new StudyEventArgs { StudyInstanceUid = study.StudyInstanceUid });
                            Platform.Log(LogLevel.Info, "Deleted Study that wasn't on disk, but in the database: {0}",
                                         study.StudyInstanceUid);
                        }
                        else
                            EventsHelper.Fire(_studyProcessedEvent, this, new StudyEventArgs { StudyInstanceUid = study.StudyInstanceUid });
                    }                    
                }
                catch (Exception x)
                {
                    Platform.Log(LogLevel.Warn, "Unexpected exception attempting to reindex StudyOid {0}: {1}", oid, x.Message);
                }
            }
        }

        private void ProcessFilesystem()
        {
            foreach (string studyFolder in DirectoryList)
            {
                ProcessStudyFolder(studyFolder);
            }
        }

        private void ProcessStudyFolder(string folder)
        {
            try
            {
                string studyInstanceUid = Path.GetFileName(folder);
                var location = new StudyLocation(studyInstanceUid);

                var reprocessor = new ReprocessStudyFolder(location);
       
                reprocessor.Process();
                
                if (!reprocessor.StudyStoredInDatabase)                
                    EventsHelper.Fire(_studyFolderProcessedEvent, this, new StudyEventArgs { StudyInstanceUid = studyInstanceUid });                
                else
                    EventsHelper.Fire(_studyProcessedEvent, this, new StudyEventArgs {StudyInstanceUid = studyInstanceUid});
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x, "Unexpected exception reindexing folder: {0}", folder);
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
