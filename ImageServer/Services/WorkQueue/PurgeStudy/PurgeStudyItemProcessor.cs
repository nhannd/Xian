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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.PurgeStudy
{

    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.None)]
    public class PurgeStudyItemProcessor : BaseItemProcessor
	{
		#region Private Methods
		private void RemoveFilesystem()
		{
			string path = StorageLocation.GetStudyPath();
			DirectoryUtility.DeleteIfExists(path, true);
		}

		private void RemoveDatabase(Model.WorkQueue item)
		{
			using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				// Setup the delete parameters
				DeleteFilesystemStudyStorageParameters parms = new DeleteFilesystemStudyStorageParameters();

				parms.ServerPartitionKey = item.ServerPartitionKey;
				parms.StudyStorageKey = item.StudyStorageKey;
				parms.StudyStatusEnum = StudyStatusEnum.Nearline; // TODO: Don't we set Nearline only if all the storage location are purged?

				// Get the Insert Instance broker and do the insert
				IDeleteFilesystemStudyStorage delete = updateContext.GetBroker<IDeleteFilesystemStudyStorage>();

				if (false == delete.Execute(parms))
				{
					Platform.Log(LogLevel.Error, "Unexpected error when trying to delete study: {0} on partition {1}",
								 StorageLocation.StudyInstanceUid, ServerPartition.Description);
				}
				else
				{
					// Unlock the study, too
					ILockStudy studyLock = updateContext.GetBroker<ILockStudy>();
					LockStudyParameters lockParms = new LockStudyParameters();
					lockParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
					lockParms.StudyStorageKey = item.StudyStorageKey;
					studyLock.Execute(lockParms);

					updateContext.Commit();
				}
			}
		}
		#endregion

		#region Overridden Protected Method

		protected override void ProcessItem(Model.WorkQueue item)
		{
			Platform.Log(LogLevel.Info,
			             "Purging study {0} for Patient {1} (PatientId:{2} A#:{3}) on partition {4}",
			             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
			             Study.AccessionNumber, ServerPartition.Description);

			RemoveFilesystem();


			RemoveDatabase(item);

			// No need to remove / update the Queue entry, it was deleted as part of the delete process.
		}

		#endregion

        protected override bool CanStart()
        {
            WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(WorkQueueItem.StudyStorageKey);
            workQueueCriteria.WorkQueueTypeEnum.In(new WorkQueueTypeEnum[] { WorkQueueTypeEnum.StudyProcess, WorkQueueTypeEnum.ReconcileStudy });
            workQueueCriteria.WorkQueueStatusEnum.In(new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Idle, WorkQueueStatusEnum.InProgress, WorkQueueStatusEnum.Pending });

            List<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(WorkQueueItem, workQueueCriteria);
            if (! (relatedItems == null || relatedItems.Count == 0))
            {
				PostponeItem(WorkQueueItem);
            	return false;
            }
        	return true;
        }
    }
}
