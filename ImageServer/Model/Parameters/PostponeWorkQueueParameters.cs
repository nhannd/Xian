using System;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class PostponeWorkQueueParameters : ProcedureParameters
    {
        public PostponeWorkQueueParameters()
            : base("PostponeWorkQueue")
        { }

        public ServerEntityKey WorkQueueKey
        {
            set { SubCriteria["WorkQueueKey"] = new ProcedureParameter<ServerEntityKey>("WorkQueueKey", value); }
        }

        public DateTime ScheduledTime
        {
            set { SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }

        public DateTime ExpirationTime 
        {
            set { SubCriteria["ExpirationTime"] = new ProcedureParameter<DateTime>("ExpirationTime", value); }
        }


        public string Reason
        {
            set { SubCriteria["Reason"] = new ProcedureParameter<string>("Reason", value); }
        }
    }
}