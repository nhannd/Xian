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
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Xml;
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
        #region Private Static Members
        private static readonly Object _statisticsLock = new Object();
        private static TierMigrationAverageStatistics _average = new TierMigrationAverageStatistics();
        private static int _studiesMigratedCount = 0;

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

			try
			{
				Platform.Log(LogLevel.Info,
				             "Starting Tier Migration of study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
				             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
				             Study.AccessionNumber, ServerPartition.Description);

				DoMigrateStudy(StorageLocation);

				PostProcessing(item,
				               WorkQueueProcessorStatus.Complete,
				               WorkQueueProcessorDatabaseUpdate.ResetQueueState);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Info, e);
				FailQueueItem(item, e.Message);
			}
		}

    	private static void DoMigrateStudy(StudyStorageLocation storage)
        {
            TierMigrationStatistics stat = new TierMigrationStatistics();
            stat.StudyInstanceUid = storage.StudyInstanceUid;
            stat.ProcessSpeed.Start();
    	    StudyXml studyXml = storage.LoadStudyXml();
            stat.StudySize = (ulong) studyXml.GetStudySize(); 

            Platform.Log(LogLevel.Info, "About to migrate study {0} from {1}", storage.StudyInstanceUid, storage.FilesystemTierEnum);
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

            Platform.Log(LogLevel.Info, "New filesystem : {0}", newFilesystem.Filesystem.Description);
            string newPath = Path.Combine(newFilesystem.Filesystem.FilesystemPath, storage.PartitionFolder);
                
            using (ServerCommandProcessor processor = new ServerCommandProcessor("Migrate Study"))
            {
                TierMigrationContext context = new TierMigrationContext();
                context.OriginalStudyLocation = storage;
                context.Destination = newFilesystem;

                string origFolder = context.OriginalStudyLocation.GetStudyPath();
                processor.AddCommand(new CreateDirectoryCommand(newPath));

                newPath = Path.Combine(newPath, context.OriginalStudyLocation.StudyFolder);
                processor.AddCommand(new CreateDirectoryCommand(newPath));

                newPath = Path.Combine(newPath, context.OriginalStudyLocation.StudyInstanceUid);
                // don't create this directory so that it won't be backed up by MoveDirectoryCommand

                MoveDirectoryCommand moveCommand = new MoveDirectoryCommand(origFolder, newPath);
                processor.AddCommand(moveCommand);
                
                TierMigrateDatabaseUpdateCommand updateDBCommand = new TierMigrateDatabaseUpdateCommand(context);
                processor.AddCommand(updateDBCommand);

                Platform.Log(LogLevel.Info, "Start migrating study {0}.. {1} to be moved", storage.StudyInstanceUid, ByteCountFormatter.Format(stat.StudySize));
                if (!processor.Execute())
                {
                    throw new ApplicationException(processor.FailureReason);
                }

                stat.DBUpdate = updateDBCommand.Statistics;
                stat.CopyFiles = moveCommand.MoveSpeed;
            }

            stat.ProcessSpeed.SetData(stat.StudySize);
            stat.ProcessSpeed.End();

            Platform.Log(LogLevel.Info, "Successfully migrated study {0} from {1} to {2} in {7} [ {3} images, {4} @ {5}]. DB Update={6}",
                            storage.StudyInstanceUid, storage.FilesystemTierEnum, 
                            newFilesystem.Filesystem.FilesystemTierEnum, 
                            studyXml.NumberOfStudyRelatedInstances,
                            ByteCountFormatter.Format(stat.StudySize),
                            stat.CopyFiles.FormattedValue,
                            stat.DBUpdate.FormattedValue,
                            TimeSpanFormatter.Format(stat.ProcessSpeed.ElapsedTime));

            UpdateAverageStatistics(stat);                       
        }

        private static void UpdateAverageStatistics(TierMigrationStatistics stat)
        {
            lock(_statisticsLock)
            {
                _average.AverageProcessSpeed.AddSample(stat.ProcessSpeed);
                _average.AverageStudySize.AddSample(stat.StudySize);
                _average.AverageStudySize.AddSample(stat.ProcessSpeed);
                _average.AverageFileMoveTime.AddSample(stat.CopyFiles);
                _average.AverageDBUpdateTime.AddSample(stat.DBUpdate);
                _studiesMigratedCount++;
                if (_studiesMigratedCount % 5 ==0)
                {
                    StatisticsLogger.Log(LogLevel.Info, _average);
                    _average = new TierMigrationAverageStatistics();
                }
            }
        }

        

        protected override bool CanStart()
        {
			WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
			workQueueCriteria.StudyStorageKey.EqualTo(WorkQueueItem.StudyStorageKey);
			workQueueCriteria.WorkQueueTypeEnum.In(new WorkQueueTypeEnum[]
			                                           {
			                                               WorkQueueTypeEnum.StudyProcess, 
                                                           WorkQueueTypeEnum.ReconcileStudy, 
                                                           WorkQueueTypeEnum.ProcessDuplicate,
                                                           WorkQueueTypeEnum.ReconcilePostProcess,
                                                           WorkQueueTypeEnum.ReconcileCleanup
                                                        });
			List<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(WorkQueueItem, workQueueCriteria);

            IList<StudyIntegrityQueue> reconcileList = FindSIQEntries();
            
            if ((relatedItems == null || relatedItems.Count == 0) && (reconcileList == null || reconcileList.Count == 0))
                return true; // nothing related in the work queue and nothing in the reconcile queue
            

            Platform.Log(LogLevel.Info,
						 "Tier Migrate entry for study {0} has conflicting WorkQueue entry, reinserting into FilesystemQueue",
						 StorageLocation.StudyInstanceUid);
		
			using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				IWorkQueueUidEntityBroker broker = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
				WorkQueueUidSelectCriteria workQueueUidCriteria = new WorkQueueUidSelectCriteria();
				workQueueUidCriteria.WorkQueueKey.EqualTo(WorkQueueItem.Key);
				broker.Delete(workQueueUidCriteria);

				FilesystemQueueInsertParameters parms = new FilesystemQueueInsertParameters();
				parms.FilesystemQueueTypeEnum = FilesystemQueueTypeEnum.TierMigrate;
				parms.ScheduledTime = Platform.Time.AddMinutes(10);
				parms.StudyStorageKey = WorkQueueItem.StudyStorageKey;
				parms.FilesystemKey = StorageLocation.FilesystemKey;

				IInsertFilesystemQueue insertQueue = updateContext.GetBroker<IInsertFilesystemQueue>();

				if (false == insertQueue.Execute(parms))
				{
					Platform.Log(LogLevel.Error, "Unexpected failure inserting FilesystemQueue entry");
				}
				else
					updateContext.Commit();
			}

			PostProcessing(WorkQueueItem, 
				WorkQueueProcessorStatus.Complete, 
				WorkQueueProcessorDatabaseUpdate.ResetQueueState);
			return false;
		}
    }
}
