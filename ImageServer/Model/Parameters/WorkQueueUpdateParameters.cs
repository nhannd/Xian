using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class WorkQueueUpdateParameters : ProcedureParameters
    {
        public WorkQueueUpdateParameters()
            : base("UpdateWorkQueue")
        { }

        public ServerEntityKey WorkQueueKey
        {
            set { this.SubCriteria["WorkQueueKey"] = new ProcedureParameter<ServerEntityKey>("WorkQueueKey", value); }
        }

        public ServerEntityKey StudyStorageKey
        {
            set { this.SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public StatusEnum StatusEnum
        {
            set { this.SubCriteria["StatusEnum"] = new ProcedureParameter<ServerEnum>("StatusEnum", value); }
        }

        public DateTime ExpirationTime
        {
            set { this.SubCriteria["ExpirationTime"] = new ProcedureParameter<DateTime>("ExpirationTime", value); }
        }

        public DateTime ScheduledTime
        {
            set { this.SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }

        public int FailureCount
        {
            set { this.SubCriteria["FailureCount"] = new ProcedureParameter<int>("FailureCount", value); }
        }
    }
}
