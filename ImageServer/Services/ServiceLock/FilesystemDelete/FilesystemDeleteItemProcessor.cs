#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.SelectBrokers;

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
        #endregion

        #region Private Methods
        /// <summary>
        /// Update the Percentage full for a <see cref="Model.Filesystem"/> in the database.
        /// </summary>
        /// <param name="item">The <see cref="Model.ServiceLock"/> triggering this update.</param>
        private void UpdateFilesystemPercentFull(Model.ServiceLock item)
        {
            using (
                IUpdateContext update =
                    PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IUpdateFilesystem filesystemUpdate = update.GetBroker<IUpdateFilesystem>();

                FilesystemUpdateParameters updateParms = new FilesystemUpdateParameters();

                ServerFilesystemInfo fs = _monitor.Filesystems[item.FilesystemKey];
                updateParms.Description = fs.Filesystem.Description;
                updateParms.Enabled = fs.Filesystem.Enabled;
                updateParms.ReadOnly = fs.Filesystem.ReadOnly;
                updateParms.WriteOnly = fs.Filesystem.WriteOnly;
                updateParms.FileSystemKey = fs.Filesystem.GetKey();
                updateParms.FilesystemPath = fs.Filesystem.FilesystemPath;
                updateParms.FilesystemTierEnum = fs.Filesystem.FilesystemTierEnum;
                updateParms.HighWatermark = fs.Filesystem.HighWatermark;
                updateParms.LowWatermark = fs.Filesystem.LowWatermark;
                updateParms.PercentFull = (Decimal) fs.UsedSpacePercentage;

                if (false == filesystemUpdate.Execute(updateParms))
                {
                    Platform.Log(LogLevel.Error, "Unexpected problem updating Filesystem table with the Percent Full of the Filesystem");
                }
                else
                {
                    update.Commit();
                }
            }
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
                    }
                }
            }
        }

        private void DoStudyDelete(Model.ServiceLock item)
        {
            DateTime deleteTime = Platform.Time;
            FilesystemQueueTypeEnum type = FilesystemQueueTypeEnum.GetEnum("DeleteStudy");

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
                    // No candidates, no other choice but to look for candidates eligable the next day.
                    deleteTime = deleteTime.AddDays(1);
                    if (deleteTime > Platform.Time.AddDays(365))
                        break;
                }
            }
        }

        private int CheckWorkQueueDeleteCount(Model.ServiceLock item)
        {
            ISelectWorkQueue select = ReadContext.GetBroker<ISelectWorkQueue>();

            WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();

            criteria.WorkQueueTypeEnum.EqualTo(WorkQueueTypeEnum.GetEnum("DeleteStudy"));
            // Do Pending status, in case there's a Failure status entry, we don't want to 
            // block on that.
            criteria.WorkQueueStatusEnum.EqualTo(WorkQueueStatusEnum.GetEnum("Pending"));

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

            // Update the DB Percent full
            UpdateFilesystemPercentFull(item);

            ServerFilesystemInfo fs = _monitor.Filesystems[item.FilesystemKey];

            Platform.Log(LogLevel.Info, "Starting filesystem watermark check on filesystem {0} (High Watermark: {1}, Low Watermark: {2}",
                         fs.Filesystem.Description, fs.Filesystem.HighWatermark, fs.Filesystem.LowWatermark);

            if (_monitor.CheckFilesystemAboveHighWatermark(item.FilesystemKey))
            {
                if (CheckWorkQueueDeleteCount(item) > 0)
                {
                    Platform.Log(LogLevel.Info, "Delaying filesystem check, StudyDelete items still in the WorkQueue: {0} (Current: {1}, High Watermark: {2}",
                                 fs.Filesystem.Description, fs.Filesystem.PercentFull, fs.Filesystem.HighWatermark);

                    UnlockServiceLock(item, true, Platform.Time.AddMinutes(1));
                }
                else
                {
                    _bytesToRemove = _monitor.CheckFilesystemBytesToRemove(item.FilesystemKey);
                    int delayMinutes = 10;

                    try
                    {
                        DoStudyDelete(item);
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unexpected exception when processing StudyDelete records.");
                        delayMinutes = 1;
                    }

                    Platform.Log(LogLevel.Info, "Completed filesystem delete: {0} (Current: {1}, High Watermark: {2})",
                           fs.Filesystem.Description, fs.Filesystem.PercentFull, fs.Filesystem.HighWatermark);

                    UnlockServiceLock(item, true, Platform.Time.AddMinutes(delayMinutes));
                }
            }
            else if (_monitor.CheckFilesystemAboveLowWatermark(item.FilesystemKey))
            {
                Platform.Log(LogLevel.Info, "Filesystem below high watermark: {0} (Current: {1}, High Watermark: {2}",
                       fs.Filesystem.Description, fs.Filesystem.PercentFull, fs.Filesystem.HighWatermark);

                UnlockServiceLock(item, true, Platform.Time.AddMinutes(1));
            }
            else
            {
                Platform.Log(LogLevel.Info, "Filesystem below watermarks: {0} (Current: {1}, High Watermark: {2}",
                       fs.Filesystem.Description, fs.Filesystem.PercentFull, fs.Filesystem.HighWatermark);

                UnlockServiceLock(item, true, Platform.Time.AddMinutes(2));
            }
        }

        public new void Dispose()
        {
            _monitor.Dispose();
            _monitor = null;

            base.Dispose();
        }
        #endregion
    }
}
