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
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    /// <summary>
    /// Class for processing TierMigrate <see cref="WorkQueue"/> entries.
    /// </summary>
    class TierMigrateItemProcessor : BaseItemProcessor
    {
        #region Private static members
        private static int _sessionStudiesMigrate = 0;
        private static TierMigrationAverageStatistics _averageStatisics = new TierMigrationAverageStatistics();
        #endregion

        #region Private Members
        private readonly StatisticsSet _statistics = new StatisticsSet("TierMigration");
        
        #endregion

        /// <summary>
        /// Simple routine for failing a work queue item.
        /// </summary>
        /// <param name="item">The item to fail.</param>
        /// <param name="failureDescription">The reason for the failure.</param>
        protected override void FailQueueItem(Model.WorkQueue item, string failureDescription)
        {
            DBUpdateTime.Add(
                delegate
                {
                    using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
                        UpdateWorkQueueParameters parms = new UpdateWorkQueueParameters();
                        parms.ProcessorID = ServiceTools.ProcessorId;

                        parms.WorkQueueKey = item.GetKey();
                        parms.StudyStorageKey = item.StudyStorageKey;
                        parms.FailureCount = item.FailureCount + 1;
                        parms.FailureDescription = failureDescription;

                        Platform.Log(LogLevel.Error,
                                     "Failing {0} WorkQueue entry ({1}): {2}", 
                                     item.WorkQueueTypeEnum, 
                                     item.GetKey(), failureDescription);

                        parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
                        parms.ScheduledTime = Platform.Time;
                        parms.ExpirationTime = Platform.Time.AddDays(1);

                        if (false == update.Execute(parms))
                        {
                            Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}", item.WorkQueueTypeEnum,
                                         item.GetKey().ToString());
                        }
                        else
                            updateContext.Commit();
                    }
                }
                );


        }

        
        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");

            //Load the storage location.
			if (!LoadStorageLocation(item))
			{
				Platform.Log(LogLevel.Warn, "Unable to find readable location when processing TierMigrate WorkQueue item, rescheduling");
				PostponeItem(item, item.ScheduledTime.AddMinutes(2), item.ExpirationTime.AddMinutes(2));
				return;
			}
            else
            {
                try
                {
                    DoMigrateStudy(StorageLocation);
                    PostProcessing(item, false, true);
                }
                catch (Exception e)
                {
                    FailQueueItem(item, e.Message);
                }
            }
            
            
        }

        private void DoMigrateStudy(StudyStorageLocation storage)
        {
            TierMigrationStatistics stat = new TierMigrationStatistics();
            stat.StudyInstanceUid = storage.StudyInstanceUid;
            stat.ProcessSpeed.Start();

            long size = (long) DirectoryUtility.CalculateFolderSize(storage.GetStudyPath());

            Platform.Log(LogLevel.Info, "Migrating study {0} from {1}", storage.StudyInstanceUid, storage.FilesystemTierEnum);
			ServerFilesystemInfo currFilesystem = FilesystemMonitor.Instance.GetFilesystemInfo(storage.FilesystemKey);
			ServerFilesystemInfo newFilesystem = FilesystemMonitor.Instance.GetLowerTierFilesystemForStorage(currFilesystem);

            if (newFilesystem == null)
            {
            	// this entry shouldn't have been scheduled in the first place.
                String msg =
                    String.Format(
                        "Study '{0}' cannot be migrated: no writable filesystem can be found in lower tiers for filesystem '{1}'",
                        storage.StudyInstanceUid,
                        currFilesystem.Filesystem.Description);

                throw new ApplicationException(msg);
            }

            ServerCommandProcessor _processor;
            TimeSpanStatistics dbUpdateTime;
            TimeSpanStatistics fileCopyTime;

            using (_processor = new ServerCommandProcessor("Migrate Study"))
            {
                TierMigrationContext context = new TierMigrationContext();
                context.OriginalStudyLocation = storage;
                context.Destination = newFilesystem;

                TierMigrateMoveStudyFolderCommand moveStudyFolderCommand = new TierMigrateMoveStudyFolderCommand(context);
                TierMigrateDatabaseUpdateCommand updateDBCommand = new TierMigrateDatabaseUpdateCommand(context);


                // TODO: Add some kind of progress indication.

                _processor.AddCommand(moveStudyFolderCommand);
                _processor.AddCommand(updateDBCommand);

                if (!_processor.Execute())
                {
                    throw new ApplicationException(_processor.FailureReason);
                }
                fileCopyTime = moveStudyFolderCommand.Statistics;
                dbUpdateTime = updateDBCommand.Statistics;
            }

            Platform.Log(LogLevel.Info, "Successfully migrated study {0} from {1} to {2}",
                            storage.StudyInstanceUid, storage.FilesystemTierEnum, newFilesystem.Filesystem.FilesystemTierEnum);

            stat.ProcessSpeed.SetData(size);
            stat.ProcessSpeed.End();
           
            _averageStatisics.AverageProcessSpeed.AddSample(stat.ProcessSpeed);
            _averageStatisics.AverageDBUpdateTime.AddSample(dbUpdateTime);
            _averageStatisics.AverageFileMoveTime.AddSample(fileCopyTime);
            _averageStatisics.AverageStudySize.AddSample(size);
            _statistics.AddSubStats(stat);

            _sessionStudiesMigrate++;

            if (_sessionStudiesMigrate%5==0)
            {
                StatisticsLogger.Log(LogLevel.Info, _averageStatisics);
                _averageStatisics = new TierMigrationAverageStatistics();
            }
            
                
        }



        protected override bool CannotStart()
        {
            WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(WorkQueueItem.StudyStorageKey);
            workQueueCriteria.WorkQueueTypeEnum.In(new WorkQueueTypeEnum[] { WorkQueueTypeEnum.StudyProcess, WorkQueueTypeEnum.ReconcileStudy });
            workQueueCriteria.WorkQueueStatusEnum.In(new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Idle, WorkQueueStatusEnum.InProgress, WorkQueueStatusEnum.Pending });

            List<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(WorkQueueItem, workQueueCriteria);
            return relatedItems != null && relatedItems.Count > 0;
        }
    }
}
