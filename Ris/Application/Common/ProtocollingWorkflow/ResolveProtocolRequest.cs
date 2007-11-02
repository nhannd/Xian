using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class ResolveProtocolRequest : DataContractBase
    {
        public ResolveProtocolRequest(EntityRef protocolRef)
        {
            ProtocolRef = protocolRef;
        }

        [DataMember]
        public EntityRef ProtocolRef;
    }
}