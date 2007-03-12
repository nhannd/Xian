using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.HL7Admin
{
    [DataContract]
    public class ListHL7QueueItemsResponse : DataContractBase
    {
        [DataMember]
        public List<HL7QueueItemSummary> QueueItems;
    }
}
