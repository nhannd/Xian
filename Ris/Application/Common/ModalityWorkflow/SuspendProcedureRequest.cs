using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class SuspendProcedureRequest : DataContractBase
    {
        public SuspendProcedureRequest(EntityRef modalityProcedureStepRef)
        {
            ModalityProcedureStepRef = modalityProcedureStepRef;
        }

        [DataMember]
        public EntityRef ModalityProcedureStepRef;
    }
}
