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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Process
{
    public class StudyReprocessor
    {
        /// <summary>
        /// Creates a Study Reprocess entry and locks the study in <see cref="QueueStudyStateEnum.ReprocessScheduled"/> state.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="scheduleTime"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public WorkQueue ReprocessStudy(StudyStorageLocation location, DateTime scheduleTime, WorkQueuePriorityEnum priority)
        {
            Platform.CheckForNullReference(location, "location");

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                Study study = location.LoadStudy(ctx);

                // Unlock first
                ILockStudy lockStudy = ctx.GetBroker<ILockStudy>();
                LockStudyParameters lockParms = new LockStudyParameters();
                lockParms.StudyStorageKey = location.Key;
                lockParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
                if (!lockStudy.Execute(lockParms) || !lockParms.Successful)
                    return null;

                // Now relock
                lockParms.QueueStudyStateEnum = QueueStudyStateEnum.ReprocessScheduled;
                if (!lockStudy.Execute(lockParms) || !lockParms.Successful)
                    return null;

                InsertWorkQueueParameters columns = new InsertWorkQueueParameters();
                columns.ScheduledTime = scheduleTime;
                columns.ServerPartitionKey = location.ServerPartitionKey;
                columns.StudyStorageKey = location.Key;
                columns.WorkQueuePriorityEnum = priority;
                columns.WorkQueueTypeEnum = WorkQueueTypeEnum.ReprocessStudy;
                columns.ExpirationTime = scheduleTime.Add(TimeSpan.FromMinutes(5));

                ReprocessStudyQueueData queueData = new ReprocessStudyQueueData();
                queueData.State = new ReprocessStudyState();
                queueData.State.ExecuteAtLeastOnce = false; 
                queueData.ChangeLog = new ReprocessStudyChangeLog();
                queueData.ChangeLog.Reason = "N/A";
                queueData.ChangeLog.TimeStamp = Platform.Time;
                queueData.ChangeLog.User = (Thread.CurrentPrincipal is CustomPrincipal)
                                         ? (Thread.CurrentPrincipal as CustomPrincipal).Identity.Name
                                         : "N/A";
                columns.WorkQueueData = XmlUtils.SerializeAsXmlDoc(queueData);
                IInsertWorkQueue insertBroker = ctx.GetBroker<IInsertWorkQueue>();
                WorkQueue reprocessEntry = insertBroker.FindOne(columns);
                if (reprocessEntry != null)
                {
                    ctx.Commit();

                    if (study != null)
                    {
                        Platform.Log(LogLevel.Info,
                                     "Study Reprocess Scheduled for Study {0}, A#: {1}, Patient: {2}, ID={3}",
                                     study.StudyInstanceUid, study.AccessionNumber, study.PatientsName, study.PatientId);
                    }
                    else
                    {
                        Platform.Log(LogLevel.Info, "Study Reprocess Scheduled for Study {1}.", location.StudyInstanceUid);

                    }
                }

                return reprocessEntry;
            }
        }

    }
}
