using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class DiscontinueModalityPerformedProcedureStepResponse : DataContractBase
    {
        [DataMember]
        public ProcedurePlanSummary ProcedurePlanSummary;

        [DataMember]
        public ModalityPerformedProcedureStepSummary DiscontinuedMpps;
    }
}