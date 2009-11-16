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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy
{
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.None)]
    public class DeleteStudyItemProcessor : BaseItemProcessor
    {
        #region Private Members
        private IList<IDeleteStudyProcessorExtension> _extensions;
        private IEnumerable<DirectoryInfo> _relatedDirectories;

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds all storage locations used for the study.
        /// </summary>
        protected void FindAllRelatedDirectories()
        {
            // Check the work queue for other entries
            IList<Model.WorkQueue> list;

            // NOTE: a local read context is used for lookup because we want to
            // release the lock on the rows asap.
            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IWorkQueueEntityBroker broker = ctx.GetBroker<IWorkQueueEntityBroker>();
                WorkQueueSelectCriteria criteria = new WorkQueueSelectCriteria();
                criteria.StudyStorageKey.EqualTo(WorkQueueItem.StudyStorageKey);
                list = broker.Find(criteria);
            }


            List<DirectoryInfo> dirs = new List<DirectoryInfo>();
            foreach(Model.WorkQueue item in list)
            {
                string path = GetWorkQueueSecondaryFolder(item);
                if (!string.IsNullOrEmpty(path))    
                    dirs.Add(new DirectoryInfo(path));
            }

            // NOTE: Under normal operation, the SIQ entries should be 
            // empty at this point because the Delete Study button is disabled otherwise.
            // This block of code is still needed just in case this DeleteStudy work queue entry
            // is inserted through different means.
            IList<StudyIntegrityQueue> siqList;
            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IStudyIntegrityQueueEntityBroker broker = ctx.GetBroker<IStudyIntegrityQueueEntityBroker>();
                StudyIntegrityQueueSelectCriteria criteria = new StudyIntegrityQueueSelectCriteria();
                criteria.StudyStorageKey.EqualTo(WorkQueueItem.StudyStorageKey);
                siqList = broker.Find(criteria);
            }

            foreach (StudyIntegrityQueue item in siqList)
            {
                string path = GetSIQItemStorageFolder(item);
                if (!string.IsNullOrEmpty(path))
                    dirs.Add(new DirectoryInfo(path));
            }


            _relatedDirectories = dirs;
        }

        private string GetSIQItemStorageFolder(StudyIntegrityQueue queue)
        {
            if (queue.StudyIntegrityReasonEnum.Equals(StudyIntegrityReasonEnum.InconsistentData))
            {
                ReconcileStudyWorkQueueData data = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(queue.Details);
                DirectoryInfo dir = new DirectoryInfo(data.StoragePath);
                return dir.Parent.FullName;
            }

            if (queue.StudyIntegrityReasonEnum.Equals(StudyIntegrityReasonEnum.Duplicate))
            {
                DuplicateSIQQueueData data = XmlUtils.Deserialize<DuplicateSIQQueueData>(queue.Details);
                DirectoryInfo dir = new DirectoryInfo(data.StoragePath);
                return dir.Parent.FullName;
            }

            return null;
        }

        private string GetWorkQueueSecondaryFolder(Model.WorkQueue item)
        {
            if (WorkQueueHasSecondaryPath(item))
            {
                if (item.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.ProcessDuplicate))
                {
                    ProcessDuplicateQueueEntryQueueData queueData = XmlUtils.Deserialize<ProcessDuplicateQueueEntryQueueData>(item.Data);
                    DirectoryInfo dir = new DirectoryInfo(queueData.DuplicateSopFolder);
                    return dir.Parent.FullName;
                }

                if (item.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.CleanupDuplicate))
                {
                    ProcessDuplicateQueueEntryQueueData queueData = XmlUtils.Deserialize<ProcessDuplicateQueueEntryQueueData>(item.Data);
                    DirectoryInfo dir = new DirectoryInfo(queueData.DuplicateSopFolder);
                    return dir.Parent.FullName;
                }

                if (item.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.ReconcileCleanup))
                {
                    ReconcileStudyWorkQueueData queueData = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(item.Data);
                    DirectoryInfo dir = new DirectoryInfo(queueData.StoragePath);
                    return dir.Parent.FullName;
                }
                if (item.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.ReconcileStudy))
                {
                    ReconcileStudyWorkQueueData queueData = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(item.Data);
                    DirectoryInfo dir = new DirectoryInfo(queueData.StoragePath);
                    return dir.Parent.FullName;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="WorkQueue"/> item
        /// uses a diretory that is not the primary study storage location.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool WorkQueueHasSecondaryPath(Model.WorkQueue item)
        {
            List<WorkQueueTypeEnum> requireCleanupTypes = new List<WorkQueueTypeEnum>(new[] {
                                               WorkQueueTypeEnum.ProcessDuplicate,
                                               WorkQueueTypeEnum.CleanupDuplicate,
                                               WorkQueueTypeEnum.ReconcileCleanup,
                                               WorkQueueTypeEnum.ReconcileStudy
                                           });

            return requireCleanupTypes.Contains(item.WorkQueueTypeEnum);
        }

        protected void RemoveFilesystem()
		{
            using(ServerCommandProcessor processor = new ServerCommandProcessor("Delete Filesystems Processor"))
            {
                processor.AddCommand(new DeleteDirectoryCommand(StorageLocation.GetStudyPath(), true));

                if (_relatedDirectories!=null)
                {
                    foreach (DirectoryInfo dir in _relatedDirectories)
                    {
                        processor.AddCommand(new DeleteDirectoryCommand(dir.FullName, true) { Log = true });
                    }
                }
                
                if (!processor.Execute())
                {
                    throw new ApplicationException(
                        String.Format("Unexpected error when deleting study folders: {0}", processor.FailureReason),
                        processor.FailureException);
                }
            }

		}

        protected void RemoveDatabase(Model.WorkQueue item)
        {
			// NOTE:  This was an IUpdateContext, however, it was modified to be an IReadContext
			// after having problems w/ locks on asystem with a fair amount of load.  The 
			// updates are just automatically committed within the stored procedure when it
			// runs...
            using (IReadContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                // Setup the delete parameters
                DeleteStudyStorageParameters parms = new DeleteStudyStorageParameters();

                parms.ServerPartitionKey = item.ServerPartitionKey;
                parms.StudyStorageKey = item.StudyStorageKey;

                // Get the Insert Instance broker and do the insert
                IDeleteStudyStorage delete = updateContext.GetBroker<IDeleteStudyStorage>();

                if (false == delete.Execute(parms))
                {
                    Platform.Log(LogLevel.Error, "Unexpected error when trying to delete study: {0} on partition {1}",
                                 StorageLocation.StudyInstanceUid, ServerPartition.Description);
                }
            }
        }

        protected IList<IDeleteStudyProcessorExtension> LoadExtensions()
        {
            if (_extensions == null)
            {
                _extensions = CollectionUtils.Cast<IDeleteStudyProcessorExtension>(
                    new DeleteStudyProcessorExtensionPoint().CreateExtensions());

                DeleteStudyContext context = CreatePluginProcessingContext();
                foreach (IDeleteStudyProcessorExtension ext in _extensions)
                {
                    ext.Initialize(context);
                }
            }

            return _extensions;
        }

        protected virtual DeleteStudyContext CreatePluginProcessingContext()
        {
            DeleteStudyContext context = new DeleteStudyContext();
            context.WorkQueueItem = WorkQueueItem;
            context.ServerPartition = ServerPartition;
            context.Study = Study;
            context.StorageLocation = StorageLocation;
            context.Filesystem = FilesystemMonitor.Instance.GetFilesystemInfo(StorageLocation.FilesystemKey);
            return context;
        }

        #endregion

        #region Overridden Protected Method

		protected override void ProcessItem(Model.WorkQueue item)
		{
			if (StorageLocation.IsReconcileRequired)
			{
				// fail immediately
				FailQueueItem(item, "Study needs to be reconciled first");
			}
			else
			{
				LoadExtensions();

			    FindAllRelatedDirectories();

				OnDeletingStudy();

				if (Study == null)
					Platform.Log(LogLevel.Info, "Deleting Study {0} on partition {1}",
								 StorageLocation.StudyInstanceUid, ServerPartition.AeTitle);
				else
					Platform.Log(LogLevel.Info,
					             "Deleting study {0} for Patient {1} (PatientId:{2} A#:{3}) on partition {4}",
					             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
					             Study.AccessionNumber, ServerPartition.Description);

				RemoveFilesystem();

				RemoveDatabase(item);

				OnStudyDeleted();

				if (Study == null)
					Platform.Log(LogLevel.Info, "Completed Deleting Study {0} on partition {1}",
								 StorageLocation.StudyInstanceUid, ServerPartition.AeTitle);
				else
					Platform.Log(LogLevel.Info,
								 "Completed Deleting study {0} for Patient {1} (PatientId:{2} A#:{3}) on partition {4}",
								 Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
								 Study.AccessionNumber, ServerPartition.Description);

			}
		}

    	protected override bool CanStart()
        {
            WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(WorkQueueItem.StudyStorageKey);

            IList<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(WorkQueueItem,
                                                        new [] { WorkQueueTypeEnum.StudyProcess, WorkQueueTypeEnum.ReconcileStudy },
                                                         new [] { WorkQueueStatusEnum.Idle, WorkQueueStatusEnum.Pending, WorkQueueStatusEnum.InProgress });

            if (! (relatedItems == null || relatedItems.Count == 0))
            {
                PostponeItem("Study is being processed or reconciled");
            	return false;
            }
    		return true;
        }

        #endregion

        #region Virtual Protected Method

        protected virtual void OnStudyDeleted()
        {

            // Audit log
            DicomStudyDeletedAuditHelper helper = new DicomStudyDeletedAuditHelper(
                                                ServerPlatform.AuditSource,
                                                EventIdentificationTypeEventOutcomeIndicator.Success);
            helper.AddUserParticipant(new AuditProcessActiveParticipant(ServerPartition.AeTitle));
            helper.AddStudyParticipantObject(new AuditStudyParticipantObject(
                                                    StorageLocation.StudyInstanceUid,
                                                    Study == null ? string.Empty : Study.AccessionNumber));
            ServerPlatform.LogAuditMessage(helper);


            IList<IDeleteStudyProcessorExtension> extensions = LoadExtensions();
            foreach (IDeleteStudyProcessorExtension ext in extensions)
            {
                if (ext.Enabled)
                    ext.OnStudyDeleted();
            }
        }

        protected virtual void OnDeletingStudy()
        {
            foreach (IDeleteStudyProcessorExtension ext in _extensions)
            {
                if (ext.Enabled)
                    ext.OnStudyDeleting();
            }
        }

        #endregion

    }

}
