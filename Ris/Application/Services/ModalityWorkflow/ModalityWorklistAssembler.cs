using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Services.Admin;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Workflow;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Workflow.Brokers;
using ClearCanvas.Healthcare.PatientReconciliation;
using ClearCanvas.Common.Utilities;

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
            item.MrnID = domainItem.Mrn.Id;
            item.MrnAssigningAuthority = domainItem.Mrn.AssigningAuthority;

            item.AccessionNumber = domainItem.AccessionNumber;
            item.ModalityProcedureStepName = domainItem.ModalityProcedureStepType.Name;
            item.RequestedProcedureStepName = domainItem.RequestedProcedureType.Name;
            item.ModalityName = domainItem.Modality.Name;

            OrderPriorityEnumTable orderPriorities = context.GetBroker<IOrderPriorityEnumBroker>().Load();
            item.Priority = new EnumValueInfo(domainItem.Priority.ToString(), orderPriorities[domainItem.Priority].Value);

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
                    SexEnumTable sexTable = context.GetBroker<ISexEnumBroker>().Load();
                    preview.Sex = sexTable[profile.Sex].Value;
                    preview.DateOfBirth = profile.DateOfBirth;

                    IPatientReconciliationStrategy strategy = (IPatientReconciliationStrategy)(new PatientReconciliationStrategyExtensionPoint()).CreateExtension();
                    IList<PatientProfileMatch> matches = strategy.FindReconciliationMatches(profile, context);

                    preview.HasReconciliationCandidates = matches.Count > 0;

                    break;
                }
            }

            // Order level details
            preview.AccessionNumber = mps.RequestedProcedure.Order.AccessionNumber;
            OrderPriorityEnumTable priorityTable = context.GetBroker<IOrderPriorityEnumBroker>().Load();
            preview.Priority = priorityTable[mps.RequestedProcedure.Order.Priority].Value;
            preview.OrderingPhysician = new StaffAssembler().CreateStaffDetail(mps.RequestedProcedure.Order.OrderingPractitioner, context);
            preview.Facility = new FacilityAssembler().CreateFacilityDetail(mps.RequestedProcedure.Order.OrderingFacility);

            //preview.DSBreakdown = new List<string>();
            List<ProcedureStep> mpsList = new List<ProcedureStep>();
            foreach (RequestedProcedure rp in mps.RequestedProcedure.Order.RequestedProcedures)
            {
                mpsList.AddRange(CollectionUtils.Select<ProcedureStep, List<ProcedureStep>>(rp.ProcedureSteps,
                    delegate(ProcedureStep procedureStep)
                    {
                        return procedureStep is ModalityProcedureStep;
                    }));
            }

            ActivityStatusEnumTable activityStatusTable = context.GetBroker<IActivityStatusEnumBroker>().Load();
            preview.DSBreakdown = CollectionUtils.Map<ModalityProcedureStep, DiagnosticServiceBreakdownSummary, List<DiagnosticServiceBreakdownSummary>>(
                mpsList,
                delegate(ModalityProcedureStep siblingMps)
                {
                    return new DiagnosticServiceBreakdownSummary(siblingMps.RequestedProcedure.Order.DiagnosticService.Name,
                        siblingMps.RequestedProcedure.Type.Name,
                        siblingMps.Name,
                        activityStatusTable[siblingMps.State].Value,
                        siblingMps.Equals(mps));
                });

            preview.MpsName = mps.Name;
            preview.Modality = new ModalityAssembler().CreateModalityDetail(mps.Modality);
            preview.Status = activityStatusTable[mps.State].Value;
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

            OrderStatusEnumTable orderStatusTable = context.GetBroker<IOrderStatusEnumBroker>().Load();
            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            preview.RICs = CollectionUtils.Map<Order, RICSummary, List<RICSummary>>(
                    context.GetBroker<IRegistrationWorklistBroker>().GetOrdersForPatientPreview(mps.RequestedProcedure.Order.Patient),
                    delegate(Order o)
                    {
                        RICSummary summary = new RICSummary();
                        summary.OrderingFacility = o.OrderingFacility.Name;
                        summary.OrderingPractitioner = nameAssembler.CreatePersonNameDetail(o.OrderingPractitioner.Name);
                        summary.Insurance = "";
                        summary.Status = orderStatusTable[o.Status].Value;

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
                                CheckInProcedureStep cps = (CheckInProcedureStep)CollectionUtils.SelectFirst<ProcedureStep>(rp.ProcedureSteps,
                                    delegate(ProcedureStep step)
                                    {
                                        return step is CheckInProcedureStep;
                                    });

                                return cps.Scheduling.StartTime;
                            });

                        if (listScheduledTime.Count > 1)
                            listScheduledTime.Sort(new Comparison<DateTime?>(RICSummary.CompreMoreRecent));

                        summary.ScheduledTime = CollectionUtils.FirstElement<DateTime?>(listScheduledTime);

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
