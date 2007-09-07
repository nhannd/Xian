using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [ComVisible(true)]
    [DataContract]
    public class PatientOrderData : DataContractBase
    {
        public PatientOrderData()
        {
        }

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
        public DateTime? EarliestScheduledMPSDateTime;

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
