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
using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Data;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CleanupReconcile
{
    /// <summary>
    /// For processing 'CleanupReconcile' WorkQueue items.
    /// </summary>
    class CleanupReconcileItemProcessor : BaseItemProcessor
    {
        private ReconcileStudyWorkQueueData _reconcileQueueData;

        protected override bool CanStart()
        {
            return true;
        }

		protected override void ProcessItem(Model.WorkQueue item)
		{
			Platform.CheckForNullReference(item, "item");
			Platform.CheckForNullReference(item.Data, "item.Data");

			_reconcileQueueData = XmlUtils.Deserialize<ReconcileStudyWorkQueueData>(WorkQueueItem.Data);

			LoadUids(item);


			if (WorkQueueUidList.Count == 0)
			{
				DirectoryUtility.DeleteIfEmpty(_reconcileQueueData.StoragePath);

				Platform.Log(LogLevel.Info, "Reconcile Cleanup is completed. GUID={0}.", WorkQueueItem.GetKey());
				PostProcessing(WorkQueueItem,
					WorkQueueProcessorStatus.Complete,
					WorkQueueProcessorDatabaseUpdate.ResetQueueState);
			}
			else
			{
				Platform.Log(LogLevel.Info,
				             "Starting Cleanup of Reconcile Queue item for study {0} for Patient {1} (PatientId:{2} A#:{3}) on Partition {4}, {5} objects",
				             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
				             Study.AccessionNumber, ServerPartition.Description,
				             WorkQueueUidList.Count);

				ProcessUidList();

				Platform.Log(LogLevel.Info, "Successfully complete Reconcile Cleanup. GUID={0}. {0} uids processed.", WorkQueueItem.GetKey(), WorkQueueUidList.Count);
				PostProcessing(WorkQueueItem,
					WorkQueueProcessorStatus.Pending,
					WorkQueueProcessorDatabaseUpdate.None);
			}
		}

        private void ProcessUidList()
        {
            Platform.CheckForNullReference(WorkQueueUidList, "WorkQueueUidList");

            foreach(WorkQueueUid uid in WorkQueueUidList)
            {
                ProcessUid(uid);
            }
        }

        private void ProcessUid(WorkQueueUid uid)
        {
            Platform.CheckForNullReference(uid, "uid");

            string imagePath = GetUidPath(uid);
            
            using (ServerCommandProcessor processor = new ServerCommandProcessor(String.Format("Deleting {0}", uid.SopInstanceUid)))
            {
                
                // If the file for some reason doesn't exist, we just ignore it
                if (File.Exists(imagePath))
                {
                    Platform.Log(LogLevel.Info, "Deleting {0}", imagePath);
                    FileDeleteCommand deleteFile = new FileDeleteCommand(imagePath, true);
                    processor.AddCommand(deleteFile);
                }
                else
                {
                    Platform.Log(LogLevel.Info, "WARNING {0} is missing.", imagePath);
                }

                DeleteWorkQueueUidCommand deleteUid = new DeleteWorkQueueUidCommand(uid);
                processor.AddCommand(deleteUid);
                if (!processor.Execute())
                {
                    throw new Exception(String.Format("Unable to delete image {0}", uid.SopInstanceUid));
                }
            }

        }

        private string GetUidPath(WorkQueueUid sop)
        {
            string imagePath = Path.Combine(_reconcileQueueData.StoragePath, sop.SopInstanceUid + ".dcm");
            Debug.Assert(String.IsNullOrEmpty(imagePath)==false);

            return imagePath;
        }
    }
}
