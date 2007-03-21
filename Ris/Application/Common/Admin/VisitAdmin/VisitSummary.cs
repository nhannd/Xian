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
        public string PatientClass;

        [DataMember]
        public string PatientType;

        [DataMember]
        public string AdmissionType;

        [DataMember]
        public string Status;

        [DataMember]
        public DateTime? AdmitDateTime;

        [DataMember]
        public DateTime? DischargeDateTime;
    }
}
