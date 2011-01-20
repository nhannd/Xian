#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
    /// <summary>
    /// Represents a specialized type of <see cref="StudyIntegrityQueue"/>
    /// </summary>
    public class DuplicateSopReceivedQueue : StudyIntegrityQueue
    {
        private StudyStorageLocation _location;

        public DuplicateSopReceivedQueue()
        {
            
        }

        public DuplicateSopReceivedQueue(StudyIntegrityQueue studyIntegrityQueueEntry)
        {
            Platform.CheckTrue(studyIntegrityQueueEntry.StudyIntegrityReasonEnum == StudyIntegrityReasonEnum.Duplicate,
                               String.Format("Cannot copy data from StudyIntegrityQueue record of type {0}",
                                             studyIntegrityQueueEntry.StudyIntegrityReasonEnum));

            this.SetKey(studyIntegrityQueueEntry.GetKey());
            this.Description = studyIntegrityQueueEntry.Description;
            this.InsertTime = studyIntegrityQueueEntry.InsertTime;
            this.Details = studyIntegrityQueueEntry.Details;
            this.ServerPartitionKey = studyIntegrityQueueEntry.ServerPartitionKey;
            this.StudyData = studyIntegrityQueueEntry.StudyData;
            this.StudyIntegrityReasonEnum = studyIntegrityQueueEntry.StudyIntegrityReasonEnum;
            this.StudyStorageKey = studyIntegrityQueueEntry.StudyStorageKey;
            this.GroupID = studyIntegrityQueueEntry.GroupID;
            
        }


        public new static DuplicateSopReceivedQueue Load(IPersistenceContext context, ServerEntityKey key)
        {
            return new DuplicateSopReceivedQueue(StudyIntegrityQueue.Load(context, key));
        }

        public string GetFolderPath(IPersistenceContext context)
        {
            if (_location ==null)
            {
                if (_studyStorage==null)
                {
                    _studyStorage = StudyStorage.Load(context, StudyStorageKey);                    
                }

                _location = StudyStorageLocation.FindStorageLocations(_studyStorage)[0];
                
            }

            String path = Path.Combine(_location.FilesystemPath, _location.PartitionFolder);
            path = Path.Combine(path, "Reconcile");
            path = Path.Combine(path, GroupID);
            path = Path.Combine(path, _location.StudyInstanceUid);

            return path;
        }
    }
}
