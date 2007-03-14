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

    [DataContract]
    public class RICSummary : DataContractBase
    {
        public RICSummary(string rpName, string orderingPractitioner, string insurance, DateTime? mpsScheduledTime, string performingFacility)
        {
            this.RequestedProcedureName = rpName;
            this.OrderingPractitioner = orderingPractitioner;
            this.Insurance = insurance;
            this.ModalityProcedureStepScheduledTime = mpsScheduledTime;
            this.PerformingFacility = performingFacility;
        }

        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public string OrderingPractitioner;

        [DataMember]
        public string Insurance;

        [DataMember]
        public DateTime? ModalityProcedureStepScheduledTime;

        [DataMember]
        public string PerformingFacility;
    }

}

