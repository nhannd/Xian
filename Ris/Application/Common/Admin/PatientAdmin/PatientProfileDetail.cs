using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    [DataContract]
    public class PatientProfileDetail : DataContractBase
    {
        [DataMember]
        public string Mrn;

        [DataMember]
        public string MrnAssigningAuthority;

        [DataMember]
        public string Healthcard;

        [DataMember]
        public string HealthcardAssigningAuthority;

        [DataMember]
        public PersonNameDetail Name;

        [DataMember]
        public DateTime DateOfBirth;

        [DataMember]
        public EnumValueInfo Sex;

        [DataMember]
        public EnumValueInfo PrimaryLanguage;

        [DataMember]
        public EnumValueInfo Religion;

        [DataMember]
        public bool DeathIndicator;

        [DataMember]
        public DateTime? TimeOfDeath;

        //[DataMember]
        //public List<EnumValueInfo> AmbulatoryStatuses;

        [DataMember]
        public List<AddressDetail> Addresses;

        [DataMember]
        public List<TelephoneDetail> TelephoneNumbers;

        [DataMember]
        public List<EmailAddressDetail> EmailAddresses;

        [DataMember]
        public List<ContactPersonDetail> ContactPersons;

        //[DataMember]
        //public List<NoteDetail> Notes;
    }
}
