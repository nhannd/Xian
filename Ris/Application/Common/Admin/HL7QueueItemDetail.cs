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
        public EnumValueInfo Direction;

        [DataMember]
        public EnumValueInfo StatusCode;

        [DataMember]
        public string StatusDescription;

        [DataMember]
        public DateTime CreationDateTime;

        [DataMember]
        public DateTime? UpdateDateTime;

        [DataMember]
        public EnumValueInfo Peer;

        [DataMember]
        public string MessageType;

        [DataMember]
        public string MessageEvent;

        [DataMember]
        public EnumValueInfo MessageVersion;

        [DataMember]
        public EnumValueInfo MessageFormat;

        [DataMember]
        public string MessageText;
    }
}
