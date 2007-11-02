using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class AddProtocolRequest : DataContractBase
    {
        public AddProtocolRequest(EntityRef requestedProcedureRef)
        {
            RequestedProcedureRef = requestedProcedureRef;
        }

        [DataMember]
        public EntityRef RequestedProcedureRef;
    }
}