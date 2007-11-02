using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class GetProcedureProtocolRequest : DataContractBase
    {
        public GetProcedureProtocolRequest(EntityRef requestedProcedureRef)
        {
            RequestedProcedureRef = requestedProcedureRef;
        }

        [DataMember]
        public EntityRef RequestedProcedureRef;
    }
}