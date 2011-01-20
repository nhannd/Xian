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
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

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

		public WorkQueue ReprocessStudy(IUpdateContext ctx, String reason, StudyStorageLocation location, DateTime scheduleTime)
		{
			Platform.CheckForNullReference(location, "location");

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
