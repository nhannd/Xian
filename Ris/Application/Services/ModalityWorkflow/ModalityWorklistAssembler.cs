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
using ClearCanvas.Ris.Application.Services.Admin;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
    public class ModalityWorklistAssembler
    {
        public ModalityWorklistItem CreateModalityWorklistItem(WorklistItem domainItem, IPersistenceContext context)
        {
            ModalityWorklistItem item = new ModalityWorklistItem();

            item.ProcedureStepRef = domainItem.ModalityProcedureStepRef;

            PersonNameAssembler assembler = new PersonNameAssembler();
            item.PersonNameDetail = assembler.CreatePersonNameDetail(domainItem.PatientName);
            item.Mrn = new MrnDetail(domainItem.Mrn.Id, domainItem.Mrn.AssigningAuthority);
            item.AccessionNumber = domainItem.AccessionNumber;
            item.ModalityProcedureStepName = domainItem.ModalityProcedureStepType.Name;
            item.RequestedProcedureStepName = domainItem.RequestedProcedureType.Name;
            item.ModalityName = domainItem.Modality.Name;

            item.Priority = EnumUtils.GetEnumValueInfo<OrderPriority>(domainItem.Priority, context);
            return item;
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

                        summary.RequestedProcedureName = StringUtilities.Combine<string>(
                            CollectionUtils.Map<RequestedProcedure, string>(o.RequestedProcedures,
                                delegate(RequestedProcedure rp)
                                {
                                    return rp.Type.Name;
                                }),
                            "/");


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
