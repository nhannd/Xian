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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemStudyProcess
{
    /// <summary>
    /// <see cref="ServerDatabaseCommand"/> derived class for deleting <see cref="FilesystemQueue"/> entries for a
    /// specific study from the database.
    /// </summary>
    public class DeleteFilesystemQueueCommand : ServerDatabaseCommand
    {
        private readonly ServerEntityKey _storageLocationKey;

        public DeleteFilesystemQueueCommand(ServerEntityKey storageLocationKey)
            : base("Delete FilesystemQueue", false)
        {
            _storageLocationKey = storageLocationKey;
        }

        protected override void OnExecute(IUpdateContext updateContext)
        {
            IFilesystemQueueEntityBroker filesystemQueueBroker= updateContext.GetBroker<IFilesystemQueueEntityBroker>();
            FilesystemQueueSelectCriteria criteria = new FilesystemQueueSelectCriteria();
            criteria.StudyStorageKey.EqualTo(_storageLocationKey);
            IList<FilesystemQueue> filesystemQueueItems = filesystemQueueBroker.Find(criteria);

            IWorkQueueEntityBroker workQueueBroker = updateContext.GetBroker<IWorkQueueEntityBroker>();
            WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(_storageLocationKey);
            workQueueCriteria.WorkQueueTypeEnum.In(new WorkQueueTypeEnum[] { WorkQueueTypeEnum.PurgeStudy, WorkQueueTypeEnum.DeleteStudy, WorkQueueTypeEnum.CompressStudy, WorkQueueTypeEnum.MigrateStudy });
            IList<WorkQueue> workQueueItems = workQueueBroker.Find(workQueueCriteria);

            foreach (FilesystemQueue queue in filesystemQueueItems)
            {
                if (!filesystemQueueBroker.Delete(queue.GetKey()))
                    throw new ApplicationException("Unable to delete items in the filesystem queue");
            }

            // delete work queue
            foreach (WorkQueue item in workQueueItems)
            {
                if (!item.Delete(updateContext))
                    throw new ApplicationException("Unable to delete items in the work queue");
            }
        }
    }
}
