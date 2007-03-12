using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    [DataContract]
    public class ListHL7QueueItemsRequest : DataContractBase
    {
        [DataMember]
        public string Direction;

        [DataMember]
        public string StatusCode;

        [DataMember]
        public DateTime? StartingCreationDateTime;

        [DataMember]
        public DateTime? EndingCreationDateTime;

        [DataMember]
        public DateTime? StartingUpdateDateTime;

        [DataMember]
        public DateTime? EndingUpdateDateTime;

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
