using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class GetProtocolGroupDetailResponse : DataContractBase
    {
        public GetProtocolGroupDetailResponse(ProtocolGroupDetail protocolGroup)
        {
            ProtocolGroup = protocolGroup;
        }

        [DataMember]
        public ProtocolGroupDetail ProtocolGroup;
    }
}