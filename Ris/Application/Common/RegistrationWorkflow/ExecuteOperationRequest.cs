using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class ExecuteOperationRequest : DataContractBase
    {
        public ExecuteOperationRequest(EntityRef procedureStepRef, string operationClassName)
        {
            this.ProcedureStepRef = procedureStepRef;
            this.OperationClassName = operationClassName;
        }

        [DataMember]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public string OperationClassName;
    }
}
