using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class RequestedProcedureDetail : DataContractBase
    {
        [DataMember]
        public EntityRef RequestedProcedureRef;

        [DataMember]
        public string Name;

        [DataMember]
        public EnumValueInfo Status;
        
        [DataMember]
        public List<ModalityProcedureStepDetail> ModalityProcedureSteps;
    }
}