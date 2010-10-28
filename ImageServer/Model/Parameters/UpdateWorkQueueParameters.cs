#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class UpdateWorkQueueParameters : ProcedureParameters
    {
        public UpdateWorkQueueParameters()
            : base("UpdateWorkQueue")
        { }

        public ServerEntityKey WorkQueueKey
        {
            set { SubCriteria["WorkQueueKey"] = new ProcedureParameter<ServerEntityKey>("WorkQueueKey", value); }
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public WorkQueueStatusEnum WorkQueueStatusEnum
        {
            set { SubCriteria["WorkQueueStatusEnum"] = new ProcedureParameter<ServerEnum>("WorkQueueStatusEnum", value); }
        }

        public DateTime ExpirationTime 
        {
            set { SubCriteria["ExpirationTime"] = new ProcedureParameter<DateTime>("ExpirationTime", value); }
        }

        public DateTime ScheduledTime
        {
            set { SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }

        public int FailureCount
        {
            set { SubCriteria["FailureCount"] = new ProcedureParameter<int>("FailureCount", value); }
        }

        public string ProcessorID
        {
            set { SubCriteria["ProcessorID"] = new ProcedureParameter<string>("ProcessorID", value); }
        }

        public string FailureDescription
        {
            set { SubCriteria["FailureDescription"] = new ProcedureParameter<string>("FailureDescription", value); }
        }

		public QueueStudyStateEnum QueueStudyStateEnum
		{
			set { SubCriteria["QueueStudyStateEnum"] = new ProcedureParameter<ServerEnum>("QueueStudyStateEnum", value); }
		}
    }
}
