using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class GetProcedurePlanForWorklistItemRequest : DataContractBase
    {
        public GetProcedurePlanForWorklistItemRequest(EntityRef procedureStepRef)
        {
            ProcedureStepRef = procedureStepRef;
        }

        [DataMember]
        public EntityRef ProcedureStepRef;
    }
}