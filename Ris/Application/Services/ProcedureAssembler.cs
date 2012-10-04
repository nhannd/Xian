#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using System;

namespace ClearCanvas.Ris.Application.Services
{
	public class ProcedureAssembler
	{
		/// <summary>
		/// Creates the most verbose possible procedure detail.
		/// </summary>
		/// <param name="rp"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public ProcedureDetail CreateProcedureDetail(Procedure rp, IPersistenceContext context)
		{
			return CreateProcedureDetail(rp, delegate { return true; }, true, context);
		}

		/// <summary>
		/// Creates procedure detail optionally including specified data.
		/// </summary>
		/// <param name="rp"></param>
		/// <param name="procedureStepFilter"></param>
		/// <param name="includeProtocol"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public ProcedureDetail CreateProcedureDetail(
			Procedure rp,
			Predicate<ProcedureStep> procedureStepFilter,
			bool includeProtocol,
			IPersistenceContext context)
		{
			var detail = new ProcedureDetail
							{
								ProcedureRef = rp.GetRef(),
								Status = EnumUtils.GetEnumValueInfo(rp.Status, context),
								Type = new ProcedureTypeAssembler().CreateSummary(rp.Type),
								ScheduledStartTime = rp.ScheduledStartTime,
								SchedulingCode = EnumUtils.GetEnumValueInfo(rp.SchedulingCode),
								StartTime = rp.StartTime,
								EndTime = rp.EndTime,
								CheckInTime = rp.ProcedureCheckIn.CheckInTime,
								CheckOutTime = rp.ProcedureCheckIn.CheckOutTime,
								PerformingFacility = new FacilityAssembler().CreateFacilitySummary(rp.PerformingFacility),
								PerformingDepartment = rp.PerformingDepartment == null ? null : new DepartmentAssembler().CreateSummary(rp.PerformingDepartment, context),
								Laterality = EnumUtils.GetEnumValueInfo(rp.Laterality, context),
								ImageAvailability = EnumUtils.GetEnumValueInfo(rp.ImageAvailability, context),
								Portable = rp.Portable
							};

			var includedSteps = CollectionUtils.Select(rp.GetWorkflowHistory(), procedureStepFilter);
			if (includedSteps.Count > 0)
			{
				var procedureStepAssembler = new ProcedureStepAssembler();
				detail.ProcedureSteps = CollectionUtils.Map(
					includedSteps,
					(ProcedureStep ps) => procedureStepAssembler.CreateProcedureStepDetail(ps, context));
			}

			// the Protocol may be null, if this procedure has not been protocolled
			if (includeProtocol && rp.ActiveProtocol != null)
			{
				var protocolAssembler = new ProtocolAssembler();
				detail.Protocol = protocolAssembler.CreateProtocolDetail(rp.ActiveProtocol, context);
			}

			return detail;
		}

		public ProcedureSummary CreateProcedureSummary(Procedure rp, IPersistenceContext context)
		{
			var rptAssembler = new ProcedureTypeAssembler();
			var summary = new ProcedureSummary
							{
								OrderRef = rp.Order.GetRef(),
								ProcedureRef = rp.GetRef(),
								Index = rp.Index,
								ScheduledStartTime = rp.ScheduledStartTime,
								SchedulingCode = EnumUtils.GetEnumValueInfo(rp.SchedulingCode),
								PerformingFacility = new FacilityAssembler().CreateFacilitySummary(rp.PerformingFacility),
								Type = rptAssembler.CreateSummary(rp.Type),
								Laterality = EnumUtils.GetEnumValueInfo(rp.Laterality, context),
								Portable = rp.Portable
							};

			return summary;
		}
	}
}
