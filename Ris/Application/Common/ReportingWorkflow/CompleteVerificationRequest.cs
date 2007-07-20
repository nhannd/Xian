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

        [DataMember]
        public EntityRef VerificationStepRef;
    }
}