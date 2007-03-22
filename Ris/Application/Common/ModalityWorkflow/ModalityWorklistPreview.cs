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
        public ModalityWorklistPreview(EntityRef procedureStepRef, EntityRef patientProfileRef)
        {
            this.ProcedureStepRef = procedureStepRef;
            this.PatientProfile = patientProfileRef;
        }

        public ModalityWorklistPreview()
        {
        }

        [DataMember(IsRequired = true)]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public EntityRef PatientProfile;

        // TODO: Technologist home page hasn't been defined yet.
    }
}
