using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class PatientProfileDetail : DataContractBase
    {
        public PatientProfileDetail()
        {
            this.Mrn = new MrnDetail();
            this.Healthcard = new HealthcardDetail();
            this.Addresses = new List<AddressDetail>();
            this.ContactPersons = new List<ContactPersonDetail>();
            this.EmailAddresses = new List<EmailAddressDetail>();
            this.TelephoneNumbers = new List<TelephoneDetail>();
            this.Notes = new List<NoteDetail>();
            this.Name = new PersonNameDetail();
        }

        [DataMember]
        public MrnDetail Mrn;

        [DataMember]
        public HealthcardDetail Healthcard;

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

        [DataMember]
        public List<NoteDetail> Notes;
    }
}
