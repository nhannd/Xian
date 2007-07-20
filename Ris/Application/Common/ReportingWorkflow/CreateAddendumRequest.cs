using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CreateAddendumRequest : DataContractBase
    {
        public CreateAddendumRequest(EntityRef verificationStepRef)
        {
            this.VerificationStepRef = verificationStepRef;
        }

        [DataMember]
        public EntityRef VerificationStepRef;
    }
}
