using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    [DataContract]
    public class SetHL7QueueItemCompleteRequest : DataContractBase
    {
        public SetHL7QueueItemCompleteRequest(EntityRef queueItemRef)
        {
            this.QueueItemRef = queueItemRef;
        }

        [DataMember]
        public EntityRef QueueItemRef;
    }
}
