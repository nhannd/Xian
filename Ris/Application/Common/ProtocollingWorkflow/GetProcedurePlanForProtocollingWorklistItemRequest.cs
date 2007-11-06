using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class GetProcedurePlanForProtocollingWorklistItemRequest : DataContractBase
    {
        public GetProcedurePlanForProtocollingWorklistItemRequest(EntityRef procedureStepRef)
        {
            ProcedureStepRef = procedureStepRef;
        }

        [DataMember]
        public EntityRef ProcedureStepRef;
    }
}