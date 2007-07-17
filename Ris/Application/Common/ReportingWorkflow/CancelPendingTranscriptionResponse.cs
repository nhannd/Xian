using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CancelPendingTranscriptionResponse : DataContractBase
    {
        [DataMember]
        public EntityRef TranscriptionStepRef;

        [DataMember]
        public EntityRef InterpretationStepRef;
    }
}