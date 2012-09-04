#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
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
