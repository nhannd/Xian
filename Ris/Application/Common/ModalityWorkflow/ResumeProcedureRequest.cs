using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class ResumeProcedureRequest : DataContractBase
    {
        public ResumeProcedureRequest(EntityRef modalityProcedureStepRef)
        {
            ModalityProcedureStepRef = modalityProcedureStepRef;
        }

        [DataMember]
        public EntityRef ModalityProcedureStepRef;
    }
}
