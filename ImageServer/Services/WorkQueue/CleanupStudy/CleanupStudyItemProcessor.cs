#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CleanupStudy
{
    /// <summary>
    /// For processing 'CleanupStudy' WorkQueue items.
    /// </summary>
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.None)]
    public class CleanupStudyItemProcessor : BaseItemProcessor
    {

        private void CheckEmptyStudy(Model.WorkQueue item)
        {
            using (IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IStudyEntityBroker study = context.GetBroker<IStudyEntityBroker>();
                StudySelectCriteria criteria = new StudySelectCriteria();

                criteria.StudyInstanceUid.EqualTo(StorageLocation.StudyInstanceUid);
                criteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);

                int count = study.Count(criteria);
                if (count == 0)
                {
                    IDeleteStudyStorage delete = context.GetBroker<IDeleteStudyStorage>();

                    DeleteStudyStorageParameters parms = new DeleteStudyStorageParameters
                                                         	{
                                                         		ServerPartitionKey = item.ServerPartitionKey,
                                                         		StudyStorageKey = item.StudyStorageKey
                                                         	};

                	delete.Execute(parms);
                }
                context.Commit();
            }
        }

        protected override void ProcessItem(Model.WorkQueue item)
        {
			if (!LoadWritableStorageLocation(item))
			{
				Platform.Log(LogLevel.Warn, "Unable to find readable location when processing CleanupStudy WorkQueue item, rescheduling");
                PostponeItem(item.ScheduledTime.AddMinutes(2), item.ExpirationTime.AddMinutes(2), "Unable to find readable location.");

				return;
			}

            LoadUids(item);
            
            if (WorkQueueUidList.Count == 0)
            {
                // No UIDs associated with the WorkQueue item.  Set the status back to idle
				if (item.ExpirationTime <= Platform.Time)
				{
					Platform.Log(LogLevel.Info, "Applying rules engine to study being cleaned up to ensure disk management is applied.");

					// Run Study / Series Rules Engine.
					StudyRulesEngine engine = new StudyRulesEngine(StorageLocation,ServerPartition);
					engine.Apply(ServerRuleApplyTimeEnum.StudyProcessed);
					StorageLocation.LogFilesystemQueue();

					PostProcessing(item,
								   WorkQueueProcessorStatus.Complete,
								   WorkQueueProcessorDatabaseUpdate.ResetQueueState);
				}
				else
				{
					PostProcessing(item,
					               WorkQueueProcessorStatus.IdleNoDelete,
					               WorkQueueProcessorDatabaseUpdate.ResetQueueState);
				}

				// This will just delete the study, if there's no images that have been sucessfully processed.
            	CheckEmptyStudy(item);
                return;
            }

        	Platform.Log(LogLevel.Info,
        	             "Starting Cleanup of study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
        	             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
        	             Study.AccessionNumber, ServerPartition.Description);

            string basePath = StorageLocation.GetStudyPath();

            using (IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                foreach (WorkQueueUid sop in WorkQueueUidList)
                {
                    string path = Path.Combine(basePath, sop.SeriesInstanceUid);

                    path = Path.Combine(path, sop.SopInstanceUid);

                    if (sop.Extension != null)
                        path += "." + sop.Extension;
                    else
                        path += ServerPlatform.DicomFileExtension;

                    try
                    {
                        if (File.Exists(path))
                        {
							FileUtils.Delete(path);
                        }
                        IWorkQueueUidEntityBroker delete = context.GetBroker<IWorkQueueUidEntityBroker>();

                        delete.Delete(sop.GetKey());
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unexpected exception deleting file: {0}", path);
                    }
                }

                context.Commit();
            }

        	Platform.Log(LogLevel.Info,
        	             "Completed Cleanup of study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}",
        	             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
        	             Study.AccessionNumber, ServerPartition.Description);

			PostProcessing(item, 
				WorkQueueProcessorStatus.Pending, 
				WorkQueueProcessorDatabaseUpdate.ResetQueueState);

        }

        protected override bool CanStart()
        {
            return true; // can start anytime
        }
    }
}
