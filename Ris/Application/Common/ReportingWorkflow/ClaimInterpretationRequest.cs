using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ClaimInterpretationRequest : DataContractBase
    {
        public ClaimInterpretationRequest(EntityRef interpretationStepRef)
        {
            this.InterpretationStepRef = interpretationStepRef;
        }

        [DataMember]
        public EntityRef InterpretationStepRef;
    }
}