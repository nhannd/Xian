using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class SaveProtocolRequest : DataContractBase
    {
        public SaveProtocolRequest(EntityRef protocolRef, ProtocolDetail protocolDetail)
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