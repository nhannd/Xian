using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class DiscontinueRequestedProcedureOrModalityProcedureStepRequest : DataContractBase
    {
        [DataMember]
        public List<RequestedProcedureDetail> RequestedProcedures;

        [DataMember]
        public List<ModalityProcedureStepDetail> ModalityProcedureSteps;
    }
}