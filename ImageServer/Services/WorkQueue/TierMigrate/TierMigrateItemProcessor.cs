using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    
    class TierMigrateItemProcessor : BaseItemProcessor
    {
        static FilesystemMonitor _monitor = new FilesystemMonitor();

        
        static TierMigrateItemProcessor()
        {
            _monitor.Load();

        }

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
                        WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();
                        parms.ProcessorID = ServiceTools.ProcessorId;

                        parms.WorkQueueKey = item.GetKey();
                        parms.StudyStorageKey = item.StudyStorageKey;
                        parms.FailureCount = item.FailureCount + 1;
                        parms.FailureDescription = failureDescription;

                        WorkQueueSettings settings = WorkQueueSettings.Default;
                        Platform.Log(LogLevel.Error,
                                     "Failing {0} WorkQueue entry ({1})", item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
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
                _storageLocationList = LoadStorageLocation(item);
                DoMigrateStudies(_storageLocationList);
                PostProcessing(item, false, true);
            }
            catch(Exception e)
            {
                FailQueueItem(item, e.Message);
            }

            
        }

        private void DoMigrateStudies(IList<StudyStorageLocation> storages)
        {
            foreach(StudyStorageLocation storage in storages)
            {
                DoMigrateStudy(storage);
            }
        }


        private void DoMigrateStudy(StudyStorageLocation storage)
        {
            Platform.Log(LogLevel.Info, "Migrating study {0} from {1}", storage.StudyInstanceUid, storage.FilesystemTierEnum);
            ServerFilesystemInfo currFilesystem = _monitor.GetFilesystemInfo(storage.FilesystemKey);
            List<ServerFilesystemInfo> nextTierFilesystems = _monitor.FindNextTierFilesystems(currFilesystem);

            if (nextTierFilesystems == null || nextTierFilesystems.Count==0)
            {
            	// this entry shouldn't have been scheduled in the first place.
                throw new ApplicationException("No filesystem found in next tier.");
            }
                
            ServerFilesystemInfo newFilesystem = nextTierFilesystems[0];
            ServerCommandProcessor _processor; 

            using (_processor = new ServerCommandProcessor("Migrate Study"))
            {
                TierMigrationContext context = new TierMigrationContext();
                context.OriginalStudyLocation = storage;
                context.Destination = newFilesystem;

                TierMigrateMoveStudyFolderCommand moveStudyFolderCommand = new TierMigrateMoveStudyFolderCommand(context);
                TierMigrateDatabaseUpdateCommand updateDBCommand = new TierMigrateDatabaseUpdateCommand(context);

                _processor.AddCommand(moveStudyFolderCommand);
                _processor.AddCommand(updateDBCommand);

                if (!_processor.Execute())
                {
                    throw new ApplicationException(_processor.FailureReason);
                }
            }

            Platform.Log(LogLevel.Info, "Successfully migrated study {0} from {1} to {2}",
                            storage.StudyInstanceUid, storage.FilesystemTierEnum, newFilesystem.Filesystem.FilesystemTierEnum);
            

        }

        
    }
}
