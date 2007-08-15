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

        public ServerEntityKey StudyStorageRef
        {
            set { this.SubCriteria["StudyStorageRef"] = new ProcedureParameter<ServerEntityKey>("StudyStorageRef", value); }
        }
        public DateTime ExpirationTime
        {
            set { this.SubCriteria["ExpirationTime"] = new ProcedureParameter<DateTime>("ExpirationTime", value); }
        }

        public DateTime ScheduledTime
        {
            set { this.SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }
        
    }
}
