using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class ListProtocolGroupsForProcedureResponse : DataContractBase
    {
        public ListProtocolGroupsForProcedureResponse(List<ProtocolGroupSummary> protocolGroups, ProtocolGroupSummary initialProtocolGroup)
        {
            ProtocolGroups = protocolGroups;
            InitialProtocolGroup = initialProtocolGroup;
        }

        [DataMember]
        public List<ProtocolGroupSummary> ProtocolGroups;

        [DataMember]
        public ProtocolGroupSummary InitialProtocolGroup;
    }
}