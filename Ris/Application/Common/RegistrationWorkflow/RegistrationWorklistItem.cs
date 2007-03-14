using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class RegistrationWorklistItem : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public EntityRef PatientProfileRef;

        [DataMember(IsRequired = true)]
        public string WorklistClassName;

        [DataMember]
        public string MrnAssigningAuthority;

        [DataMember]
        public string MrnID;

        [DataMember]
        public PersonNameDetail Name;

        [DataMember]
        public HealthcardDetail Healthcard;

        [DataMember]
        public DateTime? DateOfBirth;

        [DataMember]
        public EnumValueInfo Sex;
    }
}
