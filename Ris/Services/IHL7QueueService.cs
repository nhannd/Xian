using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.HL7;

namespace ClearCanvas.Ris.Services
{
    public interface IHL7QueueService : IHL7ServiceLayer
    {
        IList<HL7QueueItem> GetNextInboundHL7QueueItemBatch();
        IList<HL7QueueItem> GetAllHL7QueueItems();
        void ProcessHL7QueueItem(HL7QueueItem item);
        void EnqueueHL7QueueItem(HL7QueueItem item);
        void SyncExternalQueue();
    }
}
