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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    public class ReconcileStudyWorkQueue : WorkQueue
    {
        private StudyStorageLocation _location;

        public ReconcileStudyWorkQueue(Model.WorkQueue workqueue)
        {
            Platform.CheckTrue(workqueue.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.ReconcileStudy),
                               String.Format("Cannot copy data from Work Queue record of type {0}",
                                             workqueue.WorkQueueTypeEnum));

            this.SetKey(workqueue.GetKey());
            this.Data= workqueue.Data;
            this.InsertTime = workqueue.InsertTime;
            this.DeviceKey = workqueue.DeviceKey;
            this.ExpirationTime = workqueue.ExpirationTime;
            this.FailureCount = workqueue.FailureCount;
            this.FailureDescription = workqueue.FailureDescription;
            this.GroupID = workqueue.GroupID;
            this.InsertTime = workqueue.InsertTime;
            this.ProcessorID = workqueue.ProcessorID;
            this.ScheduledTime = workqueue.ScheduledTime;
            this.ServerPartitionKey = workqueue.ServerPartitionKey;
            this.StudyHistoryKey = workqueue.StudyHistoryKey;
            this.StudyStorageKey = workqueue.StudyStorageKey;
            this.WorkQueuePriorityEnum = workqueue.WorkQueuePriorityEnum;
            this.WorkQueueStatusEnum = workqueue.WorkQueueStatusEnum;
            this.WorkQueueTypeEnum = this.WorkQueueTypeEnum;
        }

        public string GetFolderPath()
        {
            if (_location == null)
            {
                if (_studyStorage == null)
                {
                    using (IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                    {
                        _studyStorage = StudyStorage.Load(context, this.StudyStorageKey);
                    }
                }

                _location = StudyStorageLocation.FindStorageLocations(_studyStorage)[0];

            }
            
            XmlNode nodeStoragePath = Data.SelectSingleNode("//StoragePath");
            String path = Path.Combine(_location.FilesystemPath, _location.PartitionFolder);
            path = Path.Combine(path, "Reconcile");
            path = Path.Combine(path, nodeStoragePath.InnerText);
            return path;
        }

        public string GetSopPath(string seriesUid, string instanceUid)
        {
            string path = Path.Combine(GetFolderPath(), instanceUid);
            path += "." + "dcm";
            return path;
        }
    }
}
