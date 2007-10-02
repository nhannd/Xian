using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class ProcedurePlanSummary : DataContractBase
    {
        [DataMember] 
        public EntityRef OrderRef;

        [DataMember]
        public DiagnosticServiceSummary DiagnosticServiceSummary;

        [DataMember]
        public List<RequestedProcedureDetail> RequestedProcedures;
    }
}