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
            ExternalPractitionerDetail orderingPhysician,
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
        public ExternalPractitionerDetail OrderingPhysician;

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

