using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class DocumentProceduresRequest : DataContractBase
    {
        public DocumentProceduresRequest(List<ProcedureStepDetail> procedureSteps)
        {
            ProcedureSteps = procedureSteps;
        }

        [DataMember]
        public List<ProcedureStepDetail> ProcedureSteps;
    }
}