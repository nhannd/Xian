#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.Web.Enterprise.Authentication;

namespace ClearCanvas.ImageServer.Core.Process
{
    public class StudyReprocessor
    {
        /// <summary>
        /// Creates a Study Reprocess entry and locks the study in <see cref="QueueStudyStateEnum.ReprocessScheduled"/> state.
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="location"></param>
        /// <param name="scheduleTime"></param>
        /// <returns></returns>
        /// <exception cref="InvalidStudyStateOperationException">Study is in a state that reprocessing is not allowed</exception>
        public WorkQueue ReprocessStudy(String reason, StudyStorageLocation location, DateTime scheduleTime)
        {
        	Platform.CheckForNullReference(location, "location");

        	IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

        	using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
        	{
				WorkQueue reprocessEntry = ReprocessStudy(ctx, reason, location, scheduleTime);
        		if (reprocessEntry != null)
        		{
        			ctx.Commit();
        		}

        		return reprocessEntry;
        	}
        }

        /// <summary>
        /// Inserts a <see cref="WorkQueue"/> request to reprocess the study
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="reason"></param>
        /// <param name="location"></param>
        /// <param name="scheduleTime"></param>
        /// <returns></returns>
        /// <exception cref="InvalidStudyStateOperationException">Study is in a state that reprocessing is not allowed</exception>
        /// 
		public WorkQueue ReprocessStudy(IUpdateContext ctx, String reason, StudyStorageLocation location, DateTime scheduleTime)
		{
			Platform.CheckForNullReference(location, "location");

            if (location.StudyStatusEnum.Equals(StudyStatusEnum.OnlineLossy))
            {
                if (location.IsLatestArchiveLossless)
                {
                    string message = String.Format("Study has been archived as lossless and is currently lossy. It must be restored first");
                    throw new InvalidStudyStateOperationException(message);
                }
            }

			Study study = location.LoadStudy(ctx);
            
			// Unlock first. 
			ILockStudy lockStudy = ctx.GetBroker<ILockStudy>();
			LockStudyParameters lockParms = new LockStudyParameters();
			lockParms.StudyStorageKey = location.Key;
			lockParms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
			if (!lockStudy.Execute(lockParms) || !lockParms.Successful)
			{
                // Note: according to the stored proc, setting study state to Idle always succeeds so
                // this will never happen
			    return null;
			}

            // Now relock into ReprocessScheduled state. If another process locks the study before this occurs,
            // 
			lockParms.QueueStudyStateEnum = QueueStudyStateEnum.ReprocessScheduled;
			if (!lockStudy.Execute(lockParms) || !lockParms.Successful)
			{
			    throw new InvalidStudyStateOperationException(lockParms.FailureReason);
			}

			InsertWorkQueueParameters columns = new InsertWorkQueueParameters();
			columns.ScheduledTime = scheduleTime;
			columns.ServerPartitionKey = location.ServerPartitionKey;
			columns.StudyStorageKey = location.Key;
			columns.WorkQueueTypeEnum = WorkQueueTypeEnum.ReprocessStudy;

			ReprocessStudyQueueData queueData = new ReprocessStudyQueueData();
			queueData.State = new ReprocessStudyState();
			queueData.State.ExecuteAtLeastOnce = false;
			queueData.ChangeLog = new ReprocessStudyChangeLog();
			queueData.ChangeLog.Reason = reason;
			queueData.ChangeLog.TimeStamp = Platform.Time;
			queueData.ChangeLog.User = (Thread.CurrentPrincipal is CustomPrincipal)
			                           	? (Thread.CurrentPrincipal as CustomPrincipal).Identity.Name
			                           	: String.Empty;
			columns.WorkQueueData = XmlUtils.SerializeAsXmlDoc(queueData);
			IInsertWorkQueue insertBroker = ctx.GetBroker<IInsertWorkQueue>();
			WorkQueue reprocessEntry = insertBroker.FindOne(columns);
			if (reprocessEntry != null)
			{
				if (study != null)
				{
					Platform.Log(LogLevel.Info,
					             "Study Reprocess Scheduled for Study {0}, A#: {1}, Patient: {2}, ID={3}",
					             study.StudyInstanceUid, study.AccessionNumber, study.PatientsName, study.PatientId);
				}
				else
				{
					Platform.Log(LogLevel.Info, "Study Reprocess Scheduled for Study {0}.", location.StudyInstanceUid);
				}
			}

			return reprocessEntry;
		}
    }
}
