using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CompleteInterpretationForVerificationRequest : DataContractBase
    {
        public CompleteInterpretationForVerificationRequest(EntityRef interpretationStepRef)
            : this(interpretationStepRef, null, null)
        {
        }

        public CompleteInterpretationForVerificationRequest(EntityRef interpretationStepRef, string reportContent)
            : this(interpretationStepRef, reportContent, null)
        {
        }

        public CompleteInterpretationForVerificationRequest(EntityRef interpretationStepRef, string reportContent, EntityRef supervisorRef)
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