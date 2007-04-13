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
        public RegistrationWorklistItem(EntityRef patientProfileRef,
            string worklistClassName,
            string mrnID,
            string mrnAssigningAuthority,
            PersonNameDetail name,
            HealthcardDetail healthcard,
            DateTime? dateOfBirth,
            string sex)
        {
            this.PatientProfileRef = patientProfileRef;
            this.WorklistClassName = worklistClassName;
            this.MrnID = mrnID;
            this.MrnAssigningAuthority = mrnAssigningAuthority;
            this.Name = name;
            this.Healthcard = healthcard;
            this.DateOfBirth = dateOfBirth;
            this.Sex = sex;
        }

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public string WorklistClassName;

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
    }
}
