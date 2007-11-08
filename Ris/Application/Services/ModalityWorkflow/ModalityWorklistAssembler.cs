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

using System;
using System.Collections.Generic;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.PatientReconciliation;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
    public class ModalityWorklistAssembler
    {
        public ModalityWorklistItem CreateModalityWorklistItem(WorklistItem domainItem, IPersistenceContext context)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            return new ModalityWorklistItem(
                domainItem.PatientRef,
                domainItem.PatientProfileRef,
                domainItem.OrderRef,
                domainItem.ModalityProcedureStepRef,
                new MrnDetail(domainItem.Mrn.Id, domainItem.Mrn.AssigningAuthority),
                assembler.CreatePersonNameDetail(domainItem.PatientName),
                EnumUtils.GetEnumValueInfo(domainItem.OrderPriority, context),
                EnumUtils.GetEnumValueInfo(domainItem.PatientClass),
                domainItem.AccessionNumber,
                domainItem.RequestedProcedureType.Name,
                domainItem.ModalityProcedureStepType.Name,
                domainItem.Modality.Name,
                domainItem.ScheduledStartTime,
                domainItem.DiagnosticServiceName);
        }

        public ModalityWorklistPreview CreateWorklistPreview(ModalityProcedureStep mps, string patientProfileAuthority, IPersistenceContext context)
        {
            ModalityWorklistPreview preview = new ModalityWorklistPreview();

            preview.ProcedureStepRef = mps.GetRef();

            foreach (PatientProfile profile in mps.RequestedProcedure.Order.Patient.Profiles)
            {
                if (profile.Mrn.AssigningAuthority.Equals(patientProfileAuthority))
                {
                    preview.PatientProfileRef = profile.GetRef();

                    preview.Healthcard = new HealthcardAssembler().CreateHealthcardDetail(profile.Healthcard);
                    preview.Mrn = new MrnDetail(profile.Mrn.Id, profile.Mrn.AssigningAuthority);
                    preview.Name = new PersonNameAssembler().CreatePersonNameDetail(profile.Name);
                    preview.Sex = EnumUtils.GetValue(profile.Sex, context);
                    preview.DateOfBirth = profile.DateOfBirth;

                    IPatientReconciliationStrategy strategy = (IPatientReconciliationStrategy)(new PatientReconciliationStrategyExtensionPoint()).CreateExtension();
                    IList<PatientProfileMatch> matches = strategy.FindReconciliationMatches(profile, context);

                    preview.HasReconciliationCandidates = matches.Count > 0;

                    break;
                }
            }

            // Order level details
            preview.AccessionNumber = mps.RequestedProcedure.Order.AccessionNumber;
            preview.Priority = EnumUtils.GetValue(mps.RequestedProcedure.Order.Priority, context);
            preview.OrderingPhysician = new ExternalPractitionerAssembler().CreateExternalPractitionerDetail(mps.RequestedProcedure.Order.OrderingPractitioner, context);
            preview.Facility = new FacilityAssembler().CreateFacilityDetail(mps.RequestedProcedure.Order.OrderingFacility);

            //preview.DSBreakdown = new List<string>();
            List<ModalityProcedureStep> mpsList = new List<ModalityProcedureStep>();
            foreach (RequestedProcedure rp in mps.RequestedProcedure.Order.RequestedProcedures)
            {
                List<ProcedureStep> psList = CollectionUtils.Select<ProcedureStep, List<ProcedureStep>>(rp.ProcedureSteps,
                    delegate(ProcedureStep procedureStep)
                    {
                        return procedureStep.Is<ModalityProcedureStep>();
                    });

                mpsList.AddRange(CollectionUtils.Map<ProcedureStep, ModalityProcedureStep, List<ModalityProcedureStep>>(psList,
                    delegate(ProcedureStep ps)
                    {
                        return ps.As<ModalityProcedureStep>();
                    }));
            }

            preview.DSBreakdown = CollectionUtils.Map<ModalityProcedureStep, DiagnosticServiceBreakdownSummary, List<DiagnosticServiceBreakdownSummary>>(
                mpsList,
                delegate(ModalityProcedureStep siblingMps)
                {
                    return new DiagnosticServiceBreakdownSummary(siblingMps.RequestedProcedure.Order.DiagnosticService.Name,
                        siblingMps.RequestedProcedure.Type.Name,
                        siblingMps.Name,
                        EnumUtils.GetValue(siblingMps.State, context),
                        siblingMps.Equals(mps));
                });

            preview.MpsName = mps.Name;
            preview.Modality = new ModalityAssembler().CreateModalityDetail(mps.Modality);
            preview.Status = EnumUtils.GetValue(mps.State, context);
            preview.DiscontinueReason = "";
            
            StaffAssembler staffAssembler = new StaffAssembler();
            preview.AssignedStaff = mps.AssignedStaff == null
                ? null
                : staffAssembler.CreateStaffDetail(mps.AssignedStaff, context);
            preview.PerformingStaff = mps.PerformingStaff == null
                ? null
                : staffAssembler.CreateStaffDetail(mps.PerformingStaff, context);
            
            preview.ScheduledStartTime = mps.Scheduling.StartTime;
            preview.ScheduledEndTime = mps.Scheduling.EndTime;
            preview.StartTime = mps.StartTime;
            preview.EndTime = mps.EndTime;

            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            preview.RICs = CollectionUtils.Map<Order, RICSummary, List<RICSummary>>(
                    context.GetBroker<IRegistrationWorklistBroker>().GetOrdersForPatientPreview(mps.RequestedProcedure.Order.Patient),
                    delegate(Order o)
                    {
                        RICSummary summary = new RICSummary();
                        summary.OrderingFacility = o.OrderingFacility.Name;
                        summary.OrderingPractitioner = nameAssembler.CreatePersonNameDetail(o.OrderingPractitioner.Name);
                        summary.Insurance = "";
                        summary.Status = EnumUtils.GetValue(o.Status, context);

                        summary.RequestedProcedureName = StringUtilities.Combine(
                            CollectionUtils.Map<RequestedProcedure, string>(o.RequestedProcedures,
                                delegate(RequestedProcedure rp) { return rp.Type.Name; }), "/");

                        List<DateTime?> listScheduledTime = CollectionUtils.Map<RequestedProcedure, DateTime?, List<DateTime?>>(o.RequestedProcedures,
                            delegate(RequestedProcedure rp)
                            {
                                CheckInProcedureStep cps = rp.CheckInProcedureStep;
                                return cps.Scheduling.StartTime;
                            });

                        if (listScheduledTime.Count > 1)
                            listScheduledTime.Sort(new Comparison<DateTime?>(RICSummary.CompreMoreRecent));

                        summary.ScheduledTime = CollectionUtils.FirstElement<DateTime?>(listScheduledTime, null);

                        return summary;
                    });

            //TODO: Technologist workflow hasn't been fully defined yet, only pass back the PatientProfile now
            preview.AlertNotifications = new List<AlertNotificationDetail>();


            return preview;
        }
    
        public ModalityProcedureStepSearchCriteria CreateSearchCriteria(ModalityWorklistSearchData criteria)
        {
            // TODO: add validation and throw RequestValidationException if necessary

            ModalityProcedureStepSearchCriteria mpsSearchCriteria = new ModalityProcedureStepSearchCriteria();

            ActivityStatus status = (ActivityStatus)Enum.Parse(typeof(ActivityStatus), criteria.ActivityStatusCode);
            mpsSearchCriteria.State.EqualTo(status);

            return mpsSearchCriteria;
        }
    }
}
