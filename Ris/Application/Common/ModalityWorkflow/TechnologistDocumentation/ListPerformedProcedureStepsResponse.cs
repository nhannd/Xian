using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class ListPerformedProcedureStepsResponse : DataContractBase
    {
        public ListPerformedProcedureStepsResponse()
        {
            PerformedProcedureSteps = new List<ModalityPerformedProcedureStepSummary>();
        }

        [DataMember]
        public List<ModalityPerformedProcedureStepSummary> PerformedProcedureSteps;
    }
}