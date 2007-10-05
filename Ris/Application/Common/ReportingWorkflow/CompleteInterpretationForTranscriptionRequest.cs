using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CompleteInterpretationForTranscriptionRequest : DataContractBase
    {
        public CompleteInterpretationForTranscriptionRequest(EntityRef interpretationStepRef)
        {
            this.InterpretationStepRef = interpretationStepRef;
        }

        public CompleteInterpretationForTranscriptionRequest(EntityRef interpretationStepRef, string reportContent)
        {
            this.InterpretationStepRef = interpretationStepRef;
            this.ReportContent = reportContent;
        }

        public CompleteInterpretationForTranscriptionRequest(EntityRef interpretationStepRef, string reportContent, EntityRef supervisorRef)
        {
            this.InterpretationStepRef = interpretationStepRef;
            this.ReportContent = reportContent;
            this.SupervisorRef = supervisorRef;
        }


        [DataMember]
        public EntityRef InterpretationStepRef;

        [DataMember]
        public string ReportContent;

        [DataMember]
        public EntityRef SupervisorRef;
    }
}