using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.PatientReconcilliation
{
    [DataContract]
    public class PatientProfileSummary : DataContractBase
    {
        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public EntityRef ProfileRef;

        [DataMember]
        public string AssigningAuthority;

        [DataMember]
        public string Mrn;

        [DataMember]
        public string Name;

        [DataMember]
        public string Healthcard;

        [DataMember]
        public DateTime DateOfBirth;

        [DataMember]
        public EnumValueInfo Sex;
    }
}
