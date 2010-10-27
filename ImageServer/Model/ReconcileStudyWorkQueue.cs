#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

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
