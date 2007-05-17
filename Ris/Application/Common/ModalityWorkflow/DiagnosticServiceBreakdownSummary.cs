using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class DiagnosticServiceBreakdownSummary : DataContractBase
    {
        public DiagnosticServiceBreakdownSummary(
            string diagnosticServiceName,
            string requestedProcedureName,
            string modalityProcedureStepName,
            string modalityProcedureStepStatus,
            bool active)
        {
            this.DiagnosticServiceName = diagnosticServiceName;
            this.RequestedProcedureName = requestedProcedureName;
            this.ModalityProcedureStepName = modalityProcedureStepName;
            this.ModalityProcedureStepStatus = modalityProcedureStepStatus;
            this.Active = active;
        }

        [DataMember]
        public string DiagnosticServiceName;

        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public string ModalityProcedureStepName;

        [DataMember]
        public string ModalityProcedureStepStatus;

        [DataMember]
        public bool Active;
    }
}
