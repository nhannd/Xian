#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemDelete
{
    /// <summary>
    /// Class for processing 'FilesystemDelete' <see cref="Model.ServiceLock"/> rows.
    /// </summary>
    public class FilesystemDeleteItemProcessor : BaseServiceLockItemProcessor, IServiceLockItemProcessor
    {
        #region Private Members
        private FilesystemMonitor _monitor;
        private float _bytesToRemove;
        private int _studiesDeleted= 0;
        private int _studiesMigrated = 0;
        #endregion

        #region Private Methods
        

        /// <summary>
        /// Process StudyDelete Candidates retrieved from the <see cref="Model.FilesystemQueue"/> table
        /// </summary>
        /// <param name="candidateList">The list of candidate studies for deleting.</param>
        private void ProcessStudyDeleteCandidates(IList<FilesystemQueue> candidateList)
        {
            foreach (FilesystemQueue queueItem in candidateList)
            {
                if (_bytesToRemove < 0)
                    return;

                // First, get the StudyStorage locations for the study, and calculate the disk usage.
                IQueryStudyStorageLocation studyStorageQuery = ReadContext.GetBroker<IQueryStudyStorageLocation>();
                StudyStorageLocationQueryParameters studyStorageParms = new StudyStorageLocationQueryParameters();
                studyStorageParms.StudyStorageKey = queueItem.StudyStorageKey;
                IList<StudyStorageLocation> storageList = studyStorageQuery.Execute(studyStorageParms);
                
                // Get the disk usage
                StudyStorageLocation location = storageList[0];

                List<ServerFilesystemInfo> newFilesystems= _monitor.FindNextTierFilesystems(_monitor.GetFilesystemInfo(location.FilesystemKey));

                if (newFilesystems != null && newFilesystems.Count > 0)
                {
                    float studySize = CalculateFolderSize(location.GetStudyPath());

                    using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        IInsertWorkQueueDeleteStudy studyDelete = update.GetBroker<IInsertWorkQueueDeleteStudy>();

                        WorkQueueDeleteStudyInsertParameters insertParms = new WorkQueueDeleteStudyInsertParameters();
                        insertParms.StudyStorageKey = location.GetKey();
                        insertParms.ServerPartitionKey = location.ServerPartitionKey;
                        DateTime expirationTime = Platform.Time.AddSeconds(10);
                        insertParms.ScheduledTime = expirationTime;
                        insertParms.ExpirationTime = expirationTime;
                        insertParms.DeleteFilesystemQueue = true;

                        if (false == studyDelete.Execute(insertParms))
                        {
                            Platform.Log(LogLevel.Error, "Unexpected problem inserting 'StudyDelete' record into WorkQueue for Study {0}", location.StudyInstanceUid);
                        }
                        else
                        {
                            update.Commit();
                            _bytesToRemove -= studySize;
                            _studiesDeleted++;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Process StudyDelete Candidates retrieved from the <see cref="Model.FilesystemQueue"/> table
        /// </summary>
        /// <param name="candidateList">The list of candidate studies for deleting.</param>
        private void ProcessStudyMigrateCandidates(IList<FilesystemQueue> candidateList)
        {
            Platform.Log(LogLevel.Info, "Scheduling tier-migration for {0} studies...", candidateList.Count);
            DateTime scheduleTime = Platform.Time.AddMinutes(1);
                    
            foreach (FilesystemQueue queueItem in candidateList)
            {
                if (_bytesToRemove < 0)
                {
                    Platform.Log(LogLevel.Info,
                                 "Stop further scheduling tier-migration: estimated size reduction has reached");
                    return;
                }

                    

                // First, get the StudyStorage locations for the study, and calculate the disk usage.
                IQueryStudyStorageLocation studyStorageQuery = ReadContext.GetBroker<IQueryStudyStorageLocation>();
                StudyStorageLocationQueryParameters studyStorageParms = new StudyStorageLocationQueryParameters();
                studyStorageParms.StudyStorageKey = queueItem.StudyStorageKey;
                IList<StudyStorageLocation> storageList = studyStorageQuery.Execute(studyStorageParms);

                // Get the disk usage
                StudyStorageLocation location = storageList[0]; // TODO: What should we do with other locations?

                float studySize = CalculateFolderSize(location.GetStudyPath());

                using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    IInsertWorkQueueMigrateStudy broker = update.GetBroker<IInsertWorkQueueMigrateStudy>();

                    WorkQueueMigrateStudyInsertParameters insertParms = new WorkQueueMigrateStudyInsertParameters();
                    insertParms.StudyStorageKey = location.GetKey();
                    insertParms.ServerPartitionKey = location.ServerPartitionKey;
                    insertParms.ScheduledTime = scheduleTime;
                    insertParms.ExpirationTime = scheduleTime.AddMinutes(1);
                    insertParms.DeleteFilesystemQueue = true;

                    Platform.Log(LogLevel.Info, "Scheduling tier-migration for study {0} from {1}...", location.StudyInstanceUid, location.FilesystemTierEnum);
                    if (false == broker.Execute(insertParms))
                    {
                        Platform.Log(LogLevel.Error, "Unexpected problem inserting 'MigrateStudy' record into WorkQueue for Study {0}", location.StudyInstanceUid);
                    }
                    else
                    {
                        update.Commit();
                        _bytesToRemove -= studySize;
                        _studiesMigrated++;

                        // spread out
                        double migrationSpeed = 1024*1024; // 1MB / sec
                        TimeSpan estMigrateTime = TimeSpan.FromSeconds(studySize/migrationSpeed);
                        scheduleTime = scheduleTime.Add(estMigrateTime.Add(TimeSpan.FromSeconds(5)));
                        
                    }
                }

                Thread.Sleep(200); // There will be tons of studies eligible.
            }
        }


        private void DoStudyDelete(Model.ServiceLock item)
        {
            DateTime deleteTime = Platform.Time;
            FilesystemQueueTypeEnum type = FilesystemQueueTypeEnum.DeleteStudy;

            while (_bytesToRemove > 0)
            {
                IList<FilesystemQueue> list =
                    GetFilesystemQueueCandidates(item, deleteTime, type);

                if (list.Count > 0)
                {
                    ProcessStudyDeleteCandidates(list);
                }
                else
                {
                    // No candidates
                    break;
                }
            }
        }

        private void DoStudyMigrate( ServerFilesystemInfo fs, Model.ServiceLock item)
        {
            DateTime migrateTime = Platform.Time;
            FilesystemQueueTypeEnum type = FilesystemQueueTypeEnum.TierMigrate;

            while (_bytesToRemove > 0)
            {
                Platform.Log(LogLevel.Info, "{2} MB need to be removed. Querying for studies on {0} that can be migrated on '{1}'", 
                                        fs.Filesystem.Description, migrateTime, _bytesToRemove / 1024*1024);
                IList<FilesystemQueue> list = GetFilesystemQueueCandidates(item, migrateTime, type);
                Platform.Log(LogLevel.Info, "Found {0} studies eligible for migration from {1}", list.Count, fs.Filesystem.Description);
                
                if (list.Count > 0)
                {
                    ProcessStudyMigrateCandidates(list);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Returns number of Delete Study work queue items that are still Pending
        /// for the filesystem associated with the specified <see cref="ServiceLock"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int CheckWorkQueueDeleteCount(Model.ServiceLock item)
        {
            IWorkQueueEntityBroker select = ReadContext.GetBroker<IWorkQueueEntityBroker>();

            WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();

            criteria.WorkQueueTypeEnum.EqualTo(WorkQueueTypeEnum.DeleteStudy);
            // Do Pending status, in case there's a Failure status entry, we don't want to 
            // block on that.
            criteria.WorkQueueStatusEnum.EqualTo(WorkQueueStatusEnum.Pending);

            StorageFilesystemSelectCriteria storageCriteria = new StorageFilesystemSelectCriteria();

            storageCriteria.FilesystemKey.EqualTo(item.FilesystemKey);

            criteria.StudyFilesystemRelatedEntityCondition.Exists(storageCriteria);
            int count = select.Count(criteria);

            return count;
        }

        private int CheckWorkQueueMigrateCount(Model.ServiceLock item)
        {
            IWorkQueueEntityBroker select = ReadContext.GetBroker<IWorkQueueEntityBroker>();

            WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();

            criteria.WorkQueueTypeEnum.EqualTo(WorkQueueTypeEnum.MigrateStudy);
            // Do Pending status, in case there's a Failure status entry, we don't want to 
            // block on that.
            criteria.WorkQueueStatusEnum.In(new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Pending, WorkQueueStatusEnum.InProgress});
            StorageFilesystemSelectCriteria storageCriteria = new StorageFilesystemSelectCriteria();

            storageCriteria.FilesystemKey.EqualTo(item.FilesystemKey);

            criteria.StudyFilesystemRelatedEntityCondition.Exists(storageCriteria);
            int count = select.Count(criteria);

            return count;
        }
        #endregion

        #region Public Methods
        public void Process(Model.ServiceLock item)
        {
            _monitor = new FilesystemMonitor();
            _monitor.Load();

            ServerFilesystemInfo fs = _monitor.GetFilesystemInfo(item.FilesystemKey);

            //Platform.Log(LogLevel.Info, "Starting filesystem watermark check on filesystem {0} (High Watermark: {1}, Low Watermark: {2}",
              //           fs.Filesystem.Description, fs.Filesystem.HighWatermark, fs.Filesystem.LowWatermark);


            _bytesToRemove = _monitor.CheckFilesystemBytesToRemove(item.FilesystemKey);

            if (NeedToShrink(item, fs))
            {
                MigrateStudies(item, fs);

                if (_studiesMigrated==0)
                    DeleteStudies(item, fs);
            }

            UnlockServiceLock(item, true, Platform.Time.AddMinutes(5));

            
            if (_monitor.CheckFilesystemAboveHighWatermark(item.FilesystemKey))
            {
                Platform.Log(LogLevel.Info, "Filesystem above high watermark: {0} (Current: {1}, High Watermark: {2}",
                             fs.Filesystem.Description, fs.UsedSpacePercentage, fs.Filesystem.HighWatermark);
            }
            else
            {
                Platform.Log(LogLevel.Info, "Filesystem below watermarks: {0} (Current: {1}, High Watermark: {2}",
                             fs.Filesystem.Description, fs.UsedSpacePercentage, fs.Filesystem.HighWatermark);
            }

            _monitor.Dispose();
            _monitor = null;

        }

        private bool NeedToShrink(Model.ServiceLock item,  ServerFilesystemInfo fs)
        {
            return fs.AboveHighWatermark;
        }

        private void MigrateStudies(Model.ServiceLock item, ServerFilesystemInfo fs)
        {
            List<ServerFilesystemInfo> newFS = _monitor.FindNextTierFilesystems(fs);
            if (newFS==null || newFS.Count==0)
            {
                // no next tier, don't schedule any migration
                return;
            }

            ServiceLockSettings settings = ServiceLockSettings.Default;
            
            try
            {
                DoStudyMigrate(fs, item);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when scheduling tier-migration.");
            }

            int delayMinutes = settings.FilesystemDeleteRecheckDelay;
            DateTime scheduledTime = Platform.Time.AddMinutes(delayMinutes);
            Platform.Log(LogLevel.Warn, "{0} studies have been scheduled for migration from filesystem '{1}'", _studiesMigrated, fs.Filesystem.Description, scheduledTime);
            
            
        }

        private void DeleteStudies(Model.ServiceLock item, ServerFilesystemInfo fs)
        {
            ServiceLockSettings settings = ServiceLockSettings.Default;
            
            if (fs.AboveHighWatermark)
            {
                if (CheckWorkQueueDeleteCount(item) > 0 || CheckWorkQueueMigrateCount(item)>0)
                {
                    Platform.Log(LogLevel.Info, "Delaying filesystem check, StudyDelete or MigrateStudy items still in the WorkQueue: {0} (Current: {1}, High Watermark: {2})",
                                 fs.Filesystem.Description, fs.UsedSpacePercentage, fs.Filesystem.HighWatermark);

                }
                else
                {
                    int delayMinutes = settings.FilesystemDeleteRecheckDelay;

                    Platform.Log(LogLevel.Info,
                                 "Filesystem above high watermark (Current: {0}, High Watermark: {1}).  Starting query for Filesystem delete candidates on '{2}'.",
                                 fs.UsedSpacePercentage, fs.Filesystem.HighWatermark, fs.Filesystem.Description);
                    try
                    {
                        DoStudyDelete(item);
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unexpected exception when processing StudyDelete records.");
                        delayMinutes = 1;
                    }

                    DateTime scheduledTime = Platform.Time.AddMinutes(delayMinutes);
                    Platform.Log(LogLevel.Warn, "{0} studies have been scheduled for removal from filesystem '{1}'", _studiesDeleted, fs.Filesystem.Description, scheduledTime);
                
                }
            }
        }

        public new void Dispose()
        {
            if (_monitor != null)
            {
                _monitor.Dispose();
                _monitor = null;
            }

            base.Dispose();
        }
        #endregion
    }
}
