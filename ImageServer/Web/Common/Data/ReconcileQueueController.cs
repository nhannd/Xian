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
	public class ReconcileQueueController
	{
        private readonly ReconcileQueueAdaptor _adaptor = new ReconcileQueueAdaptor();

        public IList<ReconcileQueue> GetReconcileQueueItems(ReconcileQueueSelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }

        public IList<ReconcileQueue> GetRangeReconcileQueueItems(ReconcileQueueSelectCriteria criteria, int startIndex, int maxRows)
        {
            return _adaptor.GetRange(criteria, startIndex, maxRows);
        }

        public int GetReconicleQueueItemsCount(ReconcileQueueSelectCriteria criteria)
        {
            return _adaptor.GetCount(criteria);
        }

        public bool DeleteReconcileQueueItem(ReconcileQueue item)
        {
            return _adaptor.Delete(item.Key);
        }

        private void ReconcileStudy(string command, ServerEntityKey itemKey)
        {
            ReconcileQueueAdaptor queueAdaptor = new ReconcileQueueAdaptor();
            Model.ReconcileQueue item = queueAdaptor.Get(itemKey);

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

            ReconcileQueueUidAdaptor reconcileQueueUidAdaptor = new ReconcileQueueUidAdaptor();
            ReconcileQueueUidSelectCriteria crit = new ReconcileQueueUidSelectCriteria();
            crit.ReconcileQueueKey.EqualTo(item.GetKey());
            IList<ReconcileQueueUid> uidList = reconcileQueueUidAdaptor.Get(crit);

            WorkQueueUidAdaptor workQueueUidAdaptor = new WorkQueueUidAdaptor();
            WorkQueueUidUpdateColumns update = new WorkQueueUidUpdateColumns();
            foreach (ReconcileQueueUid uid in uidList)
            {
                update.WorkQueueKey = newWorkQueueItem.GetKey();
                update.SeriesInstanceUid = uid.SeriesInstanceUid;
                update.SopInstanceUid = uid.SopInstanceUid;
                workQueueUidAdaptor.Add(update);
            }

            //DeleteReconcileQueue Item
            ReconcileQueueUidSelectCriteria criteria = new ReconcileQueueUidSelectCriteria();
            criteria.ReconcileQueueKey.EqualTo(item.GetKey());
            reconcileQueueUidAdaptor.Delete(criteria);

            ReconcileQueueAdaptor reconcileQueueAdaptor = new ReconcileQueueAdaptor();
            reconcileQueueAdaptor.Delete(item.GetKey());   
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
