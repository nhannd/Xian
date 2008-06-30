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
        static DateTime? _scheduledMigrateTime = null;
            
        private FilesystemMonitor _monitor;
        private float _bytesToRemove;
        private int _studiesDeleted= 0;
        private int _studiesMigrated = 0;
        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the scheduled time based on the last entry in the queue.
        /// </summary>
        private void InitializeScheduleTime()
        {
            if (_scheduledMigrateTime == null)
            {
                IWorkQueueEntityBroker workQueueBroker = ReadContext.GetBroker<IWorkQueueEntityBroker>();
                WorkQueueSelectCriteria workQueueSearchCriteria = new WorkQueueSelectCriteria();
                workQueueSearchCriteria.WorkQueueTypeEnum.EqualTo(WorkQueueTypeEnum.MigrateStudy);
                workQueueSearchCriteria.WorkQueueStatusEnum.In(new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Pending, WorkQueueStatusEnum.InProgress });
                workQueueSearchCriteria.ScheduledTime.SortDesc(0);

                IList<Model.WorkQueue> migrateItems = workQueueBroker.Find(workQueueSearchCriteria, 1);

                if (migrateItems != null && migrateItems.Count > 0)
                {
                    _scheduledMigrateTime = migrateItems[0].ScheduledTime;
                    Platform.Log(LogLevel.Info, "Last migration entry was scheduled for {0}. New migration will be scheduled after that time.", _scheduledMigrateTime.Value);

                }
                else
                    _scheduledMigrateTime = Platform.Time;

            }


            if (_scheduledMigrateTime < Platform.Time)
                _scheduledMigrateTime = Platform.Time;
        }


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


        /// <summary>
        /// Process study migration candidates retrieved from the <see cref="Model.FilesystemQueue"/> table
        /// </summary>
        /// <param name="candidateList">The list of candidate studies for deleting.</param>
        private void ProcessStudyMigrateCandidates(IList<FilesystemQueue> candidateList)
        {
            Platform.Log(LogLevel.Debug, "Scheduling tier-migration for {0} studies...", candidateList.Count);
        
            foreach (FilesystemQueue queueItem in candidateList)
            {
                if (_bytesToRemove < 0)
                {
                    break;
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
                    insertParms.ScheduledTime = _scheduledMigrateTime.Value;
                    insertParms.ExpirationTime = _scheduledMigrateTime.Value.AddMinutes(1);
                    insertParms.DeleteFilesystemQueue = true;

                    Platform.Log(LogLevel.Debug, "Scheduling tier-migration for study {0} from {1} at {2}...",
                            location.StudyInstanceUid, location.FilesystemTierEnum, _scheduledMigrateTime);
                    if (false == broker.Execute(insertParms))
                    {
                        Platform.Log(LogLevel.Error, "Unexpected problem inserting 'MigrateStudy' record into WorkQueue for Study {0}", location.StudyInstanceUid);
                    }
                    else
                    {
                        update.Commit();
                        _bytesToRemove -= studySize;
                        _studiesMigrated++;

                        // spread out the scheduled migration entries based on the size
                        // assuming that the larger the study the longer it will take to migrate
                        // The assumed migration speed is arbitarily chosen.
                        double migrationSpeed = ServiceLockSettings.Default.TierMigrationSpeed * 1024 * 1024; // MB / sec
                        TimeSpan estMigrateTime = TimeSpan.FromSeconds(studySize/migrationSpeed);
                        _scheduledMigrateTime = _scheduledMigrateTime.Value.Add(estMigrateTime);
                        if (_studiesMigrated % 10 == 0)
                        {
                            _scheduledMigrateTime.Value.AddSeconds(15);
                            Thread.Sleep(200); // Since they are potentially let other processes have a chance to talk to the database
                        }
                            
                        
                    }
                }
                
                
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
            FilesystemQueueTypeEnum type = FilesystemQueueTypeEnum.TierMigrate;

            while (_bytesToRemove > 0)
            {
                Platform.Log(LogLevel.Debug,
                             "{2:0.0} MB need to be removed from '{2}'. Querying for studies on {0} that can be migrated on '{1}'",
                             fs.Filesystem.Description, Platform.Time, _bytesToRemove/(1024*1024), fs.Filesystem.Description);
                IList<FilesystemQueue> list = GetFilesystemQueueCandidates(item, Platform.Time, type);
                Platform.Log(LogLevel.Debug, "Found {0} studies eligible for migration from {1}", list.Count, fs.Filesystem.Description);
                
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

        /// <summary>
        /// Returns number of Delete Tier Migration work queue items that are still Pending or In-progress
        /// for the filesystem associated with the specified <see cref="ServiceLock"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int CheckWorkQueueMigrateCount(Model.ServiceLock item)
        {
            IWorkQueueEntityBroker select = ReadContext.GetBroker<IWorkQueueEntityBroker>();

            WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();

            criteria.WorkQueueTypeEnum.EqualTo(WorkQueueTypeEnum.MigrateStudy);
            criteria.WorkQueueStatusEnum.In(new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Pending, WorkQueueStatusEnum.InProgress});
            StorageFilesystemSelectCriteria storageCriteria = new StorageFilesystemSelectCriteria();

            storageCriteria.FilesystemKey.EqualTo(item.FilesystemKey);

            criteria.StudyFilesystemRelatedEntityCondition.Exists(storageCriteria);
            int count = select.Count(criteria);

            return count;
        }


        private void MigrateStudies(Model.ServiceLock item, ServerFilesystemInfo fs)
        {
            ServiceLockSettings settings = ServiceLockSettings.Default;

            ServerFilesystemInfo newFS = _monitor.GetLowerTierFilesystemForStorage(fs);
            if (newFS == null)
            {
                Platform.Log(LogLevel.Warn, "No writable storage in lower tiers. Tier-migration for '{0}' has effectively disabled at this time.", fs.Filesystem.Description);
                return;
            }

            // avoid overshoot
            if (CheckWorkQueueDeleteCount(item) > 0 || CheckWorkQueueMigrateCount(item) > 0)
            {
                Platform.Log(LogLevel.Info,
                             "Delaying study tier migration check, StudyDelete or MigrateStudy items still in the WorkQueue: {0} (Current: {1}, High Watermark: {2})",
                             fs.Filesystem.Description, fs.UsedSpacePercentage, fs.Filesystem.HighWatermark);
            }
            else
            {
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
                Platform.Log(LogLevel.Info, "{0} studies have been scheduled for migration from filesystem '{1}'",
                             _studiesMigrated, fs.Filesystem.Description, scheduledTime);
            }



        }

        private void DeleteStudies(Model.ServiceLock item, ServerFilesystemInfo fs)
        {
            ServiceLockSettings settings = ServiceLockSettings.Default;

            // avoid overshoot
            if (CheckWorkQueueDeleteCount(item) > 0 || CheckWorkQueueMigrateCount(item) > 0)
            {
                Platform.Log(LogLevel.Info, "Delaying study deletion check, StudyDelete or MigrateStudy items still in the WorkQueue: {0} (Current: {1}, High Watermark: {2})",
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


        #endregion

        #region Public Methods

        public void Process(Model.ServiceLock item)
        {
            _monitor = new FilesystemMonitor();
            _monitor.Load();

            ServerFilesystemInfo fs = _monitor.GetFilesystemInfo(item.FilesystemKey);

            InitializeScheduleTime();

            _bytesToRemove = _monitor.CheckFilesystemBytesToRemove(item.FilesystemKey);

            if (fs.AboveHighWatermark)
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
