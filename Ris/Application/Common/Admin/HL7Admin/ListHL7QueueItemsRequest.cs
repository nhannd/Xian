using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    [DataContract]
    public class ListHL7QueueItemsRequest : DataContractBase
    {
        [DataMember]
        public EnumValueInfo Direction;

        [DataMember]
        public EnumValueInfo StatusCode;

        [DataMember]
        public DateTime? StartingCreationDateTime;

        [DataMember]
        public DateTime? EndingCreationDateTime;

        [DataMember]
        public DateTime? StartingUpdateDateTime;

        [DataMember]
        public DateTime? EndingUpdateDateTime;

        [DataMember]
        public EnumValueInfo Peer;

        [DataMember]
        public string MessageType;

        [DataMember]
        public EnumValueInfo MessageVersion;

        [DataMember]
        public EnumValueInfo MessageFormat;
    }
}
