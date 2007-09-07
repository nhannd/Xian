using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common.PreviewService;
using ClearCanvas.Workflow;
using ClearCanvas.Ris.Application.Services.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Services.PreviewService
{
    public class PreviewServiceAssembler
    {
        public PatientOrderData CreatePatientOrderData(Order order, IPersistenceContext context)
        {
            PatientOrderData data = new PatientOrderData();

            PatientProfile profile = CollectionUtils.SelectFirst<PatientProfile>(order.Patient.Profiles,
                delegate(PatientProfile thisProfile)
                {
                    return thisProfile.Mrn.AssigningAuthority == order.Visit.VisitNumber.AssigningAuthority;
                });

            UpdatePatientOrderData(data, profile, context);
            UpdatePatientOrderData(data, order, context);
            UpdatePatientOrderData(data, order.Visit, context);

            return data;
        }

        public PatientOrderData CreatePatientOrderData(RequestedProcedure rp, IPersistenceContext context)
        {
            PatientOrderData data = new PatientOrderData();

            PatientProfile profile = CollectionUtils.SelectFirst<PatientProfile>(rp.Order.Patient.Profiles,
                delegate(PatientProfile thisProfile)
                {
                    return thisProfile.Mrn.AssigningAuthority == rp.Order.Visit.VisitNumber.AssigningAuthority;
                });

            UpdatePatientOrderData(data, profile, context);
            UpdatePatientOrderData(data, rp.Order, context);
            UpdatePatientOrderData(data, rp.Order.Visit, context);
            UpdatePatientOrderData(data, rp, context);

            return data;
        }

        public PatientOrderData CreatePatientOrderData(ProcedureStep ps, IPersistenceContext context)
        {
            PatientOrderData data = new PatientOrderData();

            PatientProfile profile = CollectionUtils.SelectFirst<PatientProfile>(ps.RequestedProcedure.Order.Patient.Profiles,
                delegate(PatientProfile thisProfile)
                {
                    return thisProfile.Mrn.AssigningAuthority == ps.RequestedProcedure.Order.Visit.VisitNumber.AssigningAuthority;
                });

            UpdatePatientOrderData(data, profile, context);
            UpdatePatientOrderData(data, ps.RequestedProcedure.Order, context);
            UpdatePatientOrderData(data, ps.RequestedProcedure.Order.Visit, context);
            UpdatePatientOrderData(data, ps.RequestedProcedure, context);
            UpdatePatientOrderData(data, ps, context);

            return data;
        }

        #region Private Helpers

        private void UpdatePatientOrderData(PatientOrderData data, PatientProfile profile, IPersistenceContext context)
        {
            PersonNameAssembler nameAssembler = new PersonNameAssembler();

            data.MrnId = profile.Mrn.Id;
            data.MrnAssigningAuthority = profile.Mrn.AssigningAuthority;

            data.HealthcardId = profile.Healthcard.Id;
            data.HealthcardAssigningAuthority = profile.Healthcard.AssigningAuthority;
            data.HealthcardVersionCode = profile.Healthcard.VersionCode;
            data.HealthcardExpiryDate = profile.Healthcard.ExpiryDate;

            data.PatientName = nameAssembler.CreatePersonNameDetail(profile.Name);
            data.DateOfBirth = profile.DateOfBirth;
            data.Sex = EnumUtils.GetValue(profile.Sex, context);
            data.PrimaryLanguage = EnumUtils.GetDisplayValue(profile.PrimaryLanguage);
            data.Religion = EnumUtils.GetDisplayValue(profile.Religion);
            data.DeathIndicator = profile.DeathIndicator;
            data.TimeOfDeath = profile.TimeOfDeath;
        }

        private void UpdatePatientOrderData(PatientOrderData data, Visit visit, IPersistenceContext context)
        {
            // Visit locations and practitioners collections not implemented

            data.VisitNumberId = visit.VisitNumber.Id;
            data.VisitNumberAssigningAuthority = visit.VisitNumber.AssigningAuthority;
            data.PatientClass = EnumUtils.GetDisplayValue(visit.PatientClass);
            data.PatientType = EnumUtils.GetDisplayValue(visit.PatientType);
            data.AdmissionType = EnumUtils.GetDisplayValue(visit.AdmissionType);
            data.VisitStatus = EnumUtils.GetValue(visit.VisitStatus, context);
            data.AdmitDateTime = visit.AdmitDateTime;
            data.DischargeDateTime = visit.DischargeDateTime;
            data.VisitFacilityName = visit.Facility.Name;
            data.DischargeDisposition = visit.DischargeDisposition;
            data.VipIndicator = visit.VipIndicator;
            data.PreadmitNumber = visit.PreadmitNumber;
        }

        private void UpdatePatientOrderData(PatientOrderData data, Order order, IPersistenceContext context)
        {
            PersonNameAssembler nameAssembler = new PersonNameAssembler();

            data.PlacerNumber = order.PlacerNumber;
            data.AccessionNumber = order.AccessionNumber;
            data.DiagnosticServiceName = order.DiagnosticService.Name;
            data.EnteredDateTime = order.EnteredDateTime;
            data.SchedulingRequestDateTime = order.SchedulingRequestDateTime;
            data.OrderingPractitionerName = nameAssembler.CreatePersonNameDetail(order.OrderingPractitioner.Name);
            data.OrderingFacilityName = order.OrderingFacility.Name;
            data.ReasonForStudy = order.ReasonForStudy;
            data.OrderPriority = EnumUtils.GetValue(order.Priority, context);
            data.CancelReason = EnumUtils.GetDisplayValue(order.CancelReason);
            data.OrderStatus = EnumUtils.GetValue(order.Status, context);
            data.EarliestScheduledMPSDateTime = order.ScheduledStartTime;
        }

        private void UpdatePatientOrderData(PatientOrderData data, RequestedProcedure rp, IPersistenceContext context)
        {
            data.RequestedProcedureName = rp.Type.Name;
            data.RequestedProcedureScheduledStartTime = rp.ScheduledStartTime;
        }

        private void UpdatePatientOrderData(PatientOrderData data, ProcedureStep ps, IPersistenceContext context)
        {
            PersonNameAssembler nameAssembler = new PersonNameAssembler();

            data.ProcedureStepStatus = EnumUtils.GetValue(ps.State, context);
            if (ps.Scheduling != null)
            {
                //TODO ScheduledPerformerStaff for ModalityProcedureStepSummary
                //summary.ScheduledPerformerStaff = staffAssembler.CreateStaffSummary(mps.Scheduling.Performer);
                data.ScheduledStartTime = ps.Scheduling.StartTime;
                data.ScheduledEndTime = ps.Scheduling.EndTime;
            }

            if (ps.AssignedStaff != null)
                data.AssignedStaffName = nameAssembler.CreatePersonNameDetail(ps.AssignedStaff.Name);

            if (ps.PerformingStaff != null)
                data.PerformerStaffName = nameAssembler.CreatePersonNameDetail(ps.PerformingStaff.Name);

            data.StartTime = ps.StartTime;
            data.EndTime = ps.EndTime;

            data.DiscontinueReason = "";

            if (ps.Is<ModalityProcedureStep>())
            {
                ModalityProcedureStep mps = ps.As<ModalityProcedureStep>();
                data.ModalityProcedureStepTypeName = mps.Type.Name;
                data.Modality = mps.Modality.Name;
            }
            else if (ps.Is<ReportingProcedureStep>())
            {
                ReportingProcedureStep rps = ps.As<ReportingProcedureStep>();
                if (rps.ReportPart != null)
                {
                    ReportingWorkflowAssembler reportingAssembler = new ReportingWorkflowAssembler();
                    data.Report = reportingAssembler.CreateReportSummary(rps.RequestedProcedure, rps.ReportPart.Report, context);
                }
            }
        }

        #endregion
    }
}
