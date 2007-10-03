using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class StartModalityProcedureStepsRequest : DataContractBase
    {
        public StartModalityProcedureStepsRequest(List<ModalityProcedureStepDetail> modalityProcedureSteps)
        {
            ModalityProcedureSteps = modalityProcedureSteps;
        }

        [DataMember]
        public List<ModalityProcedureStepDetail> ModalityProcedureSteps;
    }
}