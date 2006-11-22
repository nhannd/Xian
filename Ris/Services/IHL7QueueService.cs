using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.HL7;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Services
{
    public interface IHL7QueueService : IHL7ServiceLayer
    {
        HL7QueueItem LoadHL7QueueItem(EntityRef<HL7QueueItem> queueItemRef);
        IList<HL7QueueItem> GetNextInboundHL7QueueItemBatch();
        IList<HL7QueueItem> GetAllHL7QueueItems();
        IList<HL7QueueItem> GetFilteredHL7QueueItems(HL7QueueItemSearchCriteria criteria);
        IList<Patient> ProcessHL7QueueItem(HL7QueueItem item);
        void EnqueueHL7QueueItem(HL7QueueItem item);
        void SyncExternalQueue();

        PatientProfile GetReferencedPatient(EntityRef<HL7QueueItem> hl7QueueItemRef);
    }
}
