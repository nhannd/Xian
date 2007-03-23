using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    [DataContract]
    public class GetReferencedPatientRequest : DataContractBase
    {
        public GetReferencedPatientRequest(EntityRef queueItemRef)
        {
            this.QueueItemRef = queueItemRef;
        }

        [DataMember]
        public EntityRef QueueItemRef;
    }
}
