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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.PurgeStudy
{

	public class PurgeStudyItemProcessor : BaseItemProcessor
	{
		#region Private Members

		private ServerPartition _partition;

		#endregion

		#region Private Methods
		private void RemoveFilesystem()
		{
			string path = StorageLocation.GetStudyPath();
			try
			{
				if (Directory.Exists(path))
				{
					Directory.Delete(path, true);

					DirectoryInfo info = Directory.GetParent(path);
					DirectoryInfo[] subdirs = info.GetDirectories();
					if (subdirs.Length == 0)
						Directory.Delete(info.FullName);
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when trying to delete directory: {0}", path);
			}
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
								 StorageLocation.StudyInstanceUid, _partition.Description);
				}
				else
					updateContext.Commit();
			}
		}
		#endregion

		#region Overridden Protected Method

		protected override void ProcessItem(Model.WorkQueue item)
		{
            //Load the storage location.
			if (!LoadStorageLocation(item))
			{
				Platform.Log(LogLevel.Warn, "Unable to find readable location when processing PurgeStudy WorkQueue item, rescheduling");
				PostponeItem(item, item.ScheduledTime.AddMinutes(2), item.ExpirationTime.AddMinutes(2));
				return;
			}

            
            else
            {
                _partition = ServerPartition.Load(ReadContext, item.ServerPartitionKey);

                Platform.Log(LogLevel.Info, "Purging study '{0}' from partition '{1}'", StorageLocation.StudyInstanceUid,
                             _partition.Description);

                RemoveFilesystem();
            }

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
            return (relatedItems == null || relatedItems.Count == 0);
        }
    }
}
