using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    [DataContract]
    public class SetHL7QueueItemErrorRequest : DataContractBase
    {
        public SetHL7QueueItemErrorRequest(EntityRef queueItemRef, string errorMessage)
        {
            this.QueueItemRef = queueItemRef;
            this.ErrorMessage = errorMessage;
        }

        [DataMember]
        public EntityRef QueueItemRef;

        [DataMember]
        public string ErrorMessage;
    }
}
