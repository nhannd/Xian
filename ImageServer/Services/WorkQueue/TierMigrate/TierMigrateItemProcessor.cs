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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    /// <summary>
    /// States of the tier migration entry. Used for resume after auto-recovery.
    /// </summary>
    public enum TierMigrationProcessingState
    {
        NotStated,
        Migrated,
    }

    /// <summary>
    /// The data in the Data column.
    /// </summary>
    public class TierMigrationWorkQueueData
    {
        #region Private Members
        private TierMigrationProcessingState _state;
        
        #endregion

        #region Public Properties

        public TierMigrationProcessingState State
        {
            get { return _state; }
            set { _state = value; }
        } 
        #endregion
    }

    
    /// <summary>
    /// Class for processing TierMigrate <see cref="WorkQueue"/> entries.
    /// </summary>
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
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
                        parms.ProcessorID = ServerPlatform.ProcessorId;

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

            Platform.Log(LogLevel.Info,
                             "Starting Tier Migration of study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
                             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                             Study.AccessionNumber, ServerPartition.Description);

            // The WorkQueue Data column contains the state of the processing. It is set to "Migrated" if the entry has been
            // successfully executed once.  
            TierMigrationProcessingState state = GetCurrentState(item);
            switch(state)
            {
                case TierMigrationProcessingState.Migrated:
                    Platform.Log(LogLevel.Info, "Study has been migrated to {0} on {1}", StorageLocation.FilesystemPath, StorageLocation.FilesystemTierEnum.Description);
                    PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                    break;

                case TierMigrationProcessingState.NotStated:
                    ServerFilesystemInfo currFilesystem = FilesystemMonitor.Instance.GetFilesystemInfo(StorageLocation.FilesystemKey);
                    ServerFilesystemInfo newFilesystem = FilesystemMonitor.Instance.GetLowerTierFilesystemForStorage(currFilesystem);
                    
                    if (newFilesystem == null)
                    {
                        // This entry shouldn't have been scheduled in the first place.
                        // It's possible that the folder wasn't full when the entry was scheduled. 
                        // Another possiblity is the study was migrated but an error was encountered when updating the entry.//
                        // We should re-insert the filesystem queue so that if the study will be migrated if the space is freed up 
                        // in the future.
                        String msg = String.Format(
                                "Study '{0}' cannot be migrated: no writable filesystem can be found in lower tiers for filesystem '{1}'. Reschedule migration for future.",
                                StorageLocation.StudyInstanceUid, currFilesystem.Filesystem.Description);

                        Platform.Log(LogLevel.Warn, msg);
                        ReinsertFilesystemQueue();
                        PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                    }
                    else
                    {
                        DoMigrateStudy(StorageLocation, newFilesystem);

                        // Update the state separately so that if the validation (done in the PostProcessing method) fails, 
                        // we know the study has been migrated when we resume after auto-recovery has been completed.
                        UpdateState(item, TierMigrationProcessingState.Migrated);
                        PostProcessing(item, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                    
                    }
                    break;

                default:
                    throw new NotImplementedException("Not implemented");
            }

		}

        private static TierMigrationProcessingState GetCurrentState(Model.WorkQueue item)
        {
            if (item.Data == null)
            {
                //TODO: What about old entries?
                return TierMigrationProcessingState.NotStated;
            }
            else
            {
                TierMigrationWorkQueueData data = XmlUtils.Deserialize<TierMigrationWorkQueueData>(item.Data);
                return data.State;
            }
        }

        private static void UpdateState(Model.WorkQueue item, TierMigrationProcessingState state)
        {
            TierMigrationWorkQueueData data = new TierMigrationWorkQueueData();
            data.State = state;

            using(IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueEntityBroker broker = context.GetBroker<IWorkQueueEntityBroker>();
                WorkQueueUpdateColumns parms = new WorkQueueUpdateColumns();
                parms.Data = XmlUtils.SerializeAsXmlDoc(data);
                if (!broker.Update(item.Key, parms))
                    throw new ApplicationException("Unable to update work queue state");
                else
                    context.Commit();
            }
        }

        /// <summary>
        /// Migrates the study to new tier
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="newFilesystem"></param>
        private void DoMigrateStudy(StudyStorageLocation storage, ServerFilesystemInfo newFilesystem)
        {
            Platform.CheckForNullReference(storage, "storage");
            Platform.CheckForNullReference(newFilesystem, "newFilesystem");

            TierMigrationStatistics stat = new TierMigrationStatistics();
            stat.StudyInstanceUid = storage.StudyInstanceUid;
            stat.ProcessSpeed.Start();
    	    StudyXml studyXml = storage.LoadStudyXml();
            stat.StudySize = (ulong) studyXml.GetStudySize(); 

            Platform.Log(LogLevel.Info, "About to migrate study {0} from {1} to {2}", 
                    storage.StudyInstanceUid, storage.FilesystemTierEnum, newFilesystem.Filesystem.Description);
			
            string newPath = Path.Combine(newFilesystem.Filesystem.FilesystemPath, storage.PartitionFolder);
    	    DateTime startTime = Platform.Time;
            DateTime lastLog = DateTime.Now;
    	    int fileCounter = 0;
    	    ulong bytesCopied = 0;
    	    long instanceCountInXml = studyXml.NumberOfStudyRelatedInstances;
            
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

                CopyDirectoryCommand copyDirCommand = new CopyDirectoryCommand(origFolder, newPath, 
                    delegate (string path)
                        {
                            // Update the progress. This is useful if the migration takes long time to complete.

                            FileInfo file = new FileInfo(path);
                            bytesCopied += (ulong)file.Length;
                            if (file.Extension != null && file.Extension.Equals(".dcm", StringComparison.InvariantCultureIgnoreCase))
                            {
                                fileCounter++;
                                TimeSpan elapsed = DateTime.Now - lastLog;
                                TimeSpan totalElapsed = Platform.Time - startTime;
                                double speedInMBPerSecond = 0;
                                if (totalElapsed.TotalSeconds > 0)
                                {
                                    speedInMBPerSecond = (bytesCopied / 1024f / 1024f) / totalElapsed.TotalSeconds;
                                }

                                if (elapsed > TimeSpan.FromSeconds(WorkQueueSettings.Instance.TierMigrationProgressUpdateInSeconds))
                                {
                                    StringBuilder stats = new StringBuilder();
                                    if (instanceCountInXml!=0)
                                    {
                                        float pct = (float)fileCounter/instanceCountInXml;
                                        stats.AppendFormat("{0} files moved [{1:0.0}MB] since {2} ({3:0}% completed). Speed={4:0.00}MB/s", 
                                                    fileCounter, bytesCopied/1024f/1024f, startTime, pct * 100, speedInMBPerSecond);
                                    }
                                    else
                                    {
                                        stats.AppendFormat("{0} files moved [{1:0.0}MB] since {2}. Speed={3:0.00}MB/s", 
                                                    fileCounter, bytesCopied/1024f/1024f, startTime, speedInMBPerSecond);
                                        
                                    }

                                    Platform.Log(LogLevel.Info, "Tier migration for study {0}: {1}", storage.StudyInstanceUid, stats.ToString());
                                    try
                                    {
                                        using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                                        {
                                            IWorkQueueEntityBroker broker = ctx.GetBroker<IWorkQueueEntityBroker>();
                                            WorkQueueUpdateColumns parameters = new WorkQueueUpdateColumns();
                                            parameters.FailureDescription = stats.ToString();
                                            broker.Update(WorkQueueItem.GetKey(), parameters);
                                            ctx.Commit();
                                        }
                                        
                                    }
                                    catch(Exception)
                                    {
                                        // can't log the progress so far... just ignore it
                                    }
                                    finally
                                    {
                                        lastLog = DateTime.Now;
                                    }
                                }
                            }
                        });
                processor.AddCommand(copyDirCommand);

                DeleteDirectoryCommand delDirCommand = new DeleteDirectoryCommand(origFolder, false);
                delDirCommand.RequiresRollback = false;
                processor.AddCommand(delDirCommand);
                
                TierMigrateDatabaseUpdateCommand updateDBCommand = new TierMigrateDatabaseUpdateCommand(context);
                processor.AddCommand(updateDBCommand);

                Platform.Log(LogLevel.Info, "Start migrating study {0}.. {1} to be moved", storage.StudyInstanceUid, ByteCountFormatter.Format(stat.StudySize));
                if (!processor.Execute())
                {
                    if (processor.FailureException != null)
                        throw processor.FailureException;
                    else
                        throw new ApplicationException(processor.FailureReason);
                }

                stat.DBUpdate = updateDBCommand.Statistics;
                stat.CopyFiles = copyDirCommand.CopySpeed;
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

    	    string originalPath = storage.GetStudyPath();
            if (Directory.Exists(storage.GetStudyPath()))
            {
                Platform.Log(LogLevel.Info, "Original study folder could not be deleted. It must be cleaned up manually: {0}", originalPath);
                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Warning, WorkQueueItem.WorkQueueTypeEnum.ToString(), 1000, GetWorkQueueContextData(WorkQueueItem), TimeSpan.Zero,
                    "Study has been migrated to a new tier. Original study folder must be cleaned up manually: {0}", originalPath);
            }

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
			
			IList<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(WorkQueueItem,
                                                    new WorkQueueTypeEnum[]
			                                           {
			                                               WorkQueueTypeEnum.StudyProcess, 
                                                           WorkQueueTypeEnum.ReconcileStudy, 
                                                           WorkQueueTypeEnum.ProcessDuplicate,
                                                           WorkQueueTypeEnum.ReconcilePostProcess,
                                                           WorkQueueTypeEnum.ReconcileCleanup
                                                        }, null);

            IList<StudyIntegrityQueue> reconcileList = ServerHelper.FindSIQEntries(WorkQueueItem.StudyStorage, null);
            
            if ((relatedItems == null || relatedItems.Count == 0) && (reconcileList == null || reconcileList.Count == 0))
                return true; // nothing related in the work queue and nothing in the reconcile queue
            

            Platform.Log(LogLevel.Info,
						 "Tier Migrate entry for study {0} has conflicting WorkQueue entry, reinserting into FilesystemQueue",
						 StorageLocation.StudyInstanceUid);
		
			ReinsertFilesystemQueue();

			PostProcessing(WorkQueueItem, 
				WorkQueueProcessorStatus.Complete, 
				WorkQueueProcessorDatabaseUpdate.ResetQueueState);
			return false;
		}

        private void ReinsertFilesystemQueue()
        {
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
        }
    }

    
}
