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
	public class ModalityProcedureStepAssembler
	{
		public ModalityProcedureStepSummary CreateModalityProcedureStepSummary(ModalityProcedureStep mp, IPersistenceContext context)
		{
			ProcedureAssembler assembler = new ProcedureAssembler();
			ModalityAssembler modalityAssembler = new ModalityAssembler();
			return new ModalityProcedureStepSummary(
				mp.GetRef(),
				mp.Name,
				EnumUtils.GetEnumValueInfo(mp.State, context),
				mp.StartTime,
				mp.EndTime,
				modalityAssembler.CreateModalitySummary(mp.Modality),
				assembler.CreateProcedureSummary(mp.Procedure, context));
		}

		public ModalityProcedureStepDetail CreateModalityProcedureStepDetail(ModalityProcedureStep mp, IPersistenceContext context)
		{
			StaffAssembler staffAssembler = new StaffAssembler();
			ModalityAssembler modalityAssembler = new ModalityAssembler();

			return new ModalityProcedureStepDetail(
				mp.GetRef(),
				mp.Name,
				EnumUtils.GetEnumValueInfo(mp.State, context),
				mp.Scheduling == null ? null : mp.Scheduling.StartTime,
				mp.StartTime,
				mp.EndTime,
				mp.AssignedStaff == null ? null : staffAssembler.CreateStaffSummary(mp.AssignedStaff, context),
				mp.PerformingStaff == null ? null : staffAssembler.CreateStaffSummary(mp.PerformingStaff, context),
				modalityAssembler.CreateModalitySummary(mp.Modality));
		}
	}
}
