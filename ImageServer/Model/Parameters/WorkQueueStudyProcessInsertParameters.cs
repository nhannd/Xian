using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model
{
    public class WorkQueueStudyProcessInsertParameters : ProcedureParameters
    {
        public WorkQueueStudyProcessInsertParameters()
            : base("InsertWorkQueueStudyProcess")
        { }

        public ServerEntityKey StudyStorageKey
        {
            set { this.SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public DateTime ExpirationTime
        {
            set { this.SubCriteria["ExpirationTime"] = new ProcedureParameter<DateTime>("ExpirationTime", value); }
        }

        public DateTime ScheduledTime
        {
            set { this.SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }

        public string SeriesInstanceUid
        {
            set { this.SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<string>("SeriesInstanceUid", value); }
        }

        public string SopInstanceUid
        {
            set { this.SubCriteria["SopInstanceUid"] = new ProcedureParameter<string>("SopInstanceUid", value); }
        }
    }
}
