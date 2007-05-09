using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class LoadVisitEditorFormDataResponse : DataContractBase
    {
        [DataMember]
        public List<string> VisitNumberAssigningAuthorityChoices;

        [DataMember]
        public List<EnumValueInfo> PatientClassChoices;

        [DataMember]
        public List<EnumValueInfo> PatientTypeChoices;

        [DataMember]
        public List<EnumValueInfo> AdmissionTypeChoices;

        [DataMember]
        public List<EnumValueInfo> AmbulatoryStatusChoices;

        [DataMember]
        public List<EnumValueInfo> VisitLocationRoleChoices;

        [DataMember]
        public List<EnumValueInfo> VisitPractitionerRoleChoices;

        [DataMember]
        public List<EnumValueInfo> VisitStatusChoices;

        [DataMember]
        public List<FacilitySummary> FacilityChoices;
    }
}
