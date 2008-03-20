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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class StudyController : BaseController
    {
        #region Private Members

        private readonly StudyAdaptor _adaptor = new StudyAdaptor();
        private readonly SeriesSearchAdaptor _seriesAdaptor = new SeriesSearchAdaptor();

        #endregion

        #region Public Methods

        public IList<Study> GetStudies(StudySelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }

        public IList<Series> GetSeries(Study study)
        {
            SeriesSelectCriteria criteria = new SeriesSelectCriteria();

            criteria.StudyKey.EqualTo(study.GetKey());

            return _seriesAdaptor.Get(criteria);
        }


        public bool DeleteStudy(Study study)
        {
            WorkQueueAdaptor workqueueAdaptor = new WorkQueueAdaptor();
            WorkQueueUpdateColumns columns = new WorkQueueUpdateColumns();
            columns.WorkQueueTypeEnum = WorkQueueTypeEnum.GetEnum("WebDeleteStudy");
            columns.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Pending");
            columns.ServerPartitionKey = study.ServerPartitionKey;

            StudyStorageAdaptor studyStorageAdaptor = new StudyStorageAdaptor();
            StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
            criteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
            criteria.StudyInstanceUid.EqualTo(study.StudyInstanceUid);

            IList<StudyStorage> storages = studyStorageAdaptor.Get(criteria);
            int counter = 0;
            foreach(StudyStorage storage in storages)
            {
                counter++;
                columns.StudyStorageKey = storage.GetKey();
                columns.ScheduledTime = DateTime.Now.AddSeconds(60 + (counter)*15); // spread by 15 seconds
                columns.ExpirationTime = DateTime.Now.AddDays(1);
                columns.FailureCount = 0;

                workqueueAdaptor.Add(columns);
            }

            return true;
        }

        public bool MoveStudy(Study study, Device device)
        {
            WorkQueueAdaptor workqueueAdaptor = new WorkQueueAdaptor();
            WorkQueueUpdateColumns columns = new WorkQueueUpdateColumns();
            columns.WorkQueueTypeEnum = WorkQueueTypeEnum.GetEnum("WebMoveStudy");
            columns.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Pending");
            columns.ServerPartitionKey = study.ServerPartitionKey;

            StudyStorageAdaptor studyStorageAdaptor = new StudyStorageAdaptor();
            StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
            criteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
            criteria.StudyInstanceUid.EqualTo(study.StudyInstanceUid);

            IList<StudyStorage> storages = studyStorageAdaptor.Get(criteria);

            columns.StudyStorageKey = storages[0].GetKey();
            DateTime time = Platform.Time.AddSeconds(60);
            columns.ScheduledTime = time;
            columns.ExpirationTime = time;
            columns.FailureCount = 0;
            columns.DeviceKey = device.GetKey();

            workqueueAdaptor.Add(columns);

            return true;
        }
    

        /// <summary>
        /// Returns a value indicating whether the specified study has been scheduled for delete.
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public bool IsScheduledForDelete(Study study)
        {
            Platform.CheckForNullReference(study, "Study");
            
            StudyStorageAdaptor studyStorageAdaptor = new StudyStorageAdaptor();
            StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
            criteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
            criteria.StudyInstanceUid.EqualTo(study.StudyInstanceUid);

            IList<StudyStorage> storages = studyStorageAdaptor.Get(criteria);
            int counter = 0;
            foreach (StudyStorage storage in storages)
            {
                WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
                WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
                workQueueCriteria.WorkQueueTypeEnum.EqualTo(WorkQueueTypeEnum.GetEnum("WebDeleteStudy"));
                workQueueCriteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
                workQueueCriteria.StudyStorageKey.EqualTo(storage.GetKey());

                workQueueCriteria.WorkQueueStatusEnum.EqualTo(WorkQueueStatusEnum.GetEnum("Pending"));

                IList<WorkQueue> list = adaptor.Get(workQueueCriteria);
                if (list != null && list.Count > 0)
                    return true;
                else
                {
                    workQueueCriteria.WorkQueueStatusEnum.EqualTo(WorkQueueStatusEnum.GetEnum("Idle")); // not likely but who knows
                    list = adaptor.Get(workQueueCriteria);
                    if (list != null && list.Count > 0)
                        return true;
                }

                
            }
            return false;
            
        }
        #endregion
    }
}
