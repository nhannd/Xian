using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CompleteAddendumRequest : DataContractBase
    {
        public CompleteAddendumRequest(EntityRef verificationStepRef, EntityRef addendumStepRef, string addendumContent)
        {
            this.VerificationStepRef = verificationStepRef;
            this.AddendumStepRef = addendumStepRef;
            this.AddendumContent = addendumContent;
        }

        [DataMember]
        public EntityRef VerificationStepRef;

        [DataMember]
        public EntityRef AddendumStepRef;

        [DataMember]
        public string AddendumContent;
    }
}
