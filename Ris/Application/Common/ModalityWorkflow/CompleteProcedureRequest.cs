using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class CompleteProcedureRequest : DataContractBase
    {
        public CompleteProcedureRequest(EntityRef modalityProcedureStepRef)
        {
            ModalityProcedureStepRef = modalityProcedureStepRef;
        }
       
        [DataMember]
        public EntityRef ModalityProcedureStepRef;
    }
}
