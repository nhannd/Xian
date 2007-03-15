using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    [DataContract]
    public class SaveAdminEditsForPatientProfileRequest : DataContractBase
    {
        public SaveAdminEditsForPatientProfileRequest(EntityRef patientProfileRef, PatientProfileDetail patientDetail)
        {
            this.PatientProfileRef = patientProfileRef;
            this.PatientDetail = patientDetail;
        }

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public PatientProfileDetail PatientDetail;       
    }
}
