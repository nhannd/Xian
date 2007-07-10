using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

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
        public string RequestedProcedureTypeName;

        #endregion

        #region Modality Procedure Step

        [DataMember]
        public string MPSState;

        [DataMember]
        public PersonNameDetail ScheduledPerformerStaffName;

        [DataMember]
        public DateTime? ScheduledStartTime;

        [DataMember]
        public DateTime? ScheduledEndTime;

        [DataMember]
        public PersonNameDetail PerformerStaffName;

        [DataMember]
        public DateTime? StartTime;

        [DataMember]
        public DateTime? EndTime;

        [DataMember]
        public string ModalityProcedureStepTypeName;

        [DataMember]
        public string Modality;

        #endregion

        /// <summary>
        /// Compare whether time1 is more recent than time2.
        /// Time of null is considered unscheduled and furthest into the future.
        /// Any time after today is considered more recent than the past.
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns>0 if both time are equal, -1 if time1 is more recent and 1 if time2 is more recent</returns>
        public static int CompreMoreRecent(DateTime? time1, DateTime? time2)
        {
            if (time1 == null && time2 == null)
                return 0;

            DateTime today = Platform.Time.Date;
            bool timeOneMoreRecent = false;

            if (time1 == null)
            {
                if (time2.Value.CompareTo(today) < 0)
                    timeOneMoreRecent = true;  // time1 in the future, time2 in the past
            }
            else if (time2 == null)
            {
                if (time1.Value.CompareTo(today) < 0)
                    timeOneMoreRecent = false;  // time1 in the past, time2 in the future
            }
            else
            {
                if (time1.Value == time2.Value)
                    return 0;

                long timeOneSpan = time1.Value.Subtract(today).Ticks;
                long timeTwoSpan = time2.Value.Subtract(today).Ticks;

                if (timeOneSpan > 0 && timeTwoSpan > 0)
                {
                    // Both in the future
                    timeOneMoreRecent = timeOneSpan < timeTwoSpan;
                }
                else if (timeOneSpan < 0 && timeTwoSpan < 0)
                {
                    // Both in the past
                    timeOneMoreRecent = timeOneSpan > timeTwoSpan;
                }
                else
                {
                    timeOneMoreRecent = timeOneSpan > timeTwoSpan;
                }
            }

            return timeOneMoreRecent ? -1 : 1;
        }

    
    }
}
