using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class RegistrationWorklistPreview : DataContractBase
    {
        public RegistrationWorklistPreview(
                EntityRef patientProfileRef,
                string mrnID,
                string mrnAssigningAuthority,
                PersonNameDetail name,
                HealthcardDetail healthcard,
                DateTime? dateOfBirth,
                string sex,
                AddressDetail currentHomeAddress,
                AddressDetail currentWorkAddress,
                TelephoneDetail currentHomePhone,
                TelephoneDetail currentWorkPhone,
                List<TelephoneDetail> telephoneNumbers,
                List<AddressDetail> addresses,
                List<RICSummary> ricList,
                List<AlertNotificationDetail> alertNotifications,
                bool hasReconciliationCandidates)
        {
            this.PatientProfileRef = patientProfileRef;
            this.MrnID = mrnID;
            this.MrnAssigningAuthority = mrnAssigningAuthority;
            this.Name = name;
            this.Healthcard = healthcard;
            this.DateOfBirth = dateOfBirth;
            this.Sex = sex;
            this.CurrentHomeAddress = currentHomeAddress;
            this.CurrentWorkAddress = currentWorkAddress;
            this.CurrentHomePhone = currentHomePhone;
            this.CurrentWorkPhone = currentWorkPhone;
            this.TelephoneNumbers = telephoneNumbers;
            this.Addresses = addresses;
            this.RICs = ricList;
            this.AlertNotifications = alertNotifications;
            this.HasReconciliationCandidates = hasReconciliationCandidates;
        }

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public string MrnID;

        [DataMember]
        public string MrnAssigningAuthority;

        [DataMember]
        public PersonNameDetail Name;

        [DataMember]
        public HealthcardDetail Healthcard;

        [DataMember]
        public DateTime? DateOfBirth;

        [DataMember]
        public string Sex;

        [DataMember]
        public AddressDetail CurrentHomeAddress;

        [DataMember]
        public AddressDetail CurrentWorkAddress;

        [DataMember]
        public TelephoneDetail CurrentHomePhone;

        [DataMember]
        public TelephoneDetail CurrentWorkPhone;

        [DataMember]
        public List<TelephoneDetail> TelephoneNumbers;

        [DataMember]
        public List<AddressDetail> Addresses;

        [DataMember]
        public List<RICSummary> RICs;

        [DataMember]
        public List<AlertNotificationDetail> AlertNotifications;

        [DataMember]
        public bool HasReconciliationCandidates;
    }
}

