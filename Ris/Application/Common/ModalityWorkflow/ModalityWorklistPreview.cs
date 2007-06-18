using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class ModalityWorklistPreview : DataContractBase
    {
        public ModalityWorklistPreview(
            EntityRef procedureStepRef,
            EntityRef patientProfileRef,
            string mrnID,
            string mrnAssigningAuthority,
            PersonNameDetail name,
            HealthcardDetail healthcard,
            DateTime? dateOfBirth,
            string sex,
            string accessionNumber,
            string priority,
            StaffDetail orderingPhysician,
            FacilityDetail facility,
            List<DiagnosticServiceBreakdownSummary> dsBreakdown,
            string mpsName,
            ModalityDetail modality,
            string status,
            string discontinueReason,
            StaffDetail assignedStaff,
            StaffDetail performingStaff,
            DateTime? scheduledStartTime,
            DateTime? scheduledEndTime,
            DateTime? startTime,
            DateTime? endTime,
            List<RICSummary> rics,
            List<AlertNotificationDetail> alertNotifications,
            bool hasReconciliationCandidates)
        {
            this.ProcedureStepRef = procedureStepRef;
            this.PatientProfileRef = patientProfileRef;
            this.Mrn = new MrnDetail(mrnID, mrnAssigningAuthority);
            this.Name = name;
            this.Healthcard = healthcard;
            this.DateOfBirth = dateOfBirth;
            this.Sex = sex;
            this.AccessionNumber = accessionNumber;
            this.Priority = priority;
            this.OrderingPhysician = orderingPhysician;
            this.Facility = facility;
            this.DSBreakdown = dsBreakdown;
            this.MpsName = mpsName;
            this.Modality = modality;
            this.Status = status;
            this.DiscontinueReason = discontinueReason;
            this.AssignedStaff = assignedStaff;
            this.PerformingStaff = performingStaff;
            this.ScheduledStartTime = scheduledStartTime;
            this.ScheduledEndTime = scheduledEndTime;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.RICs = rics;
            this.AlertNotifications = alertNotifications;
            this.HasReconciliationCandidates = hasReconciliationCandidates;
        }

        public ModalityWorklistPreview()
        {
        }

        [DataMember]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public MrnDetail Mrn;

        [DataMember]
        public PersonNameDetail Name;

        [DataMember]
        public HealthcardDetail Healthcard;

        [DataMember]
        public DateTime? DateOfBirth;

        [DataMember]
        public string Sex;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public string Priority;

        [DataMember]
        public StaffDetail OrderingPhysician;

        [DataMember]
        public FacilityDetail Facility;

        [DataMember]
        public List<DiagnosticServiceBreakdownSummary> DSBreakdown;

        [DataMember]
        public string MpsName;

        [DataMember]
        public ModalityDetail Modality;

        [DataMember]
        public string Status;

        [DataMember]
        public string DiscontinueReason;

        [DataMember]
        public StaffDetail AssignedStaff;

        [DataMember]
        public StaffDetail PerformingStaff;

        [DataMember]
        public DateTime? ScheduledStartTime;

        [DataMember]
        public DateTime? ScheduledEndTime;

        [DataMember]
        public DateTime? StartTime;

        [DataMember]
        public DateTime? EndTime;

        [DataMember]
        public List<RICSummary> RICs;

        [DataMember]
        public List<AlertNotificationDetail> AlertNotifications;

        [DataMember]
        public bool HasReconciliationCandidates;
    }
}

