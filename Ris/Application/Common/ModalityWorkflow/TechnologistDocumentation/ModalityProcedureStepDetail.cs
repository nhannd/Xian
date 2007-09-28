using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class ModalityProcedureStepDetail : DataContractBase
    {
        [DataMember]
        public EntityRef ModalityProcedureStepRef;

        [DataMember]
        public string Name;

        [DataMember]
        public EnumValueInfo Status;
        
        [DataMember]
        public DateTime? StartDateTime;

        [DataMember]
        public DateTime? EndDateTime;

        [DataMember]
        public string ModalityId;

        [DataMember]
        public string ModalityName;

    }
}