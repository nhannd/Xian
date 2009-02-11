#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class ProcedureStepAssembler
	{
		public ProcedureStepSummary CreateProcedureStepSummary(ProcedureStep ps, IPersistenceContext context)
		{
			ProcedureAssembler assembler = new ProcedureAssembler();
			ModalityAssembler modalityAssembler = new ModalityAssembler();
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
			StaffAssembler staffAssembler = new StaffAssembler();
			ModalityAssembler modalityAssembler = new ModalityAssembler();

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
