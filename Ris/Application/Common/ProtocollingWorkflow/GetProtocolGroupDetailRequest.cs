using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class GetProtocolGroupDetailRequest : DataContractBase
    {
        public GetProtocolGroupDetailRequest(ProtocolGroupSummary protocolGroup)
        {
            ProtocolGroup = protocolGroup;
        }

        [DataMember]
        public ProtocolGroupSummary ProtocolGroup;
    }
}