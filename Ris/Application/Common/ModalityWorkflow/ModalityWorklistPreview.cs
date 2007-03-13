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
        [DataMember(IsRequired = true)]
        public EntityRef MPSRef;

        [DataMember]
        public EntityRef PatientProfile;

        // TODO: Technologist home page hasn't been defined yet.
    }
}
