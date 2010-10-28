#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
	public class RegistrationWorkflowAssembler
	{
		public RegistrationWorklistItemSummary CreateWorklistItemSummary(WorklistItem domainItem, IPersistenceContext context)
		{
			var nameAssembler = new PersonNameAssembler();
			var healthcardAssembler = new HealthcardAssembler();

			return new RegistrationWorklistItemSummary(
				domainItem.ProcedureRef,
				domainItem.OrderRef,
				domainItem.PatientRef,
				domainItem.PatientProfileRef,
				new MrnAssembler().CreateMrnDetail(domainItem.Mrn),
				nameAssembler.CreatePersonNameDetail(domainItem.PatientName),
				domainItem.AccessionNumber,
				EnumUtils.GetEnumValueInfo(domainItem.OrderPriority, context),
				EnumUtils.GetEnumValueInfo(domainItem.PatientClass),
				domainItem.DiagnosticServiceName,
				domainItem.ProcedureName,
				domainItem.ProcedurePortable,
				EnumUtils.GetEnumValueInfo(domainItem.ProcedureLaterality, context),
				domainItem.Time);
		}
	}
}
