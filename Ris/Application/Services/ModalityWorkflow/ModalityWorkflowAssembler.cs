#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
    public class ModalityWorkflowAssembler
    {
        public ModalityWorklistItemSummary CreateWorklistItemSummary(WorklistItem domainItem, IPersistenceContext context)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            return new ModalityWorklistItemSummary(
                domainItem.ProcedureStepRef,
                domainItem.ProcedureRef,
                domainItem.OrderRef,
                domainItem.PatientRef,
                domainItem.PatientProfileRef,
                new MrnAssembler().CreateMrnDetail(domainItem.Mrn),
                assembler.CreatePersonNameDetail(domainItem.PatientName),
                domainItem.AccessionNumber,
                EnumUtils.GetEnumValueInfo(domainItem.OrderPriority, context),
                EnumUtils.GetEnumValueInfo(domainItem.PatientClass),
                domainItem.DiagnosticServiceName,
                domainItem.ProcedureName,
				domainItem.ProcedurePortable,
				EnumUtils.GetEnumValueInfo(domainItem.ProcedureLaterality, context),
                domainItem.ProcedureStepName,
                domainItem.Time
                );
        }
    }
}
