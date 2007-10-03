using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class DiscontinueModalityProcedureStepsRequest : DataContractBase
    {
        public DiscontinueModalityProcedureStepsRequest(List<ModalityProcedureStepDetail> modalityProcedureSteps)
        {
            ModalityProcedureSteps = modalityProcedureSteps;
        }

        [DataMember]
        public List<ModalityProcedureStepDetail> ModalityProcedureSteps;
    }
}