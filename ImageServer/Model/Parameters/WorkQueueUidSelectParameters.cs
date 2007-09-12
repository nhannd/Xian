using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class WorkQueueUidSelectParameters : ProcedureParameters
    {
        public WorkQueueUidSelectParameters()
            : base("SelectWorkQueueUids")
        { }

        public ServerEntityKey WorkQueueKey
        {
            set { this.SubCriteria["WorkQueueKey"] = new ProcedureParameter<ServerEntityKey>("WorkQueueKey", value); }
        }
    }
}
