using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    [DataContract]
    public class ListProtocolGroupsResponse : DataContractBase 
    {
        public ListProtocolGroupsResponse(List<ProtocolGroupSummary> protocolGroups)
        {
            ProtocolGroups = protocolGroups;
        }

        [DataMember]
        public List<ProtocolGroupSummary> ProtocolGroups;
    }
}