using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    [DataContract]
    public class AdminAddPatientProfileRequest : DataContractBase
    {
        public AdminAddPatientProfileRequest(PatientProfileDetail patientDetail)
        {
            this.PatientDetail = patientDetail;
        }

        [DataMember]
        public PatientProfileDetail PatientDetail;       
    }
}
