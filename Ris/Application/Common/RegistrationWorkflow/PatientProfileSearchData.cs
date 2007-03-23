using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class PatientProfileSearchData : DataContractBase
    {
        public PatientProfileSearchData(EntityRef patientProfileRef,
            string mrnID,
            string mrnAssigningAuthority,
            string healthcardID,
            string familyName,
            string givenName,
            EnumValueInfo sex,
            DateTime? dateOfBirth)
        {
            this.PatientProfile = patientProfileRef;
            this.MrnID = mrnID;
            this.MrnAssigningAuthority = mrnAssigningAuthority;
            this.HealthcardID = healthcardID;
            this.FamilyName = familyName;
            this.GivenName = givenName;
            this.Sex = sex;
            this.DateOfBirth = dateOfBirth;
        }

        public PatientProfileSearchData()
        {
        }

        [DataMember]
        public EntityRef PatientProfile;

        [DataMember]
        public string MrnID;

        [DataMember]
        public string MrnAssigningAuthority;

        [DataMember]
        public string HealthcardID;

        [DataMember]
        public string FamilyName;

        [DataMember]
        public string GivenName;

        [DataMember]
        public EnumValueInfo Sex;

        [DataMember]
        public DateTime? DateOfBirth;
    }
}
