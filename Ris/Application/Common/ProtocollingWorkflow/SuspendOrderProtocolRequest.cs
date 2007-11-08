using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class SuspendOrderProtocolRequest : DataContractBase
    {
        public SuspendOrderProtocolRequest(EntityRef orderRef, EnumValueInfo suspendReason)
        {
            OrderRef = orderRef;
            SuspendReason = suspendReason;
        }

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public EnumValueInfo SuspendReason;
    }
}