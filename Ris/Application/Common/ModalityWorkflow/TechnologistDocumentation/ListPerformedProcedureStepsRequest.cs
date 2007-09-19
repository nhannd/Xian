using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class ListPerformedProcedureStepsRequest : DataContractBase
    {
        public ListPerformedProcedureStepsRequest(EntityRef procedureStepRef)
        {
            ProcedureStepRef = procedureStepRef;
        }

        [DataMember]
        public EntityRef ProcedureStepRef;
    }
}