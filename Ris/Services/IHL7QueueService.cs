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
        IList<HL7QueueItem> GetHL7QueueItems(HL7QueueItemSearchCriteria criteria, SearchResultPage page);

        void EnqueueHL7QueueItem(HL7QueueItem item);
        
        IList<Patient> ProcessHL7QueueItem(HL7QueueItem item);
        PatientProfile GetReferencedPatient(EntityRef<HL7QueueItem> hl7QueueItemRef);
        
        void SetHL7QueueItemComplete(HL7QueueItem item);
        void SetHL7QueueItemError(HL7QueueItem item, string errorMessage);

        string GetAccessionNumber();

        Practitioner FindPractitioner(string id);
    }
}
