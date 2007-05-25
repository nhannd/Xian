using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class PatientProfileSummary : DataContractBase
    {
        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public EntityRef ProfileRef;

        [DataMember]
        public MrnDetail Mrn;

        [DataMember]
        public PersonNameDetail Name;

        [DataMember]
        public HealthcardDetail Healthcard;

        [DataMember]
        public DateTime DateOfBirth;

        [DataMember]
        public EnumValueInfo Sex;
    }
}
