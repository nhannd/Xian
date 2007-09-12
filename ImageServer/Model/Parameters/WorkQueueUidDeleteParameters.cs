using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class WorkQueueUidDeleteParameters : ProcedureParameters
    {
        public WorkQueueUidDeleteParameters()
            : base("DeleteWorkQueueUid")
        {
        }

        public ServerEntityKey WorkQueueUidKey
        {
            set { this.SubCriteria["WorkQueueUidKey"] = new ProcedureParameter<ServerEntityKey>("WorkQueueUidKey", value); }
        }
    }
}
