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

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// Assembles an instance of  <see cref="WorkQueueDetails"/> based on a <see cref="WorkQueue"/> object.
    /// </summary>
    public static class WorkQueueDetailsAssembler
    {
        /// <summary>
        /// Creates an instance of <see cref="WorkQueueDetails"/> base on a <see cref="WorkQueue"/> object.
        /// </summary>
        /// <param name="workqueue"></param>
        /// <returns></returns>
        public static WorkQueueDetails CreateWorkQueueDetail(Model.WorkQueue workqueue)
        {
            if (workqueue.WorkQueueTypeEnum == WorkQueueTypeEnum.AutoRoute)
            {
                return CreateAutoRouteWorkQueueItemDetails(workqueue);
            }
            
            if (workqueue.WorkQueueTypeEnum == WorkQueueTypeEnum.WebMoveStudy)
            {
                return CreateWebMoveStudyWorkQueueItemDetails(workqueue);
            }
            
            return CreateGeneralWorkQueueItemDetails(workqueue);
            
        }

        #region Private Static Methods

        private static WorkQueueDetails CreateGeneralWorkQueueItemDetails(Model.WorkQueue item)
        {
            var detail = new WorkQueueDetails();

            detail.Key = item.Key;
            detail.ScheduledDateTime = item.ScheduledTime;
            detail.ExpirationTime = item.ExpirationTime;
            detail.InsertTime = item.InsertTime;
            detail.FailureCount = item.FailureCount;
            detail.Type = item.WorkQueueTypeEnum;
            detail.Status = item.WorkQueueStatusEnum;
            detail.Priority = item.WorkQueuePriorityEnum;
            detail.FailureDescription = item.FailureDescription;
            detail.ServerDescription = item.ProcessorID;

            StudyStorageLocation storage = WorkQueueController.GetLoadStorageLocation(item);
            detail.StorageLocationPath = storage.GetStudyPath();

            // Fetch UIDs
            var wqUidsAdaptor = new WorkQueueUidAdaptor();
            var uidCriteria = new WorkQueueUidSelectCriteria();
            uidCriteria.WorkQueueKey.EqualTo(item.GetKey());
            IList<WorkQueueUid> uids = wqUidsAdaptor.Get(uidCriteria);

            var mapSeries = new Hashtable();
            foreach (WorkQueueUid uid in uids)
            {
                if (mapSeries.ContainsKey(uid.SeriesInstanceUid) == false)
                    mapSeries.Add(uid.SeriesInstanceUid, uid.SopInstanceUid);
            }

            detail.NumInstancesPending = uids.Count;
            detail.NumSeriesPending = mapSeries.Count;


            // Fetch the study and patient info
            var ssAdaptor = new StudyStorageAdaptor();
            StudyStorage storages = ssAdaptor.Get(item.StudyStorageKey);

            var studyAdaptor = new StudyAdaptor();
            var studycriteria = new StudySelectCriteria();
            studycriteria.StudyInstanceUid.EqualTo(storages.StudyInstanceUid);
            studycriteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);
            Study study = studyAdaptor.GetFirst(studycriteria);

            // Study may not be available until the images are processed.
            if (study != null)
            {
                var studyAssembler = new StudyDetailsAssembler();
                detail.Study = studyAssembler.CreateStudyDetail(study);
            }
            return detail;
        }

        private static WorkQueueDetails CreateAutoRouteWorkQueueItemDetails(Model.WorkQueue item)
        {
            var studyStorageAdaptor = new StudyStorageAdaptor();
            var deviceAdaptor = new DeviceDataAdapter();
            var wqUidsAdaptor = new WorkQueueUidAdaptor();
            var studyAdaptor = new StudyAdaptor();

            StudyStorage studyStorage = studyStorageAdaptor.Get(item.StudyStorageKey);

            var detail = new AutoRouteWorkQueueDetails();
            detail.Key = item.GetKey();
            detail.StudyInstanceUid = studyStorage == null ? string.Empty : studyStorage.StudyInstanceUid;

            detail.DestinationAE = deviceAdaptor.Get(item.DeviceKey).AeTitle;
            detail.ScheduledDateTime = item.ScheduledTime;
            detail.ExpirationTime = item.ExpirationTime;
            detail.InsertTime = item.InsertTime;
            detail.FailureCount = item.FailureCount;
            detail.Type = item.WorkQueueTypeEnum;
            detail.Status = item.WorkQueueStatusEnum;
            detail.Priority = item.WorkQueuePriorityEnum;
            detail.ServerDescription = item.ProcessorID;
            detail.FailureDescription = item.FailureDescription;

            StudyStorageLocation storage = WorkQueueController.GetLoadStorageLocation(item);
            detail.StorageLocationPath = storage.GetStudyPath();

            // Fetch UIDs
            var uidCriteria = new WorkQueueUidSelectCriteria();
            uidCriteria.WorkQueueKey.EqualTo(item.GetKey());
            IList<WorkQueueUid> uids = wqUidsAdaptor.Get(uidCriteria);

            var mapSeries = new Hashtable();
            foreach (WorkQueueUid uid in uids)
            {
                if (mapSeries.ContainsKey(uid.SeriesInstanceUid) == false)
                    mapSeries.Add(uid.SeriesInstanceUid, uid.SopInstanceUid);
            }

            detail.NumInstancesPending = uids.Count;
            detail.NumSeriesPending = mapSeries.Count;


            // Fetch the study and patient info
            if (studyStorage != null)
            {
                var studycriteria = new StudySelectCriteria();
                studycriteria.StudyInstanceUid.EqualTo(studyStorage.StudyInstanceUid);
                studycriteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);
                Study study = studyAdaptor.GetFirst(studycriteria);

                // Study may not be available until the images are processed.
                if (study != null)
                {
                    var studyAssembler = new StudyDetailsAssembler();
                    detail.Study = studyAssembler.CreateStudyDetail(study);
                }
            }

            return detail;
        }

        private static WorkQueueDetails CreateWebMoveStudyWorkQueueItemDetails(Model.WorkQueue item)
        {
            var deviceAdaptor = new DeviceDataAdapter();
            var studyStorageAdaptor = new StudyStorageAdaptor();
            StudyStorage studyStorage = studyStorageAdaptor.Get(item.StudyStorageKey);
            var wqUidsAdaptor = new WorkQueueUidAdaptor();
            var studyAdaptor = new StudyAdaptor();
            Device dest = deviceAdaptor.Get(item.DeviceKey);

            var detail = new WebMoveStudyWorkQueueDetails();
            detail.Key = item.GetKey();

            detail.DestinationAE = dest == null ? string.Empty : dest.AeTitle;
            detail.StudyInstanceUid = studyStorage == null ? string.Empty : studyStorage.StudyInstanceUid;
            detail.ScheduledDateTime = item.ScheduledTime;
            detail.ExpirationTime = item.ExpirationTime;
            detail.InsertTime = item.InsertTime;
            detail.FailureCount = item.FailureCount;
            detail.Type = item.WorkQueueTypeEnum;
            detail.Status = item.WorkQueueStatusEnum;
            detail.Priority = item.WorkQueuePriorityEnum;
            detail.ServerDescription = item.ProcessorID;
            detail.FailureDescription = item.FailureDescription;

            StudyStorageLocation storage = WorkQueueController.GetLoadStorageLocation(item);
            detail.StorageLocationPath = storage.GetStudyPath();

            // Fetch UIDs
            var uidCriteria = new WorkQueueUidSelectCriteria();
            uidCriteria.WorkQueueKey.EqualTo(item.GetKey());
            IList<WorkQueueUid> uids = wqUidsAdaptor.Get(uidCriteria);

            var mapSeries = new Hashtable();
            foreach (WorkQueueUid uid in uids)
            {
                if (mapSeries.ContainsKey(uid.SeriesInstanceUid) == false)
                    mapSeries.Add(uid.SeriesInstanceUid, uid.SopInstanceUid);
            }

            detail.NumInstancesPending = uids.Count;
            detail.NumSeriesPending = mapSeries.Count;


            // Fetch the study and patient info
            if (studyStorage != null)
            {
                var studycriteria = new StudySelectCriteria();
                studycriteria.StudyInstanceUid.EqualTo(studyStorage.StudyInstanceUid);
                studycriteria.ServerPartitionKey.EqualTo(item.ServerPartitionKey);
                Study study = studyAdaptor.GetFirst(studycriteria);

                // Study may not be available until the images are processed.
                if (study != null)
                {
                    var studyAssembler = new StudyDetailsAssembler();
                    detail.Study = studyAssembler.CreateStudyDetail(study);
                }
            }

            return detail;
        }

        #endregion Private Static Methods
    }
}