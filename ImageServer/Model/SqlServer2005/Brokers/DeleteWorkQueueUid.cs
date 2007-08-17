using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Database.SqlServer2005;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.Brokers
{
    public class DeleteWorkQueueUid : ProcedureUpdateBroker<WorkQueueUidDeleteParameters>
    {
        public DeleteWorkQueueUid()
            : base("DeleteWorkQueueUid")
        {
        }
    }
}
