using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class RejectOrderProtocolRequest : DataContractBase
    {
        public RejectOrderProtocolRequest(EntityRef orderRef, EnumValueInfo rejectReason)
        {
            OrderRef = orderRef;
            RejectReason = rejectReason;
        }

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public EnumValueInfo RejectReason;
    }
}