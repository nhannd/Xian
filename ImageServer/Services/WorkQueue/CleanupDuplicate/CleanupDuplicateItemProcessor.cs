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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CleanupDuplicate
{
    /// <summary>
    /// For processing 'CleanupDuplicate' WorkQueue items.
    /// </summary>
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default)]
    class CleanupDuplicateItemProcessor : BaseItemProcessor
    {
        #region Private Members
        
        private ProcessDuplicateQueueEntryQueueData _reconcileQueueData;

        #endregion

        #region Overridden Protected Methods

        protected override bool CanStart()
        {
            return true;
        }

        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(item.Data, "Data column cannot be null");
            Platform.CheckForEmptyString(item.GroupID, "GroupID column cannot be null");

            _reconcileQueueData = XmlUtils.Deserialize<ProcessDuplicateQueueEntryQueueData>(WorkQueueItem.Data);

            LoadUids(item);


            if (WorkQueueUidList.Count == 0)
            {
                // cleanup
                DirectoryInfo dir = new DirectoryInfo(_reconcileQueueData.DuplicateSopFolder);
                DirectoryUtility.DeleteIfEmpty(dir.FullName);
                if (dir.Parent != null) 
                    DirectoryUtility.DeleteIfEmpty(dir.Parent.FullName);


                Platform.Log(LogLevel.Info, "Reconcile Cleanup is completed. GUID={0}.", WorkQueueItem.GetKey());
                PostProcessing(WorkQueueItem,
                               WorkQueueProcessorStatus.Complete,
                               WorkQueueProcessorDatabaseUpdate.ResetQueueState);
            }
            else
            {
                Platform.Log(LogLevel.Info,
                             "Starting Cleanup of Duplicate item for study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}, {5} objects",
                             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
                             Study.AccessionNumber, ServerPartition.Description,
                             WorkQueueUidList.Count);

                bool allFailed = CollectionUtils.SelectFirst(WorkQueueUidList, uid => !uid.Failed) == null;

                if (allFailed)
                {
                    FailQueueItem(item, item.FailureDescription ?? "All work queue Uid entries have failed.");
                }
                else
                {

                    Platform.Log(LogLevel.Info, "Duplicates to be removed are located in {0}", _reconcileQueueData.DuplicateSopFolder);

                    int successCount = ProcessUidList();

                    Platform.Log(LogLevel.Info, "Complete Duplicate Cleanup. GUID={0}. {1} uids deleted.", WorkQueueItem.GetKey(), successCount);
                    PostProcessing(WorkQueueItem, WorkQueueProcessorStatus.Pending, WorkQueueProcessorDatabaseUpdate.None);
                }

            }
        }

        #endregion

        #region Private Methods

        private int ProcessUidList()
        {
            Platform.CheckForNullReference(WorkQueueUidList, "WorkQueueUidList");
            int successCount = 0;

            foreach(WorkQueueUid uid in WorkQueueUidList)
            {
                try
                {
                    DeleteDuplicate(uid);
                    successCount++;
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Unable to delete work queue uid {0}", uid.SopInstanceUid);
                    try
                    {
                        FailWorkQueueUid(uid, true);
                    }
                    catch(Exception ex2)
                    {
                        Platform.Log(LogLevel.Error, ex2, "Unexpected exception when updating work queue uid entry");
                    }
                }
            }

            return successCount;
        }

        private FileInfo GetDuplicateSopFile(WorkQueueUid uid)
        {
            string path = Path.Combine(StorageLocation.FilesystemPath, StorageLocation.PartitionFolder);
			path = Path.Combine(path, ServerPlatform.ReconcileStorageFolder);
            path = Path.Combine(path, WorkQueueItem.GroupID);

			if (string.IsNullOrEmpty(uid.RelativePath))
			{
				path = Path.Combine(path, StorageLocation.StudyInstanceUid);
				path = Path.Combine(path, uid.SopInstanceUid + "." + uid.Extension);
			}
			else 
				path = Path.Combine(path, uid.RelativePath);

			return new FileInfo(path);
        }

        private void DeleteDuplicate(WorkQueueUid uid)
        {
            using (ServerCommandProcessor processor = new ServerCommandProcessor("Delete Received Duplicate"))
            {
                FileInfo duplicateFile = GetDuplicateSopFile(uid);
                
                processor.AddCommand(new DeleteFileCommand(duplicateFile.FullName));
                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));
                if (!processor.Execute())
                {
                    throw new ApplicationException(processor.FailureReason, processor.FailureException);
                }
                Platform.Log(ServerPlatform.InstanceLogLevel, "Removed duplicate SOP {0} in reconcile folder", uid.SopInstanceUid);
            }
        }

        #endregion
    }
}