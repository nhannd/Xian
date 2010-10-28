#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
    public class ReportingWorkflowAssembler
    {
        public ReportingWorklistItemSummary CreateWorklistItemSummary(ReportingWorklistItem domainItem, IPersistenceContext context)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            return new ReportingWorklistItemSummary(
                domainItem.ProcedureStepRef,
                domainItem.ProcedureRef,
                domainItem.OrderRef,
                domainItem.PatientRef,
                domainItem.PatientProfileRef,
                domainItem.ReportRef,
                new MrnAssembler().CreateMrnDetail(domainItem.Mrn),
                assembler.CreatePersonNameDetail(domainItem.PatientName),
                domainItem.AccessionNumber,
                EnumUtils.GetEnumValueInfo(domainItem.OrderPriority, context),
                EnumUtils.GetEnumValueInfo(domainItem.PatientClass),
                domainItem.DiagnosticServiceName,
                domainItem.ProcedureName,
                domainItem.ProcedurePortable,
                domainItem.HasErrors,
                EnumUtils.GetEnumValueInfo(domainItem.ProcedureLaterality, context),
                domainItem.ProcedureStepName,
                domainItem.Time,
				domainItem.ActivityStatus.HasValue ? EnumUtils.GetEnumValueInfo(domainItem.ActivityStatus.Value, context) : null,
                domainItem.ReportPartIndex
                );
        }
    }
}
