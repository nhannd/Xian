using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    [DataContract]
    public class LoadPatientProfileForAdminEditResponse : DataContractBase
    {
        public LoadPatientProfileForAdminEditResponse(EntityRef patientRef, EntityRef patientProfileRef, PatientProfileDetail patientDetail)
        {
            this.PatientRef = patientRef;
            this.PatientProfileRef = patientProfileRef;
            this.PatientDetail = patientDetail;
        }

        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public PatientProfileDetail PatientDetail;
    }
}
