using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class StartModalityProcedureStepResponse : DataContractBase
    {
        public StartModalityProcedureStepResponse()
        {
            RequestedProcedures = new List<RequestedProcedureDetail>();
        }

        [DataMember]
        public List<RequestedProcedureDetail> RequestedProcedures;

        [DataMember]
        public ModalityPerformedProcedureStepSummary ModalityPerformedProcedureStep;

        [DataMember]
        public EntityRef OrderRef;
    }
}