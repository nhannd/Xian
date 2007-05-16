using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class LoadWorklistItemPreviewRequest : DataContractBase
    {
        public LoadWorklistItemPreviewRequest(EntityRef procedureStepRef, string patientProfileAuthority)
        {
            this.ProcedureStepRef = procedureStepRef;
            this.PatientProfileAuthority = patientProfileAuthority;
        }

        [DataMember]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public string PatientProfileAuthority;
    }
}
