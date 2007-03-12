using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    class HL7QueueItemSummary : DataContractBase
    {
        [DataMember]
        public EntityRef QueueItemRef;

        [DataMember]
        public string Direction;

        [DataMember]
        public string StatusCode;

        [DataMember]
        public string StatusDescription;

        [DataMember]
        public DateTime CreationDateTime;

        [DataMember]
        public DateTime? UpdateDateTime;

        [DataMember]
        public string Peer;

        [DataMember]
        public string MessageType;

        [DataMember]
        public string MessageEvent;

        [DataMember]
        public string MessageVersion;

        [DataMember]
        public string MessageFormat;
    }
}
