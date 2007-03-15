using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class VisitSummary : DataContractBase
    {
        [DataMember]
        public EntityRef entityRef;

        [DataMember]
        public EntityRef Patient;

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
    }
}
