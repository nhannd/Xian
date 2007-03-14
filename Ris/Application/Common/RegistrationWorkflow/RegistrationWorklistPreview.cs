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
        public EntityRef PatientProfile;

        [DataMember]
        public string MRNID;

        [DataMember]
        public string MRNAssigningAuthority;

        [DataMember]
        public PersonNameDetail Name;

        [DataMember]
        public HealthcardDetail Healthcard;

        [DataMember]
        public DateTime? DateOfBirth;

        [DataMember]
        public string Sex;

        [DataMember]
        public string PrimaryLanguage;

        [DataMember]
        public string Religion;

        [DataMember]
        public List<TelephoneDetail> TelephoneNumbers;

        [DataMember]
        public List<AddressDetail> Addresses;

        [DataMember]
        public List<RICSummary> RICs;
    }

    [DataContract]
    public class RICSummary : DataContractBase
    {
        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public string OrderingPhysician;

        [DataMember]
        public string Insurance;

        [DataMember]
        public DateTime? ModalityProcedureStepScheduledTime;

        [DataMember]
        public string PerformingFacility;
    }

}

