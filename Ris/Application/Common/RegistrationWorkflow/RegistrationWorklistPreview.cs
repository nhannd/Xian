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
        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public MrnDetail Mrn;

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

