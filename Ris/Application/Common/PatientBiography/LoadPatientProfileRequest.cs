using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientBiography
{
    [DataContract]
    public class LoadPatientProfileRequest : DataContractBase
    {
        public LoadPatientProfileRequest(EntityRef patientProfileRef)
        {
            this.PatientProfileRef = patientProfileRef;
        }

        [DataMember]
        public EntityRef PatientProfileRef;
    }
}
