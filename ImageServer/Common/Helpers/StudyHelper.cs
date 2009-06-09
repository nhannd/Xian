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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Common.Helpers
{

    /// <summary>
    /// Helper class for handling studies
    /// </summary>
    public static class StudyHelper
    {
        /// <summary>
        /// Retrieves the <see cref="StudyStorage"/> record for a specified study.
        /// </summary>
        /// <param name="context">The persistence context used for database I/O</param>
        /// <param name="studyInstanceUid">The study instance uid of the study</param>
        /// <param name="partition">The <see cref="ServerPartition"/> where the study is located</param>
        /// <returns></returns>
        public static StudyStorage FindStorage(IPersistenceContext context, string studyInstanceUid, ServerPartition partition)
        {
            Platform.CheckForNullReference(context, "context");

            IStudyStorageEntityBroker broker = context.GetBroker<IStudyStorageEntityBroker>();
            StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
            criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
            criteria.ServerPartitionKey.EqualTo(partition.GetKey());

            return broker.FindOne(criteria);
            
        }

        /// <summary>
        /// Verifies the contents of a <see cref="DicomMessageBase"/> against a given <see cref="StudyStorageLocation"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="studyStorage"></param>
        /// <returns></returns>
        static public DifferenceCollection Compare(DicomMessageBase message, Study study, ServerPartition partition)
        {
            StudyComparer comparer = new StudyComparer();

            return comparer.Compare(message, study, partition.GetComparisonOptions());
        }

        /// <summary>
        /// Create a Study Reprocess entry.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        static public bool ReprocessStudy(StudyStorageLocation location)
        {
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                // Unlock first
                ILockStudy lockStudy = ctx.GetBroker<ILockStudy>();
                LockStudyParameters lockParms = new LockStudyParameters();
                lockParms.StudyStorageKey = location.Key;
                lockParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
                if (!lockStudy.Execute(lockParms) || !lockParms.Successful)
                    return false;

                // Now relock
                lockParms.QueueStudyStateEnum = QueueStudyStateEnum.ReprocessScheduled;
                if (!lockStudy.Execute(lockParms) || !lockParms.Successful)
                    return false;

                InsertWorkQueueParameters columns = new InsertWorkQueueParameters();
                columns.ScheduledTime = Platform.Time;
                columns.ServerPartitionKey = location.ServerPartitionKey;
                columns.StudyStorageKey = location.Key;
                columns.WorkQueuePriorityEnum = WorkQueuePriorityEnum.Low;
                columns.WorkQueueTypeEnum = WorkQueueTypeEnum.ReprocessStudy;
                columns.ExpirationTime = Platform.Time.Add(TimeSpan.FromMinutes(5));
                IInsertWorkQueue insertBroker = ctx.GetBroker<IInsertWorkQueue>();
                if (insertBroker.FindOne(columns) != null)
                {
                    ctx.Commit();
                    return true;
                }

                return false;
            }
        }
    }
}
