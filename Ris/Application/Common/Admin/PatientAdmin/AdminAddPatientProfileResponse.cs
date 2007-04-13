using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    [DataContract]
    public class AdminAddPatientProfileResponse : DataContractBase
    {
        public AdminAddPatientProfileResponse(EntityRef patientRef, EntityRef profileRef)
        {
            this.PatientRef = patientRef;
            this.PatientProfileRef = profileRef;
        }

        /// <summary>
        /// Ref to the newly created patient profile
        /// </summary>
        [DataMember]
        public EntityRef PatientProfileRef;

        /// <summary>
        /// Ref to the newly created patient
        /// </summary>
        [DataMember]
        public EntityRef PatientRef;
    }
}
