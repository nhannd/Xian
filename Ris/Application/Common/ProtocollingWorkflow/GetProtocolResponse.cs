using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class GetProtocolResponse : DataContractBase
    {
        public GetProtocolResponse(EntityRef protocolRef, ProtocolDetail protocolDetail)
        {
            ProtocolRef = protocolRef;
            ProtocolDetail = protocolDetail;
        }

        [DataMember]
        public EntityRef ProtocolRef;

        [DataMember]
        public ProtocolDetail ProtocolDetail;
    }
}