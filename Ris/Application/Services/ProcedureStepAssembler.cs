#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class ModalityProcedureStepAssembler
	{
		public ModalityProcedureStepSummary CreateProcedureStepSummary(ModalityProcedureStep mps, IPersistenceContext context)
		{
			var psSummary = new ProcedureStepAssembler().CreateProcedureStepSummary(mps, context);
			return new ModalityProcedureStepSummary(psSummary, mps.Description);
		}
	}
	
	public class ProcedureStepAssembler
	{
		public ProcedureStepSummary CreateProcedureStepSummary(ProcedureStep ps, IPersistenceContext context)
		{
			var assembler = new ProcedureAssembler();
			var modalityAssembler = new ModalityAssembler();
			return new ProcedureStepSummary(
				ps.GetRef(),
				ps.Name,
				EnumUtils.GetEnumValueInfo(ps.State, context),
				ps.StartTime,
				ps.EndTime,
				ps.Is<ModalityProcedureStep>() ? modalityAssembler.CreateModalitySummary(ps.As<ModalityProcedureStep>().Modality) : null,
				assembler.CreateProcedureSummary(ps.Procedure, context));
		}

		public ProcedureStepDetail CreateProcedureStepDetail(ProcedureStep ps, IPersistenceContext context)
		{
			var staffAssembler = new StaffAssembler();
			var modalityAssembler = new ModalityAssembler();

			return new ProcedureStepDetail(
				ps.GetRef(),
				ps.Name,
				ps.GetClass().Name,
				ps.Is<ModalityProcedureStep>() ? ps.As<ModalityProcedureStep>().Description : null,
				EnumUtils.GetEnumValueInfo(ps.State, context),
				ps.CreationTime,
				ps.Scheduling == null ? null : ps.Scheduling.StartTime,
				ps.StartTime,
				ps.EndTime,
				ps.AssignedStaff == null ? null : staffAssembler.CreateStaffSummary(ps.AssignedStaff, context),
				ps.PerformingStaff == null ? null : staffAssembler.CreateStaffSummary(ps.PerformingStaff, context),
				ps.Is<ModalityProcedureStep>() ? modalityAssembler.CreateModalitySummary(ps.As<ModalityProcedureStep>().Modality) : null);
		}
	}
}
