using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class ModalityPerformedProcedureStepSummary : DataContractBase
    {
        public ModalityPerformedProcedureStepSummary(EntityRef modalityPerformendProcedureStepRef, string inheritedName, EnumValueInfo state, DateTime startTime, DateTime? endTime, string performer)
        {
            ModalityPerformendProcedureStepRef = modalityPerformendProcedureStepRef;
            InheritedName = inheritedName;
            State = state;
            StartTime = startTime;
            EndTime = endTime;
            Performer = performer;
        }

        [DataMember]
        public EntityRef ModalityPerformendProcedureStepRef;

        [DataMember]
        public string InheritedName;

        [DataMember]
        public EnumValueInfo State;

        [DataMember]
        public DateTime StartTime;

        [DataMember]
        public DateTime? EndTime;

        [DataMember]
        public string Performer;
    }
}