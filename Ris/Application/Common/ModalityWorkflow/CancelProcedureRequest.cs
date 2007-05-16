using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class CancelProcedureRequest : DataContractBase
    {
        public CancelProcedureRequest(EntityRef modalityProcedureStepRef)
        {
            ModalityProcedureStepRef = modalityProcedureStepRef;
        }
        
        [DataMember]
        public EntityRef ModalityProcedureStepRef;
    }
}
