using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CompleteInterpretationForTranscriptionResponse : DataContractBase
    {
        [DataMember]
        public EntityRef InterpretationStepRef;

        [DataMember]
        public EntityRef TranscriptionStepRef;
    }
}