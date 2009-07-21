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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Helper class shared by all code.
    /// </summary>
    public static class ServerHelper
    {
        /// <summary>
        /// Returns the name of the directory in the filesytem
        /// where the study with the specified information will be stored.
        /// </summary>
        /// <returns></returns>
        /// 
        public static string ResolveStorageFolder(ServerPartition partition, string studyInstanceUid, string studyDate, IPersistenceContext persistenceContext,bool checkExisting)
        {
            string folder;

            if (checkExisting)
            {
                StudyStorage storage = StudyStorage.Load(persistenceContext, partition.Key, studyInstanceUid);
                if (storage != null)
                {
                    folder = ImageServerCommonConfiguration.UseReceiveDateAsStudyFolder
                                 ? storage.InsertTime.ToString("yyyyMMdd")
                                 : String.IsNullOrEmpty(studyDate)
                                       ? ImageServerCommonConfiguration.DefaultStudyRootFolder
                                       : studyDate;
                    return folder;
                }
            }

            folder = ImageServerCommonConfiguration.UseReceiveDateAsStudyFolder
                         ? Platform.Time.ToString("yyyyMMdd")
                         : String.IsNullOrEmpty(studyDate)
                               ? ImageServerCommonConfiguration.DefaultStudyRootFolder
                               : studyDate;
            return folder;
        }

        /// <summary>
        /// Checks for a storage location for the study in the database, and creates a new location
        /// in the database if it doesn't exist.
        /// </summary>
        /// <param name="message">The DICOM message to create the storage location for.</param>
        /// <param name="partition">The partition where the study is being sent to</param>
        /// <returns>A <see cref="StudyStorageLocation"/> instance.</returns>
        static public StudyStorageLocation GetWritableStudyStorageLocation(DicomMessageBase message, ServerPartition partition)
        {
            String studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
            String studyDate = message.DataSet[DicomTags.StudyDate].GetString(0, "");

            FilesystemSelector selector = new FilesystemSelector(FilesystemMonitor.Instance);
            ServerFilesystemInfo filesystem = selector.SelectFilesystem(message);
            if (filesystem == null)
            {
                Platform.Log(LogLevel.Error, "Unable to select location for storing study.");

                return null;
            }

            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IQueryStudyStorageLocation locQuery = updateContext.GetBroker<IQueryStudyStorageLocation>();
                StudyStorageLocationQueryParameters locParms = new StudyStorageLocationQueryParameters();
                locParms.StudyInstanceUid = studyInstanceUid;
                locParms.ServerPartitionKey = partition.GetKey();
                IList<StudyStorageLocation> studyLocationList = locQuery.Find(locParms);

                if (studyLocationList.Count == 0)
                {
                    StudyStorage storage = StudyStorage.Load(updateContext, partition.Key, studyInstanceUid);
                    if (storage != null)
                    {
                        Platform.Log(LogLevel.Warn, "Study in {0} state.  Rejecting image.", storage.StudyStatusEnum.Description);
                        return null;
                    }

                    IInsertStudyStorage locInsert = updateContext.GetBroker<IInsertStudyStorage>();
                    InsertStudyStorageParameters insertParms = new InsertStudyStorageParameters();
                    insertParms.ServerPartitionKey = partition.GetKey();
                    insertParms.StudyInstanceUid = studyInstanceUid;
                    
                    insertParms.Folder = ResolveStorageFolder(partition, studyInstanceUid, studyDate, updateContext, false /* set to false for optimization because we are sure it's not in the system */);
                    insertParms.FilesystemKey = filesystem.Filesystem.GetKey();
                    insertParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;

                    if (message.TransferSyntax.LosslessCompressed)
                    {
                        insertParms.TransferSyntaxUid = message.TransferSyntax.UidString;
                        insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossless;
                    }
                    else if (message.TransferSyntax.LossyCompressed)
                    {
                        insertParms.TransferSyntaxUid = message.TransferSyntax.UidString;
                        insertParms.StudyStatusEnum = StudyStatusEnum.OnlineLossy;
                    }
                    else
                    {
                        insertParms.TransferSyntaxUid = TransferSyntax.ExplicitVrLittleEndianUid;
                        insertParms.StudyStatusEnum = StudyStatusEnum.Online;
                    }

                    studyLocationList = locInsert.Find(insertParms);

                    updateContext.Commit();
                }
                else
                {
                    if (!FilesystemMonitor.Instance.CheckFilesystemWriteable(studyLocationList[0].FilesystemKey))
                    {
                        Platform.Log(LogLevel.Warn, "Unable to find writable filesystem for study {0} on Partition {1}",
                                     studyInstanceUid, partition.Description);
                        return null;
                    }
                }

                //TODO:  Do we need to do something to identify a primary storage location?
                // Also, should the above check for writeable location check the other availab
                return studyLocationList[0];
            }
        }

        /// <summary>
        /// Insert a request to restore the specified <seealso cref="StudyStorage"/>
        /// </summary>
        /// <param name="storage"></param>
        /// <returns>Reference to the <see cref="RestoreQueue"/> that was inserted.</returns>
        static public RestoreQueue InsertRestoreRequest(StudyStorage storage)
        {
            // TODO:
            // Check the stored procedure to see if it will insert another request if one already exists

            Platform.CheckForNullReference(storage, "storage");

            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IInsertRestoreQueue broker = updateContext.GetBroker<IInsertRestoreQueue>();

                InsertRestoreQueueParameters parms = new InsertRestoreQueueParameters();
                parms.StudyStorageKey = storage.Key;

                RestoreQueue queue = broker.FindOne(parms);

                if (queue == null)
                {
                    Platform.Log(LogLevel.Error, "Unable to request restore for study {0}", storage.StudyInstanceUid);
                    return null;
                }

                updateContext.Commit();
                Platform.Log(LogLevel.Info, "Restore requested for study {0}", storage.StudyInstanceUid);
                return queue;
            }
        }

        /// <summary>
        /// Insert a request to restore the specified <seealso cref="StudyStorageLocation"/>
        /// </summary>
        /// <param name="storageLocation"></param>
        /// <returns>Reference to the <see cref="RestoreQueue"/> that was inserted.</returns>
        static public RestoreQueue InsertRestoreRequest(StudyStorageLocation storageLocation)
        {
            Platform.CheckForNullReference(storageLocation, "storageLocation");

            return InsertRestoreRequest(storageLocation.StudyStorage);
        }

        /// <summary>
        /// Finds a list of <see cref="StudyIntegrityQueue"/> related to the specified <see cref="StudyStorage"/>.
        /// </summary>
        /// <param name="studyStorage"></param>
        /// <param name="filter">A delegate that will be used to filter the returned list. Pass in Null to get the entire list.</param>
        /// <returns>A list of  <see cref="StudyIntegrityQueue"/></returns>
        static public IList<StudyIntegrityQueue> FindSIQEntries(StudyStorage studyStorage, Predicate<StudyIntegrityQueue> filter)
        {
            using (ExecutionContext scope = new ExecutionContext())
            {
                IStudyIntegrityQueueEntityBroker broker = scope.PersistenceContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
                StudyIntegrityQueueSelectCriteria criteria = new StudyIntegrityQueueSelectCriteria();
                criteria.StudyStorageKey.EqualTo(studyStorage.Key);
                criteria.InsertTime.SortDesc(0);
                IList<StudyIntegrityQueue> list = broker.Find(criteria);
                if (filter != null)
                {
                    CollectionUtils.Remove(list, filter);
                }
                return list;
            }
        }

        /// <summary>
        /// Finds a list of <see cref="WorkQueue"/> related to the specified <see cref="studyStorage"/>.
        /// </summary>
        /// <param name="studyStorage"></param>
        /// <param name="filter">A delegate that will be used to filter the returned list. Pass in Null to get the entire list.</param>
        /// <returns>A list of  <see cref="WorkQueue"/></returns>
        static public IList<WorkQueue> FindWorkQueueEntries(StudyStorage studyStorage, Predicate<WorkQueue> filter)
        {
            Platform.CheckForNullReference(studyStorage, "studyStorage");

            using (ExecutionContext scope = new ExecutionContext())
            {
                IWorkQueueEntityBroker broker = scope.PersistenceContext.GetBroker<IWorkQueueEntityBroker>();
                WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();
                criteria.StudyStorageKey.EqualTo(studyStorage.Key);
                criteria.InsertTime.SortDesc(0);
                IList<WorkQueue> list = broker.Find(criteria);
                if (filter != null)
                {
                    CollectionUtils.Remove(list, filter);
                }
                return list;
            }
        }

        /// <summary>
        /// Finds a list of <see cref="StudyHistory"/> records of the specified <see cref="StudyHistoryTypeEnum"/> 
        /// for the specified <see cref="StudyStorage"/>.
        /// </summary>
        /// <param name="studyStorage"></param>
        /// <returns></returns>
        static public IList<StudyHistory> FindStudyHistories(StudyStorage studyStorage, IEnumerable<StudyHistoryTypeEnum> types)
        {
            // Use of ExecutionContext to re-use db connection if possible
            using (ExecutionContext scope = new ExecutionContext())
            {
                IStudyHistoryEntityBroker broker = scope.PersistenceContext.GetBroker<IStudyHistoryEntityBroker>();
                StudyHistorySelectCriteria criteria = new StudyHistorySelectCriteria();
                criteria.StudyStorageKey.EqualTo(studyStorage.Key);
                criteria.StudyHistoryTypeEnum.EqualTo(StudyHistoryTypeEnum.StudyReconciled);

                if (types!=null)
                {
                    criteria.StudyHistoryTypeEnum.In(types);
                }

                criteria.InsertTime.SortAsc(0);
                IList<StudyHistory> historyList = broker.Find(criteria);
                return historyList;
            } 
        }

        /// <summary>
        /// Finds all <see cref="StudyHistory"/> records for the specified <see cref="StudyStorage"/>.
        /// </summary>
        /// <param name="studyStorage"></param>
        /// <returns></returns>
        static public IList<StudyHistory> FindStudyHistories(StudyStorage studyStorage)
        {
            return FindStudyHistories(studyStorage, null);
        }

        static public StudyStorageLocation GetStudyOnlineStorageLocation(StudyStorage studyStorage, out bool restoreRequested)
        {
            restoreRequested = false;

            //Load a list of locations from the db
            IList<StudyStorageLocation> locations = StudyStorageLocation.FindStorageLocations(studyStorage);
            if (locations == null || locations.Count==0)
            {
                if (studyStorage.StudyStatusEnum.Equals(StudyStatusEnum.Nearline))
                {
                    RestoreQueue restoreQueue = ServerHelper.InsertRestoreRequest(studyStorage);
                    restoreRequested = restoreQueue != null;
                }
                return null;
            }

            return locations[0];

        }

        /// <summary>
        /// Gets a string value that represents the group ID for a <see cref="DicomFile"/> based on
        /// the source/destination AE title and the application-provided timestamp.
        /// </summary>
        /// <param name="file">The <see cref="DicomFile"/></param>
        /// <param name="partition">The <see cref="ServerPartition"/> where the file belongs to</param>
        /// <param name="timestamp">Optional timestamp to be used to generate the group ID. 
        /// If null, the current timestamp will be used.</param>
        /// <returns></returns>
        public static string GetUidGroup(DicomFile file, ServerPartition partition, DateTime? timestamp)
        {
            return String.Format("{0}_{1}",
                                 String.IsNullOrEmpty(file.SourceApplicationEntityTitle)
                                     ? partition.AeTitle
                                     : file.SourceApplicationEntityTitle,
                                     timestamp!=null? timestamp.Value.ToString("yyyyMMddHHmmss"): Platform.Time.ToString("yyyyMMddHHmmss"));
        }
    }
}
