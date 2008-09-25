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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class StudyIntegrityQueueController
	{
        private readonly StudyIntegrityQueueAdaptor _adaptor = new StudyIntegrityQueueAdaptor();

        public IList<StudyIntegrityQueue> GetStudyIntegrityQueueItems(StudyIntegrityQueueSelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }

        public IList<StudyIntegrityQueue> GetRangeStudyIntegrityQueueItems(StudyIntegrityQueueSelectCriteria criteria, int startIndex, int maxRows)
        {
            return _adaptor.GetRange(criteria, startIndex, maxRows);
        }

        public int GetReconicleQueueItemsCount(StudyIntegrityQueueSelectCriteria criteria)
        {
            return _adaptor.GetCount(criteria);
        }

        public bool DeleteStudyIntegrityQueueItem(StudyIntegrityQueue item)
        {
            return _adaptor.Delete(item.Key);
        }

        private void ReconcileStudy(string command, ServerEntityKey itemKey)
        {
            StudyIntegrityQueueAdaptor queueAdaptor = new StudyIntegrityQueueAdaptor();
            Model.StudyIntegrityQueue item = queueAdaptor.Get(itemKey);

            //Add to Study History
            StudyHistoryeAdaptor historyAdaptor = new StudyHistoryeAdaptor();
            StudyHistoryUpdateColumns parameters = new StudyHistoryUpdateColumns();

            XmlDocument changeDescription = new XmlDocument();
            changeDescription.LoadXml(command);

            parameters.StudyData = item.StudyData;
            parameters.ChangeDescription = changeDescription;
            parameters.StudyStorageKey = item.StudyStorageKey;

            StudyHistory history = historyAdaptor.Add(parameters);

            //Create WorkQueue Entry
            WorkQueueAdaptor workQueueAdaptor = new WorkQueueAdaptor();
            WorkQueueUpdateColumns row = new WorkQueueUpdateColumns();
            row.Data = item.QueueData;
            row.ServerPartitionKey = item.ServerPartitionKey;
            row.StudyStorageKey = item.StudyStorageKey;
            row.StudyHistoryKey = history.GetKey();
            row.WorkQueueTypeEnum = WorkQueueTypeEnum.ReconcileStudy;
            row.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
            row.ScheduledTime = DateTime.Now;
            row.ExpirationTime = DateTime.Now.AddHours(1);
            WorkQueue newWorkQueueItem = workQueueAdaptor.Add(row);

            StudyIntegrityQueueUidAdaptor studyIntegrityQueueUidAdaptor = new StudyIntegrityQueueUidAdaptor();
            StudyIntegrityQueueUidSelectCriteria crit = new StudyIntegrityQueueUidSelectCriteria();
            crit.StudyIntegrityQueueKey.EqualTo(item.GetKey());
            IList<StudyIntegrityQueueUid> uidList = studyIntegrityQueueUidAdaptor.Get(crit);

            WorkQueueUidAdaptor workQueueUidAdaptor = new WorkQueueUidAdaptor();
            WorkQueueUidUpdateColumns update = new WorkQueueUidUpdateColumns();
            foreach (StudyIntegrityQueueUid uid in uidList)
            {
                update.WorkQueueKey = newWorkQueueItem.GetKey();
                update.SeriesInstanceUid = uid.SeriesInstanceUid;
                update.SopInstanceUid = uid.SopInstanceUid;
                workQueueUidAdaptor.Add(update);
            }

            //DeleteStudyIntegrityQueue Item
            StudyIntegrityQueueUidSelectCriteria criteria = new StudyIntegrityQueueUidSelectCriteria();
            criteria.StudyIntegrityQueueKey.EqualTo(item.GetKey());
            studyIntegrityQueueUidAdaptor.Delete(criteria);

            StudyIntegrityQueueAdaptor studyIntegrityQueueAdaptor = new StudyIntegrityQueueAdaptor();
            studyIntegrityQueueAdaptor.Delete(item.GetKey());   
        }
        
        public void CreateNewStudy(ServerEntityKey itemKey)
        {
            ReconcileStudy(String.Format("<ImageCommands><UpdateImages><SetTag TagPath=\"0020000D\" Value=\"{0}\"/></UpdateImages></ImageCommands>", DicomUid.GenerateUid().UID), itemKey);
        }

        public void Discard(ServerEntityKey itemKey)
        {
            ReconcileStudy("<ImageCommands><Discard/></ImageCommands>", itemKey);
        }
	}
}
