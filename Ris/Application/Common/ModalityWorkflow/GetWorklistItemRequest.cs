using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class GetWorklistItemRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public EntityRef MPSRef;

        [DataMember]
        public string PatientProfileAuthority;
    }
}
