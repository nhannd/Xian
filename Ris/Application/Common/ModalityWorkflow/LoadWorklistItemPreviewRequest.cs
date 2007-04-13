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
        [DataMember]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public string PatientProfileAuthority;
    }
}
