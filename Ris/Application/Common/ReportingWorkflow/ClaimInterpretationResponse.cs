using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ClaimInterpretationResponse : DataContractBase
    {
        public ClaimInterpretationResponse(EntityRef interpretationStepRef)
        {
            this.InterpretationStepRef = interpretationStepRef;
        }

        [DataMember]
        public EntityRef InterpretationStepRef;
    }
}