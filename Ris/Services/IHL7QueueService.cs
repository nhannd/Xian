using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.HL7;

namespace ClearCanvas.Ris.Services
{
    public interface IHL7QueueService : IHL7ServiceLayer
    {
        IList<HL7QueueItem> GetNextInboundItemBatch();
        void UpdateItemStatus(HL7QueueItem item, HL7MessageStatusCode status);
        void UpdateItemStatus(HL7QueueItem item, HL7MessageStatusCode status, string statusDescription);
        void EnqueueItem(HL7QueueItem item);
    }
}
