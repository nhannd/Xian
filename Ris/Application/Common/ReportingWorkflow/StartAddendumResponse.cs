using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class StartAddendumResponse : DataContractBase
    {
        [DataMember]
        public EntityRef VerificationStepRef;

        [DataMember]
        public EntityRef AddendumStepRef;
    }
}
