using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CompleteInterpretationForVerificationRequest : DataContractBase
    {
        public CompleteInterpretationForVerificationRequest(EntityRef interpretationStepRef)
        {
            this.InterpretationStepRef = interpretationStepRef;
        }

        public CompleteInterpretationForVerificationRequest(EntityRef interpretationStepRef, string reportContent)
        {
            this.InterpretationStepRef = interpretationStepRef;
            this.ReportContent = reportContent;
        }

        [DataMember]
        public EntityRef InterpretationStepRef;

        [DataMember]
        public string ReportContent;
    }
}