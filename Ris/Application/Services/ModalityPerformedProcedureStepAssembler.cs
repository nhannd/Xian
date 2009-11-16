#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services
{
    public class ModalityPerformedProcedureStepAssembler
    {
        public ModalityPerformedProcedureStepDetail CreateModalityPerformedProcedureStepDetail(ModalityPerformedProcedureStep mpps, IPersistenceContext context)
        {
            ProcedureStepAssembler assembler = new ProcedureStepAssembler();

            // include the details of each MPS in the mpps summary
            List<ProcedureStepSummary> mpsDetails = CollectionUtils.Map<ProcedureStep, ProcedureStepSummary>(
                mpps.Activities,
                delegate(ProcedureStep mps) { return assembler.CreateProcedureStepSummary(mps.As<ModalityProcedureStep>(), context); });

			DicomSeriesAssembler dicomSeriesAssembler = new DicomSeriesAssembler();
        	List<DicomSeriesDetail> dicomSeries = dicomSeriesAssembler.GetDicomSeriesDetails(mpps.DicomSeries);;

        	StaffSummary mppsPerformer = null;
        	ProcedureStepPerformer performer = mpps.Performer as ProcedureStepPerformer;
			if (performer != null)
			{
				StaffAssembler staffAssembler = new StaffAssembler();
				mppsPerformer = staffAssembler.CreateStaffSummary(performer.Staff, context);
			}

        	return new ModalityPerformedProcedureStepDetail(
        		mpps.GetRef(),
        		FormatMppsDescription(mpps),
        		EnumUtils.GetEnumValueInfo(mpps.State, context),
        		mpps.StartTime,
        		mpps.EndTime,
        		mppsPerformer,
        		mpsDetails,
				dicomSeries,
                new Dictionary<string, string>(mpps.ExtendedProperties));
        }

		private string FormatMppsDescription(ModalityPerformedProcedureStep mpps)
		{
			string description = StringUtilities.Combine(mpps.Activities, " / ", 
				delegate(Activity a)
					{
						var mps = a.As<ModalityProcedureStep>();
						return mps.Description;
					});

			return description;
		}
    }
}
