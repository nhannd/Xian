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
        public ExecuteOperationRequest(ModalityWorklistItem item, string operationClassName)
        {
            this.ModalityWorklistItem = item;
            this.OperationClassName = operationClassName;
        }

        [DataMember]
        public ModalityWorklistItem ModalityWorklistItem;

        [DataMember]
        public string OperationClassName;
    }
}
