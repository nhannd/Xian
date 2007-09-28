using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class ModalityPerformedProcedureStepSummary : DataContractBase
    {
        public ModalityPerformedProcedureStepSummary(EntityRef modalityPerformendProcedureStepRef, string inheritedName, EnumValueInfo state, DateTime startTime, DateTime? endTime, string performer, List<ModalityProcedureStepDetail> modalityProcedureSteps)
        {
            ModalityPerformendProcedureStepRef = modalityPerformendProcedureStepRef;
            InheritedName = inheritedName;
            State = state;
            StartTime = startTime;
            EndTime = endTime;
            Performer = performer;
            this.ModalityProcedureSteps = modalityProcedureSteps;
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

        /// <summary>
        /// Modality procedure steps that were performed with this performed procedure step.
        /// </summary>
        [DataMember]
        public List<ModalityProcedureStepDetail> ModalityProcedureSteps;
    }
}