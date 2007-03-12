using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class HL7QueueItemDetail : DataContractBase
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

        [DataMember]
        public string MessageText;
    }
}
