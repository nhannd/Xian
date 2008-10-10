using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model
{
    public partial class StudyStorage
    {
        public void Archive(IUpdateContext context)
        {
            IInsertArchiveQueue insertArchiveQueueBroker = context.GetBroker<IInsertArchiveQueue>();
            InsertArchiveQueueParameters parms = new InsertArchiveQueueParameters();
            parms.ServerPartitionKey = this.ServerPartitionKey;
            parms.StudyStorageKey = this.GetKey();
            if (!insertArchiveQueueBroker.Execute(parms))
            {
                throw new ApplicationException("Unable to schedule study archive");
            }
        }
    }
}
