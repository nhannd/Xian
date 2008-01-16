using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class PriorProcedureSummary: DataContractBase
    {
        public PriorProcedureSummary(
            EntityRef orderRef,
            EntityRef procedureRef,
            EntityRef reportRef,
            string accessionNumber,
            DiagnosticServiceSummary diagnosticService,
            ProcedureTypeSummary procedureType,
            EnumValueInfo reportStatus)
        {
            this.OrderRef = orderRef;
            this.ProcedureRef = procedureRef;
            this.ReportRef = reportRef;
            this.AccessionNumber = accessionNumber;
            this.DiagnosticService = diagnosticService;
            this.ProcedureType = procedureType;
            this.ReportStatus = reportStatus;
        }


        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public EntityRef ProcedureRef;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public DiagnosticServiceSummary DiagnosticService;

        [DataMember]
        public ProcedureTypeSummary ProcedureType;

        [DataMember]
        public EntityRef ReportRef;

        [DataMember]
        public EnumValueInfo ReportStatus;
    }
}
