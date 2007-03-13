using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class ExecuteOperationRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public EntityRef MPSRef;

        [DataMember(IsRequired=true)]
        public string OperationClassName;
    }
}
