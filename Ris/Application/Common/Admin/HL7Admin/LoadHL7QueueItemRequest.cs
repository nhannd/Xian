using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    [DataContract]
    public class LoadHL7QueueItemRequest : DataContractBase
    {
        [DataMember]
        public EntityRef QueueItemRef;
    }
}
