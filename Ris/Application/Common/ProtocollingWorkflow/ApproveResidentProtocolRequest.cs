using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class ApproveResidentProtocolRequest : DataContractBase
    {
        public ApproveResidentProtocolRequest(EntityRef protocolRef)
        {
            ProtocolRef = protocolRef;
        }

        [DataMember]
        public EntityRef ProtocolRef;
    }
}