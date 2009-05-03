using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    public class WorkQueueProcessDuplicateSopUpdateColumns : WorkQueueUpdateColumns
    {
        public WorkQueueProcessDuplicateSopUpdateColumns()
        {
            WorkQueueTypeEnum = WorkQueueTypeEnum.ProcessDuplicate;
        }
    }
}
