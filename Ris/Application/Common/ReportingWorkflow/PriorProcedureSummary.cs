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
		public PriorProcedureSummary()
		{
		}

        public PriorProcedureSummary(
            EntityRef orderRef,
            EntityRef procedureRef,
            EntityRef reportRef,
            string accessionNumber,
            DiagnosticServiceSummary diagnosticService,
            ProcedureTypeSummary procedureType,
			bool procedurePortable,
			EnumValueInfo procedureLaterality,
            EnumValueInfo reportStatus,
            DateTime? performedDate)
        {
            this.OrderRef = orderRef;
            this.ProcedureRef = procedureRef;
            this.ReportRef = reportRef;
            this.AccessionNumber = accessionNumber;
            this.DiagnosticService = diagnosticService;
            this.ProcedureType = procedureType;
        	this.ProcedurePortable = procedurePortable;
        	this.ProcedureLaterality = procedureLaterality;
            this.ReportStatus = reportStatus;
            this.PerformedDate = performedDate;
        }


        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public EntityRef ProcedureRef;

        [DataMember]
        public EntityRef ReportRef;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public DiagnosticServiceSummary DiagnosticService;

        [DataMember]
        public ProcedureTypeSummary ProcedureType;

		[DataMember]
		public bool ProcedurePortable;

		[DataMember]
		public EnumValueInfo ProcedureLaterality;

        [DataMember]
        public EnumValueInfo ReportStatus;

        [DataMember]
        public DateTime? PerformedDate;

    }
}
