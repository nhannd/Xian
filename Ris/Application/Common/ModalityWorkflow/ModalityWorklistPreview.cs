using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class ModalityWorklistPreview : DataContractBase
    {
        public ModalityWorklistPreview(
            EntityRef procedureStepRef,
            EntityRef patientProfileRef,
            string mrnID,
            string mrnAssigningAuthority,
            PersonNameDetail name,
            HealthcardDetail healthcard,
            DateTime? dateOfBirth,
            string sex,
            List<AlertNotificationDetail> alertNotifications,
            bool hasReconciliationCandidates)
        {
            this.ProcedureStepRef = procedureStepRef;
            this.PatientProfileRef = patientProfileRef;
            this.Mrn = new MrnDetail(mrnID, mrnAssigningAuthority);
            this.Name = name;
            this.Healthcard = healthcard;
            this.DateOfBirth = dateOfBirth;
            this.Sex = sex;
            this.AlertNotifications = alertNotifications;
            this.HasReconciliationCandidates = hasReconciliationCandidates;
        }

        public ModalityWorklistPreview()
        {
        }

        [DataMember]
        public EntityRef ProcedureStepRef;

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

        //[DataMember]
        //public AddressDetail CurrentHomeAddress;

        //[DataMember]
        //public AddressDetail CurrentWorkAddress;

        //[DataMember]
        //public TelephoneDetail CurrentHomePhone;

        //[DataMember]
        //public TelephoneDetail CurrentWorkPhone;

        //[DataMember]
        //public List<TelephoneDetail> TelephoneNumbers;

        //[DataMember]
        //public List<AddressDetail> Addresses;

        // Tech preview items here

        [DataMember]
        public List<AlertNotificationDetail> AlertNotifications;

        [DataMember]
        public bool HasReconciliationCandidates;
    }
}
