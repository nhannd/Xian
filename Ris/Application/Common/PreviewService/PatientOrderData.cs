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
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [ComVisible(true)]
    [DataContract]
    public class PatientOrderData : DataContractBase
    {
        #region Patient Profile

        [DataMember]
        public string MrnId;

        [DataMember]
        public string MrnAssigningAuthority;

        [DataMember]
        public string HealthcardId;

        [DataMember]
        public string HealthcardAssigningAuthority;

        [DataMember]
        public string HealthcardVersionCode;

        [DataMember]
        public DateTime? HealthcardExpiryDate;

        [DataMember]
        public PersonNameDetail PatientName;

        [DataMember]
        public DateTime? DateOfBirth;

        [DataMember]
        public string Sex;

        [DataMember]
        public string PrimaryLanguage;

        [DataMember]
        public string Religion;

        [DataMember]
        public bool DeathIndicator;

        [DataMember]
        public DateTime? TimeOfDeath;

        #endregion

        #region Visit

        [DataMember]
        public string VisitNumberId;

        [DataMember]
        public string VisitNumberAssigningAuthority;

        [DataMember]
        public string PatientClass;

        [DataMember]
        public string PatientType;

        [DataMember]
        public string AdmissionType;

        [DataMember]
        public string VisitStatus;

        [DataMember]
        public DateTime? AdmitDateTime;

        [DataMember]
        public DateTime? DischargeDateTime;

        [DataMember]
        public string VisitFacilityName;

        [DataMember]
        public string DischargeDisposition;

        [DataMember]
        public bool VipIndicator;

        [DataMember]
        public string PreadmitNumber;

        [DataMember]
        public List<string> AmbulatoryStatuses;

        #endregion

        #region Order

        [DataMember]
        public string PlacerNumber;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public string DiagnosticServiceName;

        [DataMember]
        public DateTime? EnteredDateTime;

        [DataMember]
        public DateTime? SchedulingRequestDateTime;

        [DataMember]
        public PersonNameDetail OrderingPractitionerName;

        [DataMember]
        public string OrderingFacilityName;

        [DataMember]
        public string ReasonForStudy;

        [DataMember]
        public string OrderPriority;

        [DataMember]
        public string CancelReason;

        [DataMember]
        public string OrderStatus;

        [DataMember]
        public DateTime? OrderScheduledStartTime;

        #endregion

        #region Requested Procedure

        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public DateTime? RequestedProcedureScheduledStartTime;

        #endregion

        #region Procedure Step

        [DataMember]
        public string ProcedureStepStatus;

        [DataMember]
        public PersonNameDetail ScheduledPerformerStaffName;

        [DataMember]
        public DateTime? ScheduledStartTime;

        [DataMember]
        public DateTime? ScheduledEndTime;

        [DataMember]
        public PersonNameDetail AssignedStaffName;

        [DataMember]
        public PersonNameDetail PerformerStaffName;

        [DataMember]
        public DateTime? StartTime;

        [DataMember]
        public DateTime? EndTime;

        [DataMember]
        public string DiscontinueReason;

        #endregion

        #region Modality Procedure Step

        [DataMember]
        public string ModalityProcedureStepTypeName;

        [DataMember]
        public string Modality;

        #endregion

        #region Reporting Procedure Step

        [DataMember]
        public ReportSummary Report;

        #endregion

    }
}
