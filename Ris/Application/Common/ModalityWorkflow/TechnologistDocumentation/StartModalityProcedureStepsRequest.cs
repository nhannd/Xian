using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class StartModalityProcedureStepsRequest : DataContractBase
    {
        public StartModalityProcedureStepsRequest(List<EntityRef> modalityProcedureSteps)
        {
            ModalityProcedureSteps = modalityProcedureSteps;
        }

        [DataMember]
        public List<EntityRef> ModalityProcedureSteps;
    }
}