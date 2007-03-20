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
        [DataMember(IsRequired=true)]
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
        public EnumValueInfo Sex;

        [DataMember]
        public List<TelephoneDetail> TelephoneNumbers;

        [DataMember]
        public List<AddressDetail> Addresses;

        [DataMember]
        public List<RICSummary> RICs;
    }
}

