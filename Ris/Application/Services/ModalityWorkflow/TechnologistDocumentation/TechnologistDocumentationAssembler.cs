#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;
using ClearCanvas.Workflow;
using Iesi.Collections;
using Iesi.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow.TechnologistDocumentation
{
    internal class TechnologistDocumentationAssembler
    {
        public ProcedureStepDetail CreateProcedureStepDetail(ProcedureStep ps, IPersistenceContext context)
        {
            ProcedureStepDetail detail = new ProcedureStepDetail();
            detail.EntityRef = ps.GetRef();
            detail.Name = ps.Name;

            detail.Status = EnumUtils.GetEnumValueInfo(ps.State, context);
            detail.DocumentationPage = new DocumentationPageDetail("http://localhost/RIS/nuclearmedicine.htm");
            detail.PerformedProcedureStep = CreatePerformedProcedureStepDetail(ps.PerformedSteps);

            return detail;
        }

        private PerformedProcedureStepDetail CreatePerformedProcedureStepDetail(ISet<PerformedStep> steps)
        {
            if (steps == null || steps.IsEmpty) return null;

            PerformedProcedureStep pps = CollectionUtils.FirstElement<PerformedProcedureStep>(steps);
            PerformedProcedureStepDetail detail = 
                new PerformedProcedureStepDetail(pps.GetRef(), pps.CreationTime, pps.LastStateChangeTime, pps.StartTime, pps.EndTime, pps.Documentation);

            return detail;
        }

        public void UpdateProcedureStep(ProcedureStep procedureStep, ProcedureStepDetail detail, Staff staff, IDictionary<int, EntityRef> ppsDictionary, IPersistenceContext context)
        {
            ActivityStatus detailStatus = EnumUtils.GetEnumValue<ActivityStatus>(detail.Status);

            PerformedProcedureStep pps = GetPerformedProcedureStep(detail.PerformedProcedureStep, ppsDictionary, context);

            /*
             ////////////// THIS SHOULD NOT BE IN HERE - BUSINESS LOGIC GOES IN THE MODEL!!!! ///////////////////////////
           if (procedureStep.State != detailStatus)
            {
                if (procedureStep.State == ActivityStatus.SC && detailStatus == ActivityStatus.IP)
                {
                    procedureStep.Start(staff);

                    if(procedureStep.RequestedProcedure.Status == ActivityStatus.SC) 
                        procedureStep.RequestedProcedure.Start(staff);
                    //procedureStep.StartTime = detail.PerformedProcedureStep.StartTime;
                }
                else if (procedureStep.State == ActivityStatus.IP && detailStatus == ActivityStatus.CM)
                {
                    procedureStep.Complete(staff);

                    if(pps.State == PerformedStepStatus.IP) pps.Complete();
                    //procedureStep.EndTime = detail.PerformedProcedureStep.EndTime;
                }
                else if (procedureStep.State == ActivityStatus.SC && detailStatus == ActivityStatus.CM)
                {
                    procedureStep.Start(staff);
                    if (procedureStep.RequestedProcedure.Status == ActivityStatus.SC)
                        procedureStep.RequestedProcedure.Start(staff);
                    //procedureStep.StartTime = detail.PerformedProcedureStep.StartTime;

                    procedureStep.Complete(staff);

                    if (pps.State == PerformedStepStatus.IP) pps.Complete();
                    //procedureStep.EndTime = detail.PerformedProcedureStep.EndTime;
                }
            }
            */

            pps.Documentation = detail.PerformedProcedureStep.Blob;

            procedureStep.PerformedSteps.Clear();
            procedureStep.AddPerformedStep(pps);

            if (detail.PerformedProcedureStep.PpsRef == null)
            {
                context.Lock(pps, DirtyState.New);
                context.SynchState();

                detail.PerformedProcedureStep.PpsRef = pps.GetRef();
                ppsDictionary.Add(detail.PerformedProcedureStep.GetHashCode(), detail.PerformedProcedureStep.PpsRef);
            }
        }

        private PerformedProcedureStep GetPerformedProcedureStep(PerformedProcedureStepDetail ppsDetail, IDictionary<int, EntityRef> ppsDictionary, IPersistenceContext context)
        {
            if (ppsDetail.PpsRef == null)
            {
                try
                {
                    ppsDetail.PpsRef = ppsDictionary[ppsDetail.GetHashCode()];
                }
                catch(KeyNotFoundException)
                {
                
                }
            }
            PerformedProcedureStep pps;
            if (ppsDetail.PpsRef != null)
            {
                pps = context.Load<PerformedProcedureStep>(ppsDetail.PpsRef);
            }
            else
            {
                pps = new ModalityPerformedProcedureStep();
            }
            return pps;
        }

        internal RequestedProcedureDetail CreateRequestedProcedureDetail(RequestedProcedure rp, IPersistenceContext context)
        {
            RequestedProcedureDetail detail = new RequestedProcedureDetail();

            detail.RequestedProcedureRef = rp.GetRef();
            detail.Name = rp.Type.Name;
            detail.Status = EnumUtils.GetEnumValueInfo(rp.Status, context);
            detail.ModalityProcedureSteps = CollectionUtils.Map<ModalityProcedureStep, ModalityProcedureStepDetail>(
                rp.ModalityProcedureSteps,
                delegate(ModalityProcedureStep mp) { return CreateModalityProcedureStepDetail(mp, context); });
            
            return detail;
        }

        private ModalityProcedureStepDetail CreateModalityProcedureStepDetail(ModalityProcedureStep mp, IPersistenceContext context)
        {
            ModalityProcedureStepDetail detail = new ModalityProcedureStepDetail();
            detail.Name = mp.Name;
            detail.ModalityProcedureStepRef = mp.GetRef();
            detail.StartDateTime = mp.StartTime;
            detail.EndDateTime = mp.EndTime;
            detail.Status = EnumUtils.GetEnumValueInfo(mp.State, context);
            detail.ModalityId = mp.Modality.Id;
            detail.ModalityName = mp.Modality.Name;
            return detail;
        }

        internal ModalityPerformedProcedureStepSummary CreateModalityPerformedProcedureStepSummary(ModalityPerformedProcedureStep mpps, IPersistenceContext context)
        {
            StringBuilder nameBuilder = new StringBuilder();
            int mpsCount = mpps.Activities.Count;
            foreach(ModalityProcedureStep mps in mpps.Activities)
            {
                nameBuilder.Append(mps.Name);
                if(1 < mpsCount--) nameBuilder.Append(" / ");
            }

            // include the details of each MPS in the mpps summary
            List<ModalityProcedureStepDetail> mpsDetails = CollectionUtils.Map<ModalityProcedureStep, ModalityProcedureStepDetail>(
                mpps.Activities,
                delegate(ModalityProcedureStep mps) { return CreateModalityProcedureStepDetail(mps, context); });

            Dictionary<string, string> extendedProperties = new Dictionary<string, string>();
            foreach (string key in mpps.ExtendedProperties.Keys)
            {
                extendedProperties[key] = (string)mpps.ExtendedProperties[key];
            }


            return new ModalityPerformedProcedureStepSummary(
                mpps.GetRef(),
                nameBuilder.ToString(),
                EnumUtils.GetEnumValueInfo(mpps.State, context),
                mpps.StartTime, 
                mpps.EndTime, 
                "Dummy Performer",
                mpsDetails,
                extendedProperties);
        }

        public ProcedurePlanSummary CreateProcedurePlanSummary(Order order, IPersistenceContext context)
        {
            ProcedurePlanSummary summary = new ProcedurePlanSummary();

            summary.OrderRef = order.GetRef();
            summary.RequestedProcedures = CollectionUtils.Map<RequestedProcedure, RequestedProcedureDetail>(
                order.RequestedProcedures,
                delegate(RequestedProcedure rp) { return CreateRequestedProcedureDetail(rp, context); });
            summary.DiagnosticServiceSummary = 
                new DiagnosticServiceSummary(order.DiagnosticService.GetRef(), order.DiagnosticService.Id, order.DiagnosticService.Name);

            return summary;
        }
    }
}