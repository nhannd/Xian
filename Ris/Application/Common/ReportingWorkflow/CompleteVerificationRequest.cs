using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CompleteVerificationRequest : DataContractBase
    {
        public CompleteVerificationRequest(EntityRef verificationStepRef)
        {
            this.VerificationStepRef = verificationStepRef;
        }

        public CompleteVerificationRequest(EntityRef verificationStepRef, string reportContent)
        {
            this.VerificationStepRef = verificationStepRef;
            this.ReportContent = reportContent;
        }

        [DataMember]
        public EntityRef VerificationStepRef;

        [DataMember]
        public string ReportContent;
    }
}