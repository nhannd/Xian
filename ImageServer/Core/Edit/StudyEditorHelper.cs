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
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core.Edit
{
    /// <summary>
    /// Helper class to perform update on a study
    /// </summary>
    public static class StudyEditorHelper
    {
        /// <summary>
        /// Inserts delete request(s) to delete a series in a study.
        /// </summary>
        /// <param name="context">The persistence context used for database connection.</param>
        /// <param name="partition">The <see cref="ServerPartition"/> where the study resides</param>
        /// <param name="studyInstanceUid">The Study Instance Uid of the study</param>
        /// <param name="seriesInstanceUids">The Series Instance Uid of the series to be deleted.</param>
        /// <param name="reason">The reason for deleting the series.</param>
        /// <returns>A list of DeleteSeries <see cref="WorkQueue"/> entries inserted into the system.</returns>
        /// <exception cref="InvalidStudyStateOperationException"></exception>
        public static IList<WorkQueue> DeleteSeries(IUpdateContext context, ServerPartition partition, string studyInstanceUid, List<string> seriesInstanceUids, string reason)
        {
            // Find all location of the study in the system and insert series delete request
            IList<StudyStorageLocation> storageLocations = ServerHelper.FindStudyStorages(partition, studyInstanceUid);
            IList<WorkQueue> entries = new List<WorkQueue>();

            foreach (StudyStorageLocation location in storageLocations)
            {
                if (location.IsNearline)
                {
                    throw new InvalidStudyStateOperationException("Study Is Nearline. It must be restored first.");
                }

                try
                {
                    string failureReason;
                    if (ServerHelper.LockStudy(location.Key, QueueStudyStateEnum.WebDeleteScheduled, out failureReason))
                    {
                        // insert a delete series request
                        WorkQueue request = InsertDeleteSeriesRequest(context, location, seriesInstanceUids, reason);
                        Debug.Assert(request.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.WebDeleteStudy));
                        entries.Add(request);
                    }
                    else
                    {
                        throw new ApplicationException(String.Format("Unable to lock storage location {0} for deletion : {1}", location.Key, failureReason));
                    }
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex, "Errors occurred when trying to insert delete request");
                    if (!ServerHelper.UnlockStudy(location.Key))
                        throw new ApplicationException("Unable to unlock the study");
                }
            }

            return entries;
        }

        /// <summary>
        /// Inserts a DeleteSeries work queue entry
        /// </summary>
        /// <param name="context"></param>
        /// <param name="location"></param>
        /// <param name="seriesInstanceUids"></param>
        /// <param name="reason"></param>
        /// <exception cref="ApplicationException">If the "DeleteSeries" Work Queue entry cannot be inserted.</exception>
        private static WorkQueue InsertDeleteSeriesRequest(IUpdateContext context, StudyStorageLocation location, List<string> seriesInstanceUids, string reason)
        {
            // Create a work queue entry and append the series instance uid into the WorkQueueUid table

            WorkQueue deleteSeriesEntry = null;
            foreach(string uid in seriesInstanceUids)
            {
                IInsertWorkQueue broker = context.GetBroker<IInsertWorkQueue>();
                InsertWorkQueueParameters criteria = new DeleteSeriesWorkQueueParameters(location, uid, reason);
                deleteSeriesEntry = broker.FindOne(criteria);
                if (deleteSeriesEntry == null)
                {
                    throw new ApplicationException(
                        String.Format("Unable to insert a Delete Series request for series {0} in study {1}",
                                      uid, location.StudyInstanceUid));
                }
            }

            return deleteSeriesEntry;
        }
    }

    class DeleteSeriesWorkQueueParameters : InsertWorkQueueParameters
    {
        public DeleteSeriesWorkQueueParameters(StudyStorageLocation studyStorageLocation, string seriesInstanceUid, string reason)
        {
            DateTime now = Platform.Time;
            WebDeleteSeriesLevelQueueData data = new WebDeleteSeriesLevelQueueData();
            data.Reason = reason;
            data.Timestamp = now;
            data.UserId = ServerHelper.CurrentUserName;
            
            WorkQueueTypeEnum = WorkQueueTypeEnum.WebDeleteStudy;
            WorkQueuePriorityEnum = WorkQueuePriorityEnum.High;
            StudyStorageKey = studyStorageLocation.Key;
            ServerPartitionKey = studyStorageLocation.ServerPartitionKey;
            ScheduledTime = now;
            ExpirationTime = now.AddMinutes(15);
            SeriesInstanceUid = seriesInstanceUid;
            WorkQueueData = XmlUtils.SerializeAsXmlDoc(data);
        }
    }
}
