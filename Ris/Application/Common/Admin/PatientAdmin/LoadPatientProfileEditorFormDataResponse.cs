using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    [DataContract]
    public class LoadPatientProfileEditorFormDataResponse : DataContractBase
    {
        [DataMember]
        public List<string> HealthcardAssigningAuthorityChoices;

        [DataMember]
        public List<string> MrnAssigningAuthorityChoices;

        [DataMember]
        public List<EnumValueInfo> SexChoices;

        [DataMember]
        public List<EnumValueInfo> AddressTypeChoices;

        [DataMember]
        public List<string> AddressProvinceChoices;

        [DataMember]
        public List<string> AddressCountryChoices;

        [DataMember]
        public List<EnumValueInfo> PhoneTypeChoices;

        [DataMember]
        public List<EnumValueInfo> ContactPersonTypeChoices;

        [DataMember]
        public List<EnumValueInfo> ContactPersonRelationshipChoices;

        [DataMember]
        public List<NoteCategorySummary> NoteCategoryChoices;

        [DataMember]
        public List<EnumValueInfo> PrimaryLanguageChoices;

        [DataMember]
        public List<EnumValueInfo> ReligionChoices;
    }
}
