using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model
{
    public class WorkQueueUidDeleteParameters : ProcedureParameters
    {
        public WorkQueueUidDeleteParameters()
            : base("DeleteWorkQueueUid")
        {
        }

        public ServerEntityKey WorkQueueKey
        {
            set { this.SubCriteria["WorkQueueKey"] = new ProcedureParameter<ServerEntityKey>("WorkQueueKey", value); }
        }
    }
}
