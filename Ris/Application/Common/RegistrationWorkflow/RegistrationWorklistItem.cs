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
            string mrnID,
            string mrnAssigningAuthority,
            PersonNameDetail name,
            HealthcardDetail healthcard,
            DateTime? dateOfBirth,
            EnumValueInfo sex)
        {
            this.PatientProfileRef = patientProfileRef;
            this.Mrn = new MrnDetail(mrnID, mrnAssigningAuthority);
            this.Name = name;
            this.Healthcard = healthcard;
            this.DateOfBirth = dateOfBirth;
            this.Sex = sex;
        }

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
        public EnumValueInfo Sex;

        public override bool Equals(object obj)
        {
            RegistrationWorklistItem that = obj as RegistrationWorklistItem;
            if (that != null)
                return this.PatientProfileRef.Equals(that.PatientProfileRef);

            return false;
        }

        public override int GetHashCode()
        {
            return this.PatientProfileRef.GetHashCode();
        }
    }
}
