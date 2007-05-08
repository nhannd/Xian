using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class LoadPatientProfileForBiographyRequest : DataContractBase
    {
        public LoadPatientProfileForBiographyRequest(EntityRef patientProfileRef)
        {
            this.PatientProfileRef = patientProfileRef;
        }

        [DataMember]
        public EntityRef PatientProfileRef;
    }
}
