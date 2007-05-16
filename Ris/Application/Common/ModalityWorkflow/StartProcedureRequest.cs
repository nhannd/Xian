using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class StartProcedureRequest : DataContractBase
    {
        public StartProcedureRequest(EntityRef modalityProcedureStepRef)
        {
            ModalityProcedureStepRef = modalityProcedureStepRef;
        }

        [DataMember]
        public EntityRef ModalityProcedureStepRef;
    }
}
