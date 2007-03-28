using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    [DataContract]
    public class ListHL7QueueItemsRequest : DataContractBase
    {
        public ListHL7QueueItemsRequest()
        {
        }

        public ListHL7QueueItemsRequest(int firstRow, int maxRows)
        {
            this.FirstRow = firstRow;
            this.MaxRows = maxRows;
        }

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

        [DataMember]
        public int FirstRow;

        [DataMember]
        public int MaxRows;
    }
}
