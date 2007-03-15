using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class VisitDetail : DataContractBase
    {
        [DataMember]
        public string VisitNumberId;

        [DataMember]
        public string VisitNumberAssigningAuthority;

        [DataMember]
        public EnumValueInfo PatientClass;

        [DataMember]
        public EnumValueInfo PatientType;

        [DataMember]
        public EnumValueInfo AdmissionType;

        [DataMember]
        public EnumValueInfo Status;

        [DataMember]
        public DateTime? AdmitDateTime;

        [DataMember]
        public DateTime? DischargeDateTime;

        [DataMember]
        public FacilitySummary Facility;

        [DataMember]
        public string DischargeDisposition;

        [DataMember]
        public List<VisitLocationDetail> Locations;

        [DataMember]
        public List<VisitPractitionerDetail> Practitioners;

        [DataMember]
        public bool VipIndicator;

        [DataMember]
        public string PreadmitNumber;

        [DataMember]
        public List<EnumValueInfo> AmbulatoryStatuses;
    }
}
